using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pacman : GameRules
{
    //Players can grow bigger by connecting to other blocks.
    //Largest player becomes pac man, and can no longer grow except by consuming players
    //Other players are incentivized to break apart pac man, or become larger than pac man.
    public PlayerController pacMan;
    public int pacManIndex;

    public override void Start()
    {
    }

    void PacManSetUp()
    {
        pacMan.transform.position = Vector3.zero;
        //TODO make size twice as big, if no block count;
        //TODO change texture of block
        //TODO disable adding of blocks, except for player blocks.
    }

    //Pla
    void PlayersSetUp()
    {
        for(int i = 0; i < players.Count; i++)
        {
            if(i != pacManIndex)
            {

            }
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        if(TransitionCondition())
        {

        }
        if(EndCondition())
        {

        }
    }

    public override void OnPlayerJoined(PlayerInput playerInput)
    {

    }

    public override void OnPlayerLeft(PlayerInput playerInput)
    {
    }

    public override bool TransitionCondition()
    {
        return false;
    }

    public override bool EndCondition()
    {
        return false;
    }

    public override bool SlotMessageCheck(SlotBehavior slot)
    {
        return false;
    }
}
