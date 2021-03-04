using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CustomButtonBehavior : MonoBehaviour, IPointerClickHandler
{
    public CustomMenuBehavior currentMenu;
    public TMP_Text ButtonText;
    public bool nameSync = true;
    protected Image ButtonImage;

    public virtual void Start()
    {
        if(nameSync)
        {
            ButtonText.text = gameObject.name;
        }
    }

    public string GetText()
    {
        return ButtonText.text;
    }

    public void SetText(string text)
    {
        ButtonText.text = text;
    }

    public void CenterText()
    {
        ButtonText.alignment = TMPro.TextAlignmentOptions.Center;
    }

    public virtual void OnPointerClick(PointerEventData pointerEventData) //TODO set background to white, and selected text to black
    {
        ClickedButton();
    }


    protected virtual void ClickedButton()
    {
        currentMenu.PressButton(gameObject);
    }

}
