using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CustomButtonBehavior : MonoBehaviour
{
    public bool isSelected;
    Button thisButton;
    MenuManager menuManager;

    private void OnAwake()
    {
        gameObject.GetComponentInChildren<TMP_Text>().text = " " + gameObject.name + " ";
        gameObject.GetComponentInChildren<TMP_Text>().faceColor = Color.white;
    }

    void Start()
    {
        thisButton = GetComponent<Button>();
        thisButton.onClick.AddListener(TaskOnClick);
        menuManager = GameObject.FindWithTag("MenuManager").GetComponent<MenuManager>();
    }

    void Update()
    {
        gameObject.GetComponentInChildren<TMP_Text>().text = " " + gameObject.name + " ";
        gameObject.GetComponentInChildren<TMP_Text>().faceColor = Color.white;
    }

    void TaskOnClick()
    {
        isSelected = true;
        menuManager.PressButton(gameObject.name, gameObject);
    }

}
