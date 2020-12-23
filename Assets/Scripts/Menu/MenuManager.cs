using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update

    class Page
    {
        string Name;
        public List<GameObject> PageElements;

        public Page(string name)
        {
            this.Name = name;
        }
    }

    Dictionary<string, GameObject> Pages = new Dictionary<string, GameObject>();
    Stack<GameObject> PreviousPages = new Stack<GameObject>();
    GameObject CurrentPage;

    public GameObject MainPageRef;
    public GameObject StartPageRef;

    public GameObject PressAnyKeyRef;

    public GameObject CustomButtonRef;

    public GameObject Overall;
    public GameObject LeftPanelRef;
    public GameObject RightPanelRef;

    public GameObject MenuTrophyRef;
    public GameObject TitleRef;

    GameObject LeftPanel;
    GameObject RightPanel;

    //TODO use WorldSpaceGUITransform to get overall size of object, create a mock to take up space, load it under its subpanel, and then 




    public void Start()
    {
        CurrentPage = null;
        transform.position = Camera.main.transform.position;
        LoadPreStartMenu();
    }

    public void LoadPreStartMenu()
    {
        Instantiate(MenuTrophyRef);
        Instantiate(TitleRef);
        if(LeftPanel == null && RightPanel == null)
        {
            LeftPanel = Instantiate(LeftPanelRef, Overall.transform);
            RightPanel = Instantiate(RightPanelRef, Overall.transform);
        }
        else
        {
            LeftPanel.SetActive(true);
            RightPanel.SetActive(true);
        }

        Instantiate(PressAnyKeyRef, RightPanel.transform);
        SetUpPreStartPages();
    }

    void SetUpPreStartPages()
    {
        Pages["main"] = Instantiate(MainPageRef, RightPanel.transform);
        Pages["start"] = Instantiate(StartPageRef, RightPanel.transform);

        foreach (GameObject gameObject in Pages.Values)
        {
            string objectName = gameObject.name.Replace("(Clone)", "");
            //if (objectName != "Main Page")
            //{
            //    Debug.Log(gameObject.name);
            //    GameObject backButton = Instantiate(CustomButtonRef, gameObject.transform);
            //    backButton.name = "back";
            //}
            gameObject.SetActive(false);
        }
    }

    public void PressButton(string buttonName)
    {
        buttonName = buttonName.ToLower();

        if (buttonName == "quit")
        {
            Application.Quit();
        }

        //Two cases of redirecting to another page
        if (Pages.ContainsKey(buttonName))
        {
            if (CurrentPage != null)
            {
                CurrentPage.SetActive(false);
                PreviousPages.Push(CurrentPage);
            }
            Pages[buttonName].SetActive(true);
            CurrentPage = Pages[buttonName];
        }

        if (buttonName == "back")
        {
            LoadBackPage();
        }
    }

    void LoadBackPage()
    {
        CurrentPage.SetActive(false);
        GameObject previousPage = PreviousPages.Pop();
        previousPage.SetActive(true);
        CurrentPage = previousPage;
    }

    void LoadStartMenu()
    {
        LeftPanel.SetActive(false);
        RightPanel.SetActive(false);

        Pages["start"].SetActive(true);
        CurrentPage.SetActive(false);
    }



}
