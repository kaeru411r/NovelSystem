using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �w�i�摜���A�X���ς����A�ɗ]�����o���Ȃ��悤�ɂ��邽�߂̃R���|�[�l���g
/// </summary>
[RequireComponent(typeof(RectTransform),(typeof(Image)))]
public class BackGroundFitter : MonoBehaviour
{
    /// <summary>�w�i�摜</summary>
    Sprite _sprite;
    /// <summary></summary>
    RectTransform _rectTransform;


    // Start is called before the first frame update
    void Start()
    {
        _sprite = GetComponent<Image>().sprite;
        _rectTransform = GetComponent<RectTransform>(); 
    }

    // Update is called once per frame
    void Update()
    {
    }
}
