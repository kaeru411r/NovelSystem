using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoSequentialActor
{
    const int layerIndex = 1;
    const int nameIndex = 2;
    const int targetXIndex = 3;
    const int targetYIndex = 4;
    const int timeIndex = 5;

    public override CommandType CommandType => CommandType.Move;

    protected override IEnumerator Activity(string[] command, SkipToken token)
    {
        float time = float.Parse(command[timeIndex]);
        Vector2 target = new Vector2(float.Parse(command[targetXIndex]), float.Parse(command[targetYIndex]));

        if (ObjectManager.Instance.TryFind(command[layerIndex], command[nameIndex], out GameObject gameObject))
        {
            if (gameObject.TryGetComponent(out RectTransform transform))
            {
                Vector2 start = transform.anchoredPosition;
                for (float process = 0; process < time; process += Time.deltaTime)
                {
                    transform.anchoredPosition = Vector2.Lerp(start, target, process / time);
                    Debug.Log(transform.anchoredPosition);

                    if (token.IsSkip)
                    {
                        break;
                    }

                    yield return null;
                }
                transform.anchoredPosition = target;
            }
        }
    }

    protected override bool _isWait => false;
}
