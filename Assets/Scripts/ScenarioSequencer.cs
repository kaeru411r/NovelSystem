using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

public class ScenarioSequencer : MonoBehaviour
{
    [Tooltip("CSVファイルの名前")]
    [SerializeField] string _fileName;
    [Tooltip("テキスト表示用コンポーネント")]
    [SerializeField] MonoSequentialActor _textWriter;
    [Tooltip("ImageFade用コンポーネント")]
    [SerializeField] MonoSequentialActor _fader;

    const int csvOffsetNumber = 1;

    private void Start()
    {
        StartCoroutine(MainSequence(CSVReader.Read(_fileName, csvOffsetNumber)));
    }


    IEnumerator MainSequence(string[][] sequence)
    {
        List<IEnumerator> enumeratorList = new List<IEnumerator>();
        SkipSource skipSource = new SkipSource();

        foreach (string[] command in sequence)
        {
            CommandType commandType = CommandType.NaN;
            try
            {
                commandType = Enum.Parse<CommandType>(command[0]);
            }
            catch (ArgumentException e)
            {
                Debug.LogError($"{nameof(command)}は不正なコマンドです。");
                continue;
            }
            switch (commandType)
            {
                case CommandType.Write:
                    enumeratorList.Add(_textWriter.Activity(command, skipSource.Token));
                    yield return Activity(enumeratorList, WaitForGetMouseButtonDown, skipSource);
                    skipSource = new SkipSource();
                    break;
                case CommandType.Fade:
                    enumeratorList.Add(_fader.Activity(command, skipSource.Token));
                    break;
            }
        }
    }

    /// <summary>
    /// いずれかのマウスボタンが押されるのを待つ
    /// </summary>
    IEnumerator WaitForGetMouseButtonDown()
    {
        while (true)
        {
            yield return null;

            for (int i = 0; i < 3; i++)
            {
                if (Input.GetMouseButtonDown(i))
                {
                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// <paramref name="enumerators"/>を実行する。<br></br>
    /// <paramref name="enumerators"/>か<paramref name="waitAndSkip"/>が完了するのを待つ<br></br>
    /// 完了後、<paramref name="enumerators"/>をクリアした後、再度<paramref name="waitAndSkip"/>の完了を待つ
    /// </summary>
    /// <param name="enumerators"></param>
    /// <param name="waitAndSkip"></param>
    /// <param name="skipSource"></param>
    IEnumerator Activity(List<IEnumerator> enumerators, Func<IEnumerator> waitAndSkip, SkipSource skipSource)
    {
        return Activity(enumerators, waitAndSkip, waitAndSkip, skipSource);
    }


    /// <summary>
    /// <paramref name="enumerators"/>を実行する。<br></br>
    /// <paramref name="enumerators"/>か<paramref name="skip"/>が完了するのを待つ<br></br>
    /// 完了後、<paramref name="enumerators"/>をクリアした後、<paramref name="wait"/>の完了を待つ
    /// </summary>
    /// <param name="enumerators"></param>
    /// <param name="wait"></param>
    /// <param name="skip"></param>
    /// <param name="skipSource"></param>
    IEnumerator Activity(List<IEnumerator> enumerators, Func<IEnumerator> wait, Func<IEnumerator> skip, SkipSource skipSource)
    {
        bool isComplete = false;
        yield return WaitAny(Wait(WaitAll(enumerators.ToArray()), () => isComplete = true), skip?.Invoke());
        if (!isComplete)
        {
            skipSource.Skip();
        }

        if (wait != null)
        {
            yield return wait.Invoke();
        }

        enumerators.Clear();
    }

    /// <summary>
    /// <paramref name="enumerators"/>のいずれかの完了を待つ
    /// </summary>
    /// <param name="enumerators"></param>
    IEnumerator WaitAny(params IEnumerator[] enumerators)
    {
        bool wait = false;

        foreach (IEnumerator enumerator in enumerators)
        {
            StartCoroutine(Wait(enumerator, () => wait = true));
        }

        while (!wait)
        {
            yield return null;
        }
    }

    /// <summary>
    /// <paramref name="enumerators"/>全ての完了を待つ
    /// </summary>
    /// <param name="enumerators"></param>
    IEnumerator WaitAll(params IEnumerator[] enumerators)
    {
        int count = 0;

        foreach (IEnumerator enumerator in enumerators)
        {
            StartCoroutine(Wait(enumerator, () => count++));
        }

        while (count < enumerators.Length)
        {
            yield return null;
        }
    }

    /// <summary>
    /// <paramref name="enumerator"/>が完了したら<paramref name="action"/>を実行する
    /// </summary>
    /// <param name="enumerator"></param>
    /// <param name="action"></param>
    IEnumerator Wait(IEnumerator enumerator, Action action)
    {
        yield return enumerator;
        action?.Invoke();
    }
}

enum CommandType
{
    NaN = -1,
    Wait,
    Write,
    Instantiate,
    Destroy,
    Fade,
    Color,
}

public class SkipSource
{
    public bool IsSkip { get => _isSkip; set => _isSkip = value; }

    public SkipToken Token { get => new SkipToken(this); }

    public void Skip()
    {
        _isSkip = true;
    }

    bool _isSkip = false;
}

public struct SkipToken
{
    public SkipToken(SkipSource skipSource)
    {
        _skipSource = skipSource;
    }

    public bool IsSkip { get => _skipSource.IsSkip; }

    SkipSource _skipSource;
}
