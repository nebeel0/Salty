using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    //Controls input for a playercontroller
    PlayerController Player
    {
        get { return GetComponent<PlayerController>(); }
    }

    public void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Player.Ghost.enabled)
        {
            Player.gameMaster.spawnManager.DestroyAI(Player);
        }
    }
}
