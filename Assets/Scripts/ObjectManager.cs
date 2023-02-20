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
                    Debug.LogError($"{typeof(ObjectManager)}��������܂���ł���");
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


    [Tooltip("�w�i�̊Ǘ��p�I�u�W�F�N�g")]
    [SerializeField] RectTransform _backGround;
    [Tooltip("�w�i�G�t�F�N�g�Ǘ��p�I�u�W�F�N�g")]
    [SerializeField] RectTransform _backEfect;
    [Tooltip("�L�����Ǘ��p�I�u�W�F�N�g")]
    [SerializeField] RectTransform _character;
    [Tooltip("�O�ʃG�t�F�N�g�Ǘ��p�I�u�W�F�N�g")]
    [SerializeField] RectTransform _frontEfect;
    [Tooltip("UI�w�Ǘ��p�I�u�W�F�N�g")]
    [SerializeField] RectTransform _ui;
    [Tooltip("�őO�ʃ}�X�N�Ǘ��p�I�u�W�F�N�g")]
    [SerializeField] RectTransform _frontMask;

    /// <summary>�w�i�̊Ǘ��p�I�u�W�F�N�g</summary>
    public RectTransform BackGround { get => _backGround; set => _backGround = value; }
    /// <summary>�w�i�G�t�F�N�g�Ǘ��p�I�u�W�F�N�g</summary>
    public RectTransform BackEfect { get => _backEfect; set => _backEfect = value; }
    /// <summary>�L�����Ǘ��p�I�u�W�F�N�g</summary>
    public RectTransform Character { get => _character; set => _character = value; }
    /// <summary>�O�ʃG�t�F�N�g�Ǘ��p�I�u�W�F�N�g</summary>
    public RectTransform FrontEfect { get => _frontEfect; set => _frontEfect = value; }
    /// <summary>�őO�ʃ}�X�N�Ǘ��p�I�u�W�F�N�g</summary>
    public RectTransform FrontMask { get => _frontMask; set => _frontMask = value; }
    /// <summary>UI�w�Ǘ��p�I�u�W�F�N�g</summary>
    public RectTransform UI { get => _ui; set => _ui = value; }

    public void Instantiate(UILayerGroup layerGroup, string name, Vector2 pos)
    {
        GameObject go = Resources.Load(name) as GameObject;
        if (go)
        {
            Instantiate(go, pos, Quaternion.identity, GetGroup(layerGroup));
        }
        else
        {
            Debug.Log($"{nameof(go)}��������܂���ł���");
        }
    }

    public GameObject Find(string layerGroup, string name)
    {
        if (Enum.TryParse<UILayerGroup>(layerGroup, out UILayerGroup result))
        {
            return Find(result,name);
        }
        else
        {
            Debug.LogError($"{nameof(layerGroup)}�͕s���ȓ��͂ł��B");
            return null;
        }
    }
    public GameObject Find(UILayerGroup layerGroup, string name)
    {
        return GetGroup(layerGroup)?.Find(name).gameObject;
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

    RectTransform GetGroup(UILayerGroup layerGroup)
    {
        switch (layerGroup)
        {
            case UILayerGroup.BackGround:
                return _backGround;
            case UILayerGroup.BackEfect:
                return _backEfect;
            case UILayerGroup.Character:
                return _character;
            case UILayerGroup.FrontEfect:
                return _frontEfect;
            case UILayerGroup.UI:
                return _ui;
            case UILayerGroup.FrontMask:
                return _frontMask;
            default:
                return null;
        }
    }

    public enum UILayerGroup
    {
        /// <summary>�w�i</summary>
        BackGround,
        /// <summary>�w�i�G�t�F�N�g</summary>
        BackEfect,
        /// <summary>�L�����N�^�[</summary>
        Character,
        /// <summary>�O�ʃG�t�F�N�g</summary>
        FrontEfect,
        /// <summary>UI</summary>
        UI,
        /// <summary>�őO�ʃ}�X�N</summary>
        FrontMask,
    }
}
