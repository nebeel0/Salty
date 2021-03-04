using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.InputSystem;


public abstract class GameRules : MonoBehaviour
{
    // Base rules are that if one player controls everything, game over.
    // Otherwise its state based
    public GameMaster gameMaster;

    // Examples of states, if all particles are in a collider, or a game object collects a certain number of particles
    public virtual void Start()
    {
        LevelSetup();
        PlayersSetup();
    }

    public virtual void Update()
    {
        if(EndCondition())
        {
            End();
        }
        else if(TransitionCondition())
        {
            Transition();
        }
    }

    public abstract void PlayersSetup(); //Setting up agents

    public abstract void LevelSetup(); //Setting up environment

    public abstract string GetGameDescription();

    public abstract bool TransitionCondition();  //Linking function

    public abstract bool EndCondition();  //Termination function

    public void End()
    {
        gameMaster.menuManager.SetToLoadInPanel(gameMaster.menuManager.endMenuRef);
        gameMaster.menuManager.LoadInPanel();
        EndMenuManager endMenuManager = gameMaster.menuManager.currentPanel.GetComponent<EndMenuManager>();
        endMenuManager.SetGameScore();
        endMenuManager.SetGameMessage();
        gameMaster.ClearGame();
        gameObject.SetActive(false);
    }


    public void Transition()
    {
        //TODO implement this
        gameObject.SetActive(false);
    }
}

