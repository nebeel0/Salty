using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressAnyKey : MonoBehaviour
{
    public CustomMenuBehavior currentMenu;
    FadeFlicker fadeFlicker;

    // Start is called before the first frame update
    void Start()
    {
        fadeFlicker = GetComponent<FadeFlicker>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey && fadeFlicker.loadingOffset <= 0)
        {
            fadeFlicker.turnOff = true;
        }
        if(fadeFlicker.turnOff && fadeFlicker.faceColor.a <= 0)
        {
            Debug.Log("Turning off press any key");
            currentMenu.PressButton(gameObject);
        }
    }
}
