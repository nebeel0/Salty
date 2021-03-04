using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGroupTransitionBehavior : MonoBehaviour
{
    float transitionTime = 0.35f;
    bool transitionIn;
    bool transitionOut;


    void Start()
    {
        if (transitionOut != true)
        {
            TransitionIn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        TransitionUpdate();
    }

    void TransitionUpdate()
    {
        if (transitionIn)
        {
            GetComponent<CanvasGroup>().alpha += Time.deltaTime / transitionTime;
            if (GetComponent<CanvasGroup>().alpha >= 1)
            {
                transitionIn = false;
            }
        }
        else if (transitionOut)
        {
            GetComponent<CanvasGroup>().alpha -= Time.deltaTime / transitionTime;
            if (GetComponent<CanvasGroup>().alpha <= 0)
            {
                transitionOut = false;
                gameObject.SetActive(false);
            }
        }
    }

    public void TransitionIn()
    {
        if (GetComponent<CanvasGroup>() == null)
        {
            gameObject.AddComponent<CanvasGroup>();
        }
        GetComponent<CanvasGroup>().alpha = 0;
        gameObject.SetActive(true);
        transitionIn = true;
        transitionOut = false;
    }

    public void TransitionOut()
    {
        transitionIn = false;
        transitionOut = true;
    }
}
