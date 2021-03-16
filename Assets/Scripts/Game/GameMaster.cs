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

    public GameRules.Base.GameRules CurrentGameRules
    {
        get { return GetComponent<GameRules.Base.GameRules>(); }
    }

    public static List<Type> possibleRules = ReflectionUtils.GetSubClasses(typeof(GameRules.Base.GameRules));

    public MenuManager menuManager;
    public PhysicsManager physicsManager;
    public SpawnManager spawnManager;
    public NetworkManager networkManager;
    public CageBehavior cage;

    public void SetRules(Type gameRules)
    {
        if(CurrentGameRules != null)
        {
            Destroy(CurrentGameRules);
        }
        spawnManager.players.Remove(null);
        gameObject.AddComponent(gameRules);
        CurrentGameRules.gameMaster = this;
        if(menuManager.mainCamera.gameObject.activeSelf)
        {
            menuManager.mainCamera.gameObject.SetActive(false);
        }
    }

    public void ClearGame()
    {
        spawnManager.DestroyEverything();
        Destroy(CurrentGameRules);
    }

    public void ResetGame()
    {
        Type currentRules = CurrentGameRules.GetType();
        ClearGame();
        SetRules(currentRules);
    }
}
