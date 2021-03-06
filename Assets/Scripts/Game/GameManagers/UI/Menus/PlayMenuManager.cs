using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PlayMenuManager : CustomMenuBehavior
{
    public GameObject TabRef;
    public GameObject CustomTextRef;

    public GameObject ParentTabSystem;
    public GameObject ChildTabSystem;
    public GameObject Lobby;
    public GameObject Players;
    public GameObject SocialBar;
    public GameObject Initialize;

    Dictionary<TabBehavior, List<TabBehavior>> ParentTabs = new Dictionary<TabBehavior, List<TabBehavior>>(); //Initialized at Runtime at LoadPlayMenu
    Dictionary<TabBehavior, Type> ChildTabs = new Dictionary<TabBehavior, Type>(); //TODO Dictionary for child tab to GameObject menu to load
    TabBehavior CurrentParentTab;
    TabBehavior CurrentChildTab;

    SpawnManager spawnManager
    {
        get { return menuManager.gameMaster.spawnManager; }
    }

    public override void Start()
    {
        Debug.Log(menuManager.ToString());
        spawnManager.ClearPlayers();
        SetUpChildTabs();
        SetUpParentTabs();

        CurrentParentTab = ParentTabs.Keys.ToList()[0];
        CurrentParentTab.GetComponent<TabBehavior>().Select();

        spawnManager.playMenuManager = this;
        spawnManager.playerInputManager.EnableJoining();
    }

    protected override void Update()
    {
        foreach(Transform child in Players.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (PlayerControlManager player in spawnManager.players)
        {
            CustomTextBehavior playerText = Instantiate(CustomTextRef, Players.transform).GetComponent<CustomTextBehavior>();
            playerText.SetText("Player " + spawnManager.players.Count.ToString());
        }
    }

    //public void AddPlayer(ClusterController player)
    //{
    //    players[player] = Instantiate(CustomTextRef, Players.transform).GetComponent<CustomTextBehavior>();
    //    players[player.SetText("Player " + players.Count.ToString());
    //}

    //public void RemovePlayer(ClusterController player)
    //{
    //    Destroy(players[player].gameObject);
    //    players.Remove(player);
    //}

    GameObject InstantiateTab(string name, Transform parent = null) //TODO handle enums for type of button
    {
        GameObject button = Instantiate(TabRef, parent);
        button.GetComponent<TabBehavior>().currentMenu = this;
        button.name = name;
        return button;
    }

    void SetUpParentTabs()
    {
        for (int networkTypeI = 0; networkTypeI < NetworkManager.supportedNetworkTypes.Count; networkTypeI++)
        {
            TabBehavior parentButton = InstantiateTab(NetworkManager.supportedNetworkTypes[networkTypeI], ParentTabSystem.transform).GetComponent<TabBehavior>();
            parentButton.Start();
            parentButton.SetTextSize(48);
            ParentTabs[parentButton] = new List<TabBehavior>();
            foreach(KeyValuePair<TabBehavior, Type> childTabPair in ChildTabs)
            {
                //TODO check Type to see if the game type supports this parent Tab type
                ParentTabs[parentButton].Add(childTabPair.Key);
            }
        }
    }

    void SetUpChildTabs() //Keeps a set of child tabs
    {
        for (int i = 0; i < GameMaster.possibleRules.Count; i++)
        {
            Type possibleGameType = GameMaster.possibleRules[i];
            TabBehavior possibleGameButton = InstantiateTab(possibleGameType.Name, ChildTabSystem.transform).GetComponent<TabBehavior>();
            possibleGameButton.Start();
            possibleGameButton.SetTextSize(24);
            possibleGameButton.gameObject.SetActive(false);
            ChildTabs[possibleGameButton] = possibleGameType;        }
    }

    public override void PressButton(GameObject tab)
    {
        if (tab.transform.parent == ParentTabSystem.gameObject.transform)
        {
            ParentTabRefresh(tab);
        }
        else if (tab.transform.parent == ChildTabSystem.gameObject.transform)
        {
            ChildTabRefresh(tab);
        }
        else if (tab == Initialize)
        {
            //TODO go to gameMaster and start game.
            if(spawnManager.players.Count > 0)
            {
                menuManager.gameMaster.SetRules(ChildTabs[CurrentChildTab]);
                Death();
            }
            else
            {
                Debug.Log("Can't start game till there is one player."); //TODO make a small popup message that disappears on its own, maybe a fading text.
            }
        }
        else
        {
            Debug.LogError("Tab doesn't belong to parent or child.");
        }
    }

    public override void Death()
    {
        spawnManager.playMenuManager = null;
        spawnManager.playerInputManager.DisableJoining();
        base.Death();
    }

    public void ParentTabRefresh(GameObject button)
    {
        TabBehavior tab = button.GetComponent<TabBehavior>();
        for (int i = 0; i < ParentTabSystem.transform.childCount; i++)
        {
            TabBehavior parentTab = ParentTabSystem.transform.GetChild(i).gameObject.GetComponent<TabBehavior>();
            if (parentTab != tab)
            {
                parentTab.DeSelect();
            }
        }

        CurrentParentTab = tab;
        CurrentChildTab = ParentTabs[CurrentParentTab][0];
        ReloadChildTabs();
        CurrentChildTab.Select();

        if(tab.name.ToLower() == NetworkManager.Online)
        {
            menuManager.gameMaster.networkManager.currentNetworkType = NetworkManager.Online;
            SocialBar.SetActive(true);
        }
        else
        {
            menuManager.gameMaster.networkManager.currentNetworkType = NetworkManager.Local;
            SocialBar.SetActive(false);
        }
    }

    public void ChildTabRefresh(GameObject button)
    {
        TabBehavior tab = button.GetComponent<TabBehavior>();
        for (int i = 0; i < ChildTabSystem.transform.childCount; i++)
        {
            TabBehavior childTab = ChildTabSystem.transform.GetChild(i).gameObject.GetComponent<TabBehavior>();
            if (childTab != tab)
            {
                childTab.DeSelect();
            }
        }
        CurrentChildTab = tab;
    }

    public void ReloadChildTabs()
    {
        for (int i = 0; i < ChildTabSystem.transform.childCount; i++)
        {
            TabBehavior child = ChildTabSystem.transform.GetChild(i).gameObject.GetComponent<TabBehavior>();
            if (!ParentTabs[CurrentParentTab].Contains(child))
            {
                child.gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < ParentTabs[CurrentParentTab].Count; i++)
        {
            if (!ParentTabs[CurrentParentTab][i].gameObject.activeSelf)
            {
                ParentTabs[CurrentParentTab][i].gameObject.SetActive(true);
            }
        }
    }
}
