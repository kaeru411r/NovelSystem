using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextWriter : MonoSequentialActor
{
    [Tooltip("�e�L�X�g�E�B���h�E")]
    [SerializeField] TMP_Text _text;
    [Tooltip("�l�[���E�B���h�E")]
    [SerializeField] TMP_Text _name;
    [Tooltip("�e�L�X�g�̐i�s�X�s�[�h")]
    [SerializeField] float _writeSpeed;


    public override IEnumerator Activity(string[] command, SkipToken token)
    {
        _name.text = command[1];
        _text.text = "";

        foreach (char c in command[2])
        {
            _text.text += c;
            yield return new WaitForSeconds(1 / _writeSpeed);

            if (token.IsSkip)
            {
                _text.text = command[2];
                yield break;
            }
        }
    }
}