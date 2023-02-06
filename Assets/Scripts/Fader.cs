using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoSequentialActor
{
    const int nameIndex = 1;
    const int colorIndex = 2;
    const int timeIndex = 3;

    public override IEnumerator Activity(string[] command, SkipToken token)
    {
        if (GameObject.Find(command[nameIndex])?.TryGetComponent(out Image image) is true)
        {
            Color start = image.color;
            float time = 0f;
            try
            {
                time = float.Parse(command[timeIndex]);
            }
            catch
            {
                Debug.LogError($"{nameof(command)}[{timeIndex}]���s���Ȓl�ł����B");
                yield break;
            }
            if (ColorUtility.TryParseHtmlString(command[colorIndex], out Color target))
            {
                for (float p = 0; p < time; p += Time.deltaTime)
                {
                    image.color = Color.Lerp(start, target, p / time);
                    yield return null;

                    if (token.IsSkip)
                    {
                        image.color = target;
                        yield break;
                    }
                }
            }
            else
            {
                Debug.LogError($"{nameof(command)}[{colorIndex}]�͕s���Ȓl�ł����B");
            }
        }
        else
        {
            Debug.LogError($"{command[nameIndex]}��{nameof(Image)}���擾�ł��܂���ł����B");
        }
    }


}
