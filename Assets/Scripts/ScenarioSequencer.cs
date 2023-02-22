using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;


/// <summary>
/// ���C���V�[�P���X��i�s����N���X
/// </summary>
public class ScenarioSequencer : MonoBehaviour
{
    [Tooltip("CSV�t�@�C���̖��O")]
    [SerializeField] string _fileName;
    [Tooltip("�I�u�W�F�N�g�Ǘ��p�I�u�W�F�N�g")]
    [SerializeField] ObjectManager _objectManager;
    [Tooltip("�e�����p�R���|�[�l���g")]
    [SerializeField] List<MonoSequentialActor> _sequentialActors;
    [Tooltip("�����Đ�")]
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
            {   // �R�}���h�^�C�v��Enum�ɕϊ�
                //  �Ή�����A�N�^�[�̒��o
                MonoSequentialActor actor = _sequentialActors?.Where(a => a.CommandType == commandType).ToArray().FirstOrDefault();
                //  �V�[�P���X�����s
                var activity = actor?.StartActivity(command, skipSource.Token);
                if (activity != null)
                {
                    enumeratorList.Add(activity.Value.sequence);
                    if (activity.Value.isWait)
                    {   //  �V�[�P���X���ҋ@���߂��o������
                        yield return Activity(enumeratorList, WaitForGetMouseButtonDown, skipSource);
                        skipSource = new SkipSource();
                    }
                }
                else
                {   //  �A�N�^�[���o�^����Ă��Ȃ�������
                    Debug.LogError($"{command[commandTypeIndex]}��{nameof(MonoSequentialActor)}���o�^����Ă��܂���B");
                    continue;
                }
            }
            else
            {
                Debug.LogError($"{command[commandTypeIndex]}�͕s���ȃR�}���h�ł��B");
                continue;
            }
        }
    }

    /// <summary>
    /// �����ꂩ�̃}�E�X�{�^�����������̂�҂�
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
    /// <paramref name="enumerators"/>�����s����B<br></br>
    /// <paramref name="enumerators"/>��<paramref name="waitAndSkip"/>����������̂�҂�<br></br>
    /// ������A<paramref name="enumerators"/>���N���A������A�ēx<paramref name="waitAndSkip"/>�̊�����҂�
    /// </summary>
    /// <param name="enumerators"></param>
    /// <param name="waitAndSkip"></param>
    /// <param name="skipSource"></param>
    IEnumerator Activity(List<YieldInstruction> enumerators, Func<IEnumerator> waitAndSkip, SkipSource skipSource)
    {
        return Activity(enumerators, waitAndSkip, waitAndSkip, skipSource);
    }


    /// <summary>
    /// <paramref name="enumerators"/>�����s����B<br></br>
    /// <paramref name="enumerators"/>��<paramref name="skip"/>����������̂�҂�<br></br>
    /// ������A<paramref name="enumerators"/>���N���A������A<paramref name="wait"/>�̊�����҂�
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
    /// <paramref name="enumerators"/>�̂����ꂩ�̊�����҂�
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
    /// <paramref name="enumerators"/>�S�Ă̊�����҂�
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
    /// <paramref name="enumerator"/>������������<paramref name="action"/>�����s����
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
