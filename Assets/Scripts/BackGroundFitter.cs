using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背景画像をアス比を変えず、に余白を出さないようにするためのコンポーネント
/// </summary>
[RequireComponent(typeof(RectTransform),(typeof(Image)))]
public class BackGroundFitter : MonoBehaviour
{
    /// <summary>背景画像</summary>
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
