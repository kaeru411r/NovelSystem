using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorutineTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(A());
        StartCoroutine(Main());
    }

    // Update is called once per frame
    void Update()
    {

    }


    IEnumerator A()
    {
        yield return B(C());
        Debug.Log("a");
    }

    IEnumerator B(IEnumerator coroutine)
    {
        yield return coroutine;
        Debug.Log("b");
    }

    IEnumerator C()
    {
        yield return null;
        Debug.Log("c");
    }


    IEnumerator Main()
    {
        while (true)
        {
            yield return WaiteForGetMouseButtonDown(value => { Debug.Log(value); });
        }
    }


    IEnumerator WaiteForGetMouseButtonDown(Action<int> action)
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                action.Invoke(0);
                yield break;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                action.Invoke(1);
                yield break;
            }
            else if (Input.GetMouseButtonDown(2))
            {
                action.Invoke(2);
                yield break;
            }

            yield return null;
        }
    }
}
