using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class MenuManager : MonoBehaviour
{
    public GameObject CustomButtonRef;
    public GameObject CenteredHorizontalButtonRef;
    public GameObject Overall;
    List<GameObject> CurrentElements = new List<GameObject>();

    //Start Menu Assets
    GameObject LeftPanel;
    GameObject RightPanel;
    Dictionary<string, GameObject> StartPages = new Dictionary<string, GameObject>();
    Stack<GameObject> PreviousStartPages = new Stack<GameObject>();
    GameObject CurrentStartPage;
    [ReadOnly] [HideInInspector] public string StartPageKey = "start";

    public GameObject LeftPanelRef;
    public GameObject RightPanelRef;
    public GameObject MenuTrophyRef;
    public GameObject TitleRef;
    public GameObject StartPageRef;
    public GameObject PlayMenuRef;
    public GameObject PressAnyKeyRef;

    //Play Menu Assets
    GameObject ParentTabSystem;
    GameObject ChildTabSystem;
    GameObject Lobby;
    GameObject SocialBar;

    static string PlayOnline = "play online";
    static string PlayLocal = "play local";


    Dictionary<string, List<string>> InitParentTabs = new Dictionary<string, List<string>>  //TODO store this in json, if players want to be able to edit the menu themselves
    {
        [PlayLocal] = new List<string> {"boss fight", "free for all", "teams", "journey"},
        [PlayOnline] = new List<string> { "boss fight", "free for all", "teams", "poop"},
    };
    Dictionary<GameObject, List<GameObject>> ParentTabs = new Dictionary<GameObject, List<GameObject>>(); //Initialized at Runtime at LoadPlayMenu
    Dictionary<string, GameObject> ChildTabs = new Dictionary<string, GameObject>(); //TODO Dictionary for child tab to GameObject menu to load
    GameObject CurrentParentTab;
    GameObject CurrentChildTab;


    //TODO use WorldSpaceGUITransform to get overall size of object, create a mock to take up space, load it under its subpanel, for Start Menu so that 
    bool InStartMenu
    {
        get { return LeftPanel != null && RightPanel != null; }
    }

    bool InPlayMenu
    {
        get { return ParentTabSystem != null && ChildTabSystem != null; }
    }

    public void Start()
    {
        transform.position = Camera.main.transform.position;
        SetUpStartMenu();
    }

    public void Update()
    {
    }
    public void SetUpStartMenu()
    {
        CurrentElements.Add(Instantiate(MenuTrophyRef));
        CurrentElements.Add(Instantiate(TitleRef));

        LeftPanel = Instantiate(LeftPanelRef, Overall.transform);
        RightPanel = Instantiate(RightPanelRef, Overall.transform);

        CurrentElements.Add(LeftPanel);
        CurrentElements.Add(RightPanel);

        CurrentElements.Add(Instantiate(PressAnyKeyRef, RightPanel.transform));

        StartPages[StartPageKey] = Instantiate(StartPageRef, RightPanel.transform);

        foreach (GameObject gameObject in StartPages.Values)
        {
            string objectName = gameObject.name.Replace("(Clone)", "");
            if (objectName != "Start Page")
            {
                Debug.Log(gameObject.name);
                GameObject backButton = Instantiate(CustomButtonRef, gameObject.transform);
                backButton.name = "back";
            }
            gameObject.SetActive(false);
        }
    }

    public void PressButton(string buttonName, GameObject button=null)
    {
        buttonName = buttonName.ToLower();
        Debug.Log("InStartMenu: " + InStartMenu.ToString());
        Debug.Log("InPlayMenu: " + InPlayMenu.ToString());
        if (InStartMenu)
        {
            PressButton_Start(buttonName);
        }
        else
        {
            Debug.LogError("Some button is causing issues, since its not for play or start.");
        }
    }

    void PressButton_Start(string buttonName)
    {
        switch (buttonName)
        {
            case "quit":
                Application.Quit();
                break;
            case "back":
                CurrentStartPage.SetActive(false);
                GameObject previousPage = PreviousStartPages.Pop();
                previousPage.SetActive(true);
                CurrentStartPage = previousPage;
                break;
            case "play":
                ClearStartScreen();
                LoadPlayMenu();
                break;
            default:
                if (StartPages.ContainsKey(buttonName))
                {
                    if (CurrentStartPage != null && CurrentStartPage.name != StartPages[buttonName].name)
                    {
                        CurrentStartPage.SetActive(false);
                        PreviousStartPages.Push(CurrentStartPage);
                    }
                    StartPages[buttonName].SetActive(true);
                    CurrentStartPage = StartPages[buttonName];
                }
                break;
        }
    }

    void ClearStartScreen()
    {
        //For each gameObject add component Transition
        for(int i = 0; i < CurrentElements.Count; i++)
        {
            Destroy(CurrentElements[i]); //TODO setup transition outs, and when they disappear destroy themselves.
        }
        foreach (GameObject gameObject in StartPages.Values)
        {
            Destroy(gameObject);
        }
        StartPages.Clear();
        PreviousStartPages.Clear();
        CurrentStartPage = null;
    }

    void LoadPlayMenu()
    {
        CurrentElements.Add(Instantiate(PlayMenuRef, Overall.transform));
        ParentTabSystem = GameObject.FindGameObjectWithTag("Parent Tab System");
        ChildTabSystem = GameObject.FindGameObjectWithTag("Child Tab System");
        Lobby = GameObject.FindGameObjectWithTag("Lobby");
        SocialBar = GameObject.FindGameObjectWithTag("Social Bar");

        SetUpChildTabs();
        SetUpParentTabs();

        CurrentParentTab = ParentTabs.Keys.ToList()[0];
        CurrentParentTab.GetComponent<TabBehavior>().Select();
    }

    void SetUpParentTabs()
    {
        foreach (KeyValuePair<string, List<string>> parentTab in InitParentTabs)
        {
            GameObject parentButton = InstantiateButton(parentTab.Key, ParentTabSystem.transform);
            parentButton.GetComponent<TabBehavior>().Start();
            parentButton.GetComponent<TabBehavior>().SetTextSize(48);
            ParentTabs[parentButton] = new List<GameObject>();
            foreach (string childTab in parentTab.Value)
            {
                ParentTabs[parentButton].Add(ChildTabs[childTab]);
            }
        }
    }

    void SetUpChildTabs() //Keeps a set of child tabs
    {
        List<string> childTabs_String = new List<string>();
        foreach (List<string> childTab in InitParentTabs.Values)
        {
            childTabs_String = childTabs_String.Concat(childTab).ToList();
        }
        childTabs_String = Enumerable.Distinct(childTabs_String).ToList();

        for (int i = 0; i < childTabs_String.Count; i++)
        {
            ChildTabs[childTabs_String[i]] = InstantiateButton(childTabs_String[i], ChildTabSystem.transform);
            ChildTabs[childTabs_String[i]].GetComponent<TabBehavior>().Start();
            ChildTabs[childTabs_String[i]].GetComponent<TabBehavior>().SetTextSize(24);
            ChildTabs[childTabs_String[i]].SetActive(false);
        }
    }

    public void TabRefresh(GameObject tab)
    {
        Debug.Log(tab.name);
        if(tab.transform.parent == ParentTabSystem.gameObject.transform)
        {
            ParentTabRefresh(tab);
        }
        else if(tab.transform.parent == ChildTabSystem.gameObject.transform)
        {
            ChildTabRefresh(tab);
        }
        else
        {
            Debug.LogError("Tab doesn't belong to parent or child.");
        }
    }    

    public void ParentTabRefresh(GameObject tab)
    {
        if(tab.name == PlayOnline)
        {
            SocialBar.SetActive(true);
        }
        else
        {
            SocialBar.SetActive(false);
        }
        for (int i = 0; i < ParentTabSystem.transform.childCount; i++)
        {
            GameObject parentTab = ParentTabSystem.transform.GetChild(i).gameObject;
            if (parentTab != tab)
            {
                parentTab.GetComponent<TabBehavior>().DeSelect();
            }
        }

        CurrentParentTab = tab;
        CurrentChildTab = ParentTabs[CurrentParentTab][0];
        ReloadChildTabs();
        CurrentChildTab.GetComponent<TabBehavior>().Select();
    }

    public void ChildTabRefresh(GameObject tab)
    {
        for (int i = 0; i < ChildTabSystem.transform.childCount; i++)
        {
            GameObject childTab = ChildTabSystem.transform.GetChild(i).gameObject;
            if (childTab != tab)
            {
                childTab.GetComponent<TabBehavior>().DeSelect();
            }
        }
        CurrentChildTab = tab;
    }

    public void ReloadChildTabs()
    {
        for (int i = 0; i < ChildTabSystem.transform.childCount; i++)
        {
            GameObject child = ChildTabSystem.transform.GetChild(i).gameObject;
            if (!ParentTabs[CurrentParentTab].Contains(child))
            {
                child.SetActive(false);
            }
        }

        for (int i = 0; i < ParentTabs[CurrentParentTab].Count; i++)
        {
            if (!ParentTabs[CurrentParentTab][i].activeSelf)
            {
                ParentTabs[CurrentParentTab][i].SetActive(true);
            }
        }
    }

    GameObject InstantiateButton(string name, Transform parent=null) //TODO handle enums for type of button
    {
        GameObject button = Instantiate(CenteredHorizontalButtonRef, parent);
        button.name = name;
        return button;
    }

}
