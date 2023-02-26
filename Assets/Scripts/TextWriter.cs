using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextWriter : MonoSequentialActor
{
    [Tooltip("テキストウィンドウ")]
    [SerializeField] TMP_Text _text;
    [Tooltip("ネームウィンドウ")]
    [SerializeField] TMP_Text _name;
    [Tooltip("テキストの進行スピード")]
    [SerializeField] float _writeSpeed;

    const int nameIndex = 1;
    const int textIndex = 2;

    public override CommandType CommandType => CommandType.Write;
    protected override bool _isWait => true;

    protected override IEnumerator Activity(string[] command, SkipToken token)
    {
        _name.text = command[nameIndex];
        _text.text = "";

        if (ObjectManager.Instance.TryFind(ObjectManager.UILayerGroup.Character, command[nameIndex], out GameObject gameObject))
        {
            if (gameObject.TryGetComponent(out ActorController actor))
            {
                actor.Activate();
            }
        }

        float time = 0;
        float procces = 0;
        int index = 0;
        while (index < command[textIndex].Length)
        {

            if (token.IsSkip)
            {
                _text.text = command[textIndex];
                yield break;
            }

            if (time < procces)
            {
                yield return null;
                time += Time.deltaTime;
            }
            else
            {
                _text.text += command[textIndex][index];
                index++;
                procces += 1 / _writeSpeed;
            }
        }
    }
}