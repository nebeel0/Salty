using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class EndMenuManager : CustomMenuBehavior
{
    public CustomTextBehavior GameMessage;
    public CustomTextBehavior GameScore;
    public GameObject playAgain;
    public GameObject backToMenu;

    public override void Death()
    {
        base.Death();
    }

    public void SetGameMessage()
    {
        //TODO implement logic to get game ending messages
        GameMessage.SetText("Wow the game was cool");
    }

    public void SetGameScore()
    {
        //TODO implement logic to get the current score, and the top scores
        GameScore.SetText("Score: 10");
    }

    public void LoadGameScores()
    {
        //Pull up the current game type, and load the scores corresponding to it in json
        //show the current ranking
    }

    public override void PressButton(GameObject button)
    {
        if (button == playAgain)
        {
            Debug.Log("Gotta implement play again.");
        }
        else if (button == backToMenu)
        {
            menuManager.gameMaster.ClearGame();
            menuManager.SetToLoadInPanel(menuManager.playMenuRef);
        }
    }

}
