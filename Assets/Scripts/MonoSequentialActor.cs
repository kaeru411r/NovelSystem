using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSequentialActor : MonoBehaviour 
{
    protected abstract bool _isWait { get; }
    public abstract CommandType CommandType { get; }

    public abstract IEnumerator Activity(string[] command, SkipToken token);
    public bool Activity(string[] command, SkipToken token, ref List<YieldInstruction> yieldInstructions)
    {
        yieldInstructions.Add(ActivityCoroutine(command, token));
        return _isWait;
    }

    public Coroutine ActivityCoroutine(string[] command, SkipToken token)
    {
        return StartCoroutine(Activity(command, token));
    }
}
