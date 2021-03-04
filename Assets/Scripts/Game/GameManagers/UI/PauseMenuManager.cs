using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : CustomMenuBehavior
{
    public GameObject resume;
    public GameObject exitGame;

    public override void Death()
    {
        base.Death();
    }

    public override void PressButton(GameObject button)
    {
        if(button == resume)
        {
            menuManager.TogglePauseMenu();
        }
        else if(button == exitGame)
        {
            menuManager.TogglePauseMenu();
            menuManager.gameMaster.ClearGame();
            menuManager.SetToLoadInPanel(menuManager.playMenuRef);
        }
    }

}
