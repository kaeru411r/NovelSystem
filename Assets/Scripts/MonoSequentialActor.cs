using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public abstract class MonoSequentialActor : MonoBehaviour 
{
    protected abstract bool _isWait { get; }
    public abstract CommandType CommandType { get; }

    protected abstract IEnumerator Activity(string[] command, SkipToken token);

    /// <summary>
    /// �����̎��s��ǉ�
    /// </summary>
    /// <param name="command"></param>
    /// <param name="token"></param>
    /// <param name="yieldInstructions"></param>
    /// <returns>������ҋ@���邩</returns>
    public (bool isWait, YieldInstruction sequence) StartActivity(string[] command, SkipToken token)
    {
        return (_isWait, StartCoroutine(Activity(command, token)));
    }
}
