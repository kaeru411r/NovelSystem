using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;


/// <summary>
/// メインシーケンスを進行するクラス
/// </summary>
public class ScenarioSequencer : MonoBehaviour
{
    [Tooltip("CSVファイルの名前")]
    [SerializeField] string _fileName;
    [Tooltip("オブジェクト管理用オブジェクト")]
    [SerializeField] ObjectManager _objectManager;
    [Tooltip("各処理用コンポーネント")]
    [SerializeField] List<MonoSequentialActor> _sequentialActors;
    [Tooltip("自動再生")]
    [SerializeField] bool _isAuto;

    const int csvOffsetNumber = 1;
    const int commandTypeIndex = 0;

    public bool IsAuto { get => _isAuto; set => _isAuto = value; }

    private void Start()
    {
        StartCoroutine(MainSequence(CSVReader.Read(_fileName, csvOffsetNumber)));
    }


    IEnumerator MainSequence(string[][] sequence)
    {
        List<YieldInstruction> enumeratorList = new List<YieldInstruction>();
        SkipSource skipSource = new SkipSource();

        foreach (string[] command in sequence)
        {
            if (command.Length <= commandTypeIndex && command[commandTypeIndex] == "") { break; }
            CommandType commandType = CommandType.NaN;
            if (Enum.TryParse<CommandType>(command[commandTypeIndex], out commandType))
            {   // コマンドタイプをEnumに変換
                //  対応するアクターの抽出
                MonoSequentialActor actor = _sequentialActors?.Where(a => a.CommandType == commandType).ToArray().FirstOrDefault();
                //  シーケンスを実行
                var activity = actor?.StartActivity(command, skipSource.Token);
                if (activity != null)
                {
                    enumeratorList.Add(activity.Value.sequence);
                    if (activity.Value.isWait)
                    {   //  シーケンスが待機命令を出したら
                        yield return Activity(enumeratorList, WaitForGetMouseButtonDown, skipSource);
                        skipSource = new SkipSource();
                    }
                }
                else
                {   //  アクターが登録されていなかったら
                    Debug.LogError($"{command[commandTypeIndex]}は{nameof(MonoSequentialActor)}が登録されていません。");
                    continue;
                }
            }
            else
            {
                Debug.LogError($"{command[commandTypeIndex]}は不正なコマンドです。");
                continue;
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
    IEnumerator Activity(List<YieldInstruction> enumerators, Func<IEnumerator> waitAndSkip, SkipSource skipSource)
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
    IEnumerator Activity(List<YieldInstruction> enumerators, Func<IEnumerator> wait, Func<IEnumerator> skip, SkipSource skipSource)
    {
        bool isComplete = false;
        Coroutine skipCoroutine = StartCoroutine(skip?.Invoke());
        yield return WaitAnyCoroutine(WaitCoroutine(WaitAllCo(enumerators.ToArray()), () => isComplete = true), skipCoroutine);
        if (!isComplete)
        {
            skipSource.Skip();
        }

        if (wait != null)
        {
            yield return WaitAnyCoroutine(StartCoroutine(wait?.Invoke()), WaitUntilCoroutine(() => { return _isAuto; }));
        }

        enumerators.Clear();
    }

    IEnumerator WaitUntil(Func<bool> predicate)
    {
        yield return new WaitUntil(predicate);
    }
    Coroutine WaitUntilCoroutine(Func<bool> predicate)
    {
        return StartCoroutine(new WaitUntil(predicate));
    }

    /// <summary>
    /// <paramref name="enumerators"/>のいずれかの完了を待つ
    /// </summary>
    /// <param name="enumerators"></param>
    Coroutine WaitAnyCoroutine(params YieldInstruction[] enumerators)
    {
        return StartCoroutine(WaitAny(enumerators));

    }
    IEnumerator WaitAny(params YieldInstruction[] enumerators)
    {
        bool wait = false;

        foreach (YieldInstruction enumerator in enumerators)
        {
            WaitCoroutine(enumerator, () => wait = true);
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
    Coroutine WaitAllCo(params YieldInstruction[] enumerators)
    {
        return StartCoroutine(WaitAll(enumerators));

    }
    IEnumerator WaitAll(params YieldInstruction[] enumerators)
    {
        int count = 0;

        foreach (YieldInstruction enumerator in enumerators)
        {
            WaitCoroutine(enumerator, () => count++);
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
    Coroutine WaitCoroutine(YieldInstruction enumerator, Action action)
    {
        return StartCoroutine(Wait(enumerator, action));

    }
    IEnumerator Wait(YieldInstruction enumerator, Action action)
    {
        yield return enumerator;
        action?.Invoke();
    }
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
