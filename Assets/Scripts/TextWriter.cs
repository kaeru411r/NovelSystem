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

    public override IEnumerator Activity(string[] command, SkipToken token)
    {
        _name.text = command[nameIndex];
        _text.text = "";

        if (ObjectManager.Instance.TryFind(ObjectManager.UILayerGroup.Character, command[nameIndex], out GameObject gameObject))
        {
            if(gameObject.TryGetComponent(out ActorController actor))
            {
                actor.Activate();
            }
        }

        foreach (char c in command[textIndex])
        {
            _text.text += c;
            yield return new WaitForSeconds(1 / _writeSpeed);

            if (token.IsSkip)
            {
                _text.text = command[textIndex];
                yield break;
            }
        }
    }
}