using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomMultiPanelBehavior : GameBehavior
{
    public CanvasGroupTransitionBehavior canvasGroupTransitionBehavior;

    public GameObject currentPanel;
    public GameObject toLoadPanel;

    public virtual void Death()
    {
        if (canvasGroupTransitionBehavior == null)
        {
            canvasGroupTransitionBehavior = gameObject.AddComponent<CanvasGroupTransitionBehavior>();
        }
        canvasGroupTransitionBehavior.TransitionOut();
    }

    protected virtual void Update()
    {
        if (toLoadPanel != null)
        {
            if (currentPanel == null || !currentPanel.activeSelf)
            {
                LoadInPanel();
            }
        }
    }

    public virtual void SetToLoadInPanel(GameObject toLoadPanel)
    {
        this.toLoadPanel = toLoadPanel;
        if(currentPanel != null)
        {
            LoadOutPanel();
        }
    }

    public virtual void LoadInPanel()
    {
        if (currentPanel != null)
        {
            currentPanel.gameObject.SetActive(false);
        }
        currentPanel = toLoadPanel;
        currentPanel.GetComponent<CanvasGroupTransitionBehavior>().TransitionIn();
        toLoadPanel = null;
    }

    public virtual void LoadOutPanel()
    {
        currentPanel.GetComponent<CanvasGroupTransitionBehavior>().TransitionOut();
    }

}
