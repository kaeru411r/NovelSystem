using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Audio : MonoSequentialActor
{
    public override CommandType CommandType => CommandType.Audio;

    protected override bool _isWait => throw new System.NotImplementedException();

    protected override IEnumerator Activity(string[] command, SkipToken token)
    {
        throw new System.NotImplementedException();
    }
}
