using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMenuBehavior : MonoBehaviour
{
    // TODO offload transition to another class
    MenuManager menuManager;

    float transitionTime = 3;
    bool transitionIn;
    bool transitionOut;
    void Start()
    {
        menuManager = GameObject.FindWithTag("MenuManager").GetComponent<MenuManager>();
        TransitionIn();
    }

    // Update is called once per frame
    void Update()
    {
        TransitionUpdate();
    }

    void TransitionUpdate()
    {
        if(transitionIn)
        {
            GetComponent<CanvasGroup>().alpha += Time.deltaTime / transitionTime;
            if(GetComponent<CanvasGroup>().alpha >= 1)
            {
                transitionIn = false;
            }    
        }
        if (transitionOut)
        {
            GetComponent<CanvasGroup>().alpha -= Time.deltaTime / transitionTime;
            if (GetComponent<CanvasGroup>().alpha <= 0)
            {
                transitionOut = false;
                Destroy(gameObject);
            }
        }
    }

    void TransitionIn()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        transitionIn = true;
    }

    public void TransitionOut()
    {
        transitionOut = true;
    }

}
