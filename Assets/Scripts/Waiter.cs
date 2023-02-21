using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter : MonoSequentialActor
{
    public override CommandType CommandType => CommandType.Wait;
    protected override IEnumerator Activity(string[] command, SkipToken token) { yield break; }
    protected override bool _isWait => true;
}
