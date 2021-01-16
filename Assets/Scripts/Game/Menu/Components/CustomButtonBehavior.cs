using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CustomButtonBehavior : MonoBehaviour, IPointerClickHandler
{
    protected MenuManager menuManager;
    protected TMP_Text ButtonText;
    protected Image ButtonImage;

    public virtual void Start()
    {
        menuManager = GameObject.FindWithTag("MenuManager").GetComponent<MenuManager>();

        ButtonText = gameObject.GetComponentInChildren<TMP_Text>();
        ButtonText.text = gameObject.name;
    }

    public virtual void OnPointerClick(PointerEventData pointerEventData) //TODO set background to white, and selected text to black
    {
        ClickedButton();
    }

    protected virtual void ClickedButton()
    {
        menuManager.PressButton(gameObject.name, gameObject);
    }

}
