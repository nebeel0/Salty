using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehavior : MonoBehaviour
{
    public GameMaster gameMaster;

    public GameMaster GetGameMaster()
    {
        return GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
    }
}
