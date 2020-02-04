using System;
using UnityEngine;
using UnityEngine.UI;

public class PrototypeMenuButton : MonoBehaviour
{
    [SerializeField]
    private bool popoutShown;
    public bool PopoutShown
    {
        get { return this.popoutShown; }
        set
        {
            if(this.popoutShown != value)
            {
                this.popoutShown = value;
                UpdateVisuals();
            }
        }
    }

    [SerializeField]
    private GameObject popout;
    [SerializeField]
    private Image buttonImage;
    [SerializeField]
    private Sprite shownSprite;
    [SerializeField]
    private Sprite unshownSprite;

    public bool MenuVisible { get; private set; }

    private void Start()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        popout.SetActive(popoutShown);
        buttonImage.sprite = popoutShown ? shownSprite : unshownSprite;
    }
}