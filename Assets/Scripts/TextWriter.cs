using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextWriter : MonoBehaviour
{
    [Tooltip("テキスト")]
    [SerializeField] string[] _texts;
    [Tooltip("テキストウィンドウ")]
    [SerializeField] TMP_Text _text;
    [Tooltip("ネームウィンドウ")]
    [SerializeField] TMP_Text _name;
    [Tooltip("テキストの進行スピード")]
    [SerializeField] float _writeSpeed;

    int _index = 0;

    bool _isWriting = false;
    bool _skip = false;
    // Start is called before the first frame update
    void Start()
    {
        _isWriting = true;
        StartCoroutine(Writing(_texts));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_isWriting)
            {
                _skip = true;
            }
            else
            {
                _isWriting = true;
            }
        }
    }


    IEnumerator Writing(string[] texts)
    {
        while (_index < texts.Length)
        {
            if (_isWriting)
            {
                _text.text = "";

                if (_index >= _texts.Length)
                {
                    _index = 0;
                }
                for (int i = 0; i < texts[_index].Length; i++)
                {
                    if (!_isWriting)
                    {
                        break;
                    }
                    if (texts[_index][i] == '<')
                    {

                    }
                    else
                    {
                        _text.text += texts[_index][i];
                        if (!_skip)
                        {
                            yield return new WaitForSeconds(_writeSpeed);
                        }
                    }
                }
                _skip = false;
                _index++;
                _isWriting = false;
            }
            yield return null;
        }
    }
}