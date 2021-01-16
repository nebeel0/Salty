using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TabBehavior : CustomButtonBehavior, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    protected Color FadedGray = new Color(0.15f, 0.15f, 0.15f, 0.15f);
    protected Color ClearGray = new Color(0.25f, 0.25f, 0.25f, 0.25f);
    protected Color GlowGray = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    protected Color SelectedGray = new Color(0.35f, 0.35f, 0.35f, 0.35f);

    bool Hovering = false;
    public bool Selected = false;

    public override void Start()
    {
        base.Start();
        ButtonImage = GetComponent<Image>();
        NormalState();
    }


    public void SetTextSize(int size)
    {
        ButtonText.fontSize = size;
    }

    public void DeSelect()
    {
        Selected = false;
        if(!Hovering)
        {
            NormalState();
        }
        else
        {
            HoverState();
        }
    }

    public void Select()
    {
        if(!Selected)
        {
            Selected = true;
            menuManager.TabRefresh(gameObject);
            SelectedState();
        }
    }

    public override void OnPointerClick(PointerEventData pointerEventData) //TODO set background to white, and selected text to black
    {
        Select();
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        ClickedState();
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (Hovering)
        {
            HoverState();
        }
    }

    public virtual void OnPointerEnter(PointerEventData pointerEventData)
    {
        HoverState();
        Hovering = true;
    }

    public virtual void OnPointerExit(PointerEventData pointerEventData)
    {
        NormalState();
        Hovering = false;
    }


    //Color Utils
    void NormalState()
    {
        if(!Selected)
        {
            ButtonImage.color = Color.clear;
            ButtonText.faceColor = ClearGray;
        }
    }
    void HoverState()
    {
        if (!Selected)
        {
            ButtonImage.color = FadedGray;
            ButtonText.faceColor = GlowGray;
        }
    }

    void ClickedState()
    {
        if (!Selected)
        {
            ButtonImage.color = FadedGray;
            ButtonText.faceColor = Color.white;
        }
    }
    void SelectedState()
    {
        ButtonImage.color = Color.clear;
        ButtonText.faceColor = Color.white;
    }
}
