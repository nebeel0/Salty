using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Story : GameRules
{
    //Basically an overall container for smaller game rules, that are dependent on the saved state.

    public override string GetGameDescription()
    {
        return "An adventure.";
    }

    public override void Start()
    {
        //TODO load saved data
        PlayersSetup();
        LevelSetup();
    }

    public override void Update()
    {
    }

    public override void PlayersSetup()
    {
        //TODO sets up the players based off where the initial place is on the map
    }

    public override void LevelSetup()
    {
        //TODO pull the level setup from the loaded state
    }

    public override bool TransitionCondition()
    {
        return false;
    }

    public override bool EndCondition()
    {
        return false;
    }
}
