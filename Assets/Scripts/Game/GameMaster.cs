using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class GameMaster : MonoBehaviour
{
    //Example You must survive till the end of the waves
    //Example You must find the wrong block
    //Example be the last block standing on the platform
    //Example climb to the top

    //TODO ensure conservation of mass, energy
    //This also means kinetic energy cannot be converted to energy/mass
    //or maybe not

    private void Start()
    {
        transform.position = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
        transform.localScale = Vector3.one;
    }

    public GameRules currentGameRules
    {
        get { return GetComponent<GameRules>(); }
    }

    public static List<Type> possibleRules = new List<Type>() 
    { 
        typeof(Pacman),
        typeof(Story)
    }; //TODO to turn this into a json that can be created and pulled dynamically

    public MenuManager menuManager;
    public PhysicsManager physicsManager;
    public SpawnManager spawnManager;
    public NetworkManager networkManager;
    public CageBehavior cage;

    //Mass Rules Check

    public void SetRules(Type gameRules)
    {
        if(currentGameRules != null)
        {
            Destroy(currentGameRules);
        }
        spawnManager.players.Remove(null);
        gameObject.AddComponent(gameRules);
        currentGameRules.gameMaster = this;
        if(menuManager.mainCamera.gameObject.activeSelf)
        {
            menuManager.mainCamera.gameObject.SetActive(false);
        }
    }

    public void ClearGame()
    {
        spawnManager.DestroyEverything();
        Destroy(currentGameRules);
    }

    public void ResetGame()
    {
        Type currentRules = currentGameRules.GetType();
        ClearGame();
        SetRules(currentRules);
    }

}
