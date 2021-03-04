using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class CustomTextBehavior : MonoBehaviour
{
    public TMP_Text customText;
    protected Image ButtonImage;

    public virtual void Start()
    {
        SetText();
    }

    public void SetText(string newText=null)
    {
        if(newText != null)
        {
            gameObject.name = newText;
        }
        customText.text = gameObject.name;
    }
}
