using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : CustomMultiPanelBehavior
{
    public Camera mainCamera;
    public Canvas overallCanvas;

    public GameObject startMenuRef;
    public GameObject playMenuRef;
    public GameObject endMenuRef;

    public GameObject pauseGroup;
    public DialoguePanelManager dialogueManager;
    //TODO use WorldSpaceGUITransform to get overall size of object, 
    //create a mock to take up space, load it under its subpanel, for Start Menu so that title won't overlap

    public override void Start()
    {
        base.Start();
        transform.position = Camera.main.transform.position;
    }

    public override void LoadInPanel()
    {
        if(currentPanel != null)
        {
            Destroy(currentPanel);
        }
        currentPanel = Instantiate(toLoadPanel, overallCanvas.gameObject.transform);
        currentPanel.GetComponent<CustomMenuBehavior>().menuManager = this;
        toLoadPanel = null;
    }

    public override void SetToLoadInPanel(GameObject toLoadPanel)
    {
        mainCamera.gameObject.SetActive(true);
        currentPanel.GetComponent<CustomMenuBehavior>().Death();
        this.toLoadPanel = toLoadPanel;
    }

    public void TogglePauseMenu()
    {
        pauseGroup.SetActive(!pauseGroup.activeSelf);
        if(pauseGroup.activeSelf)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
