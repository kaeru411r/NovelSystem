using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoSequentialActor
{
    const int layerIndex = 1;
    const int nameIndex = 2;
    const int colorIndex = 3;
    const int timeIndex = 4;

    public override CommandType CommandType => CommandType.Fade;
    protected override bool _isWait => false;

    protected override IEnumerator Activity(string[] command, SkipToken token)
    {
        GameObject go = ObjectManager.Instance.Find(command[layerIndex], command[nameIndex]);
        if (go?.TryGetComponent(out Image image) is true)
        {
            Color start = image.color;
            float time = 0f;
            try
            {
                time = float.Parse(command[timeIndex]);
            }
            catch
            {
                Debug.LogError($"{nameof(command)}[{timeIndex}]が不正な値でした。");
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
                        yield break;
                    }
                }

                image.color = target;
            }
            else
            {
                Debug.LogError($"{nameof(command)}[{colorIndex}]は不正な値でした。");
            }
        }
        else
        {
            Debug.LogError($"{command[nameIndex]}の{nameof(Image)}が取得できませんでした。");
        }
    }
}
