using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    static ObjectManager _instance;
    public static ObjectManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType<ObjectManager>();
                if (!_instance)
                {
                    Debug.LogError($"{typeof(ObjectManager)}が見つかりませんでした");
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }


    [Tooltip("背景の管理用オブジェクト")]
    [SerializeField] RectTransform _backGround;
    [Tooltip("背景エフェクト管理用オブジェクト")]
    [SerializeField] RectTransform _backEfect;
    [Tooltip("キャラ管理用オブジェクト")]
    [SerializeField] RectTransform _character;
    [Tooltip("前面エフェクト管理用オブジェクト")]
    [SerializeField] RectTransform _frontEfect;
    [Tooltip("UI層管理用オブジェクト")]
    [SerializeField] RectTransform _ui;
    [Tooltip("最前面マスク管理用オブジェクト")]
    [SerializeField] RectTransform _frontMask;

    /// <summary>背景の管理用オブジェクト</summary>
    public RectTransform BackGround { get => _backGround; set => _backGround = value; }
    /// <summary>背景エフェクト管理用オブジェクト</summary>
    public RectTransform BackEfect { get => _backEfect; set => _backEfect = value; }
    /// <summary>キャラ管理用オブジェクト</summary>
    public RectTransform Character { get => _character; set => _character = value; }
    /// <summary>前面エフェクト管理用オブジェクト</summary>
    public RectTransform FrontEfect { get => _frontEfect; set => _frontEfect = value; }
    /// <summary>最前面マスク管理用オブジェクト</summary>
    public RectTransform FrontMask { get => _frontMask; set => _frontMask = value; }
    /// <summary>UI層管理用オブジェクト</summary>
    public RectTransform UI { get => _ui; set => _ui = value; }


    public GameObject Find(string layerGroup, string name)
    {
        if (Enum.TryParse<UILayerGroup>(layerGroup, out UILayerGroup result))
        {
            return Find(result,name);
        }
        else
        {
            Debug.LogError($"{nameof(layerGroup)}は不正な入力です。");
            return null;
        }
    }
    public GameObject Find(UILayerGroup layerGroup, string name)
    {
        switch (layerGroup)
        {
            case UILayerGroup.BackGround:
                return _backGround.Find(name).gameObject;
            case UILayerGroup.BackEfect:
                return _backEfect.Find(name).gameObject;
            case UILayerGroup.Character:
                return _character.Find(name).gameObject;
            case UILayerGroup.FrontEfect:
                return _frontEfect.Find(name).gameObject;
            case UILayerGroup.UI:
                return _ui.Find(name).gameObject;
            case UILayerGroup.FrontMask:
                return _frontMask.Find(name).gameObject;
            default:
                return null;
        }
    }

    public bool TryFind(string layerGroup, string name, out GameObject gameObject)
    {
        return TryFind(Enum.Parse<UILayerGroup>(layerGroup), name, out gameObject);
    }
    public bool TryFind(UILayerGroup layerGroup, string name, out GameObject gameObject)
    {
        gameObject = Find(layerGroup, name);
        return (bool)gameObject;
    }

    public void MoveLayer(string layerGroup, string name, int number)
    {
        MoveLayer(Enum.Parse<UILayerGroup>(layerGroup), name, number);
    }

    public void MoveLayer(UILayerGroup layerGroup, string name, int number)
    {
        if (TryFind(layerGroup, name, out GameObject go))
        {
            number = Mathf.Clamp(number, 0, go.transform.parent.childCount - 1);
            go.transform.SetSiblingIndex(number);
        }
    }

    public void MoveLayerLast(string layerGroup, string name)
    {
        MoveLayerLast(Enum.Parse<UILayerGroup>(layerGroup), name);
    }
    public void MoveLayerLast(UILayerGroup layerGroup, string name)
    {
        if (TryFind(layerGroup, name, out GameObject go))
        {
            go.transform.SetAsLastSibling();
        }
    }


    public enum UILayerGroup
    {
        /// <summary>背景</summary>
        BackGround,
        /// <summary>背景エフェクト</summary>
        BackEfect,
        /// <summary>キャラクター</summary>
        Character,
        /// <summary>前面エフェクト</summary>
        FrontEfect,
        /// <summary>UI</summary>
        UI,
        /// <summary>最前面マスク</summary>
        FrontMask,
    }
}
