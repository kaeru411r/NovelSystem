using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ActorController : MonoBehaviour
{
    static Color inactiveColor = Color.gray;

    static ActorController active;

    Image _image;

    public Image Image
    {
        get
        {
            if (!_image)
            {
                _image = GetComponent<Image>();
                if (!_image)
                {
                    Debug.LogError($"{nameof(Image)}‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ‚Å‚µ‚½");
                }
            }
            return _image;
        }
    }

    public void Activate()
    {
        active?.InActivate();
        transform.SetAsLastSibling();
        Image.color = Color.white;
        active = this;
    }

    public void InActivate()
    {
        Image.color = inactiveColor;
    }


    private void Awake()
    {
        InActivate();
    }
}
