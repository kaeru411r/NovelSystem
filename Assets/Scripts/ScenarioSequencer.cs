using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ScenarioSequencer : MonoBehaviour
{
    [SerializeField] string _fileName;

    private void Start()
    {
        StartCoroutine(MainSequence(CSVReader.Read(_fileName, 1)));
    }


    IEnumerator MainSequence(string[][] sequence)
    {
        List<IEnumerator> enumeratorList = new List<IEnumerator>();

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
            }
            switch (commandType)
            {
                case CommandType.Write:
                    Debug.Log("Write");
                    yield return null;
                    break;
            }
        }
    }

    IEnumerator A(float time)
    {
        yield return new WaitForSeconds(time);
    }

    IEnumerator Activity(List<IEnumerator> enumerators, IEnumerator waiter)
    {
        if (waiter == null)
        {
            yield return WaitAll(enumerators.ToArray());
        }
        else
        {
            yield return WaitAny(WaitAll(enumerators.ToArray()), waiter);
        }

        enumerators.Clear();
    }

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
