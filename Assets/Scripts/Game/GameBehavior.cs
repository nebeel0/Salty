using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehavior : MonoBehaviour
{
    public GameMaster gameMaster;

    public virtual void Start()
    {
        if(gameMaster == null)
        {
            gameMaster = GetGameMaster();
        }    
    }

    public GameMaster GetGameMaster()
    {
        return GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
    }
}
