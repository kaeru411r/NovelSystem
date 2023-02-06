using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSequentialActor : MonoBehaviour 
{

    public abstract IEnumerator Activity(string[] command, SkipToken token);
}
