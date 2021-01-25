using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class StartMenuManager : CustomMenuBehavior
{
    public GameObject menuTrophy;
    public GameObject gameTitle;

    public GameObject CustomButtonRef;
    public GameObject prestart;
    public GameObject start;

    Dictionary<string, GameObject> allOptions;

    Stack<GameObject> previousOptions = new Stack<GameObject>();

    void Start()
    {
        if (allOptions == null)
        {
            allOptions = new Dictionary<string, GameObject>()
            {
                ["start"] = start,
            };
        }
        transform.position = menuManager.transform.position;
        foreach (GameObject gameObject in allOptions.Values)
        {
            if(gameObject.GetComponent<CanvasGroupTransitionBehavior>() == null)
            {
                gameObject.AddComponent<CanvasGroupTransitionBehavior>();
            }
            GameObject backButton = Instantiate(CustomButtonRef, gameObject.transform);
            backButton.name = "back";
            foreach(Transform button in gameObject.transform)
            {
                CustomButtonBehavior buttonBehavior = button.gameObject.GetComponent<CustomButtonBehavior>();
                buttonBehavior.currentMenu = this;
            }
            gameObject.SetActive(false);
        }
        SetToLoadInPanel(prestart);
    }

    public override void Death()
    {
        menuTrophy.GetComponent<CanvasGroup>().ignoreParentGroups = false;
        gameTitle.GetComponent<CanvasGroup>().ignoreParentGroups = false;
        base.Death();
    }

    public override void PressButton(GameObject button)
    {
        string buttonName = button.name.ToLower();
        switch (buttonName)
        {
            case "quit":
                Application.Quit();
                break;
            case "back":
                LoadOutPanel();
                GameObject previousPage = previousOptions.Pop();
                SetToLoadInPanel(previousPage);
                break;
            case "play":
                menuManager.SetToLoadInPanel(menuManager.playMenuRef);
                break;
            default:
                if (allOptions.ContainsKey(buttonName))
                {
                    if (currentPanel != null && currentPanel.name != allOptions[buttonName].name)
                    {
                        LoadOutPanel();
                        previousOptions.Push(currentPanel);
                    }
                    SetToLoadInPanel(allOptions[buttonName]);
                }
                break;
        }
    }



}
