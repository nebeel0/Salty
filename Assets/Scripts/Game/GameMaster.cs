using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.InputSystem;


public class GameMaster : MonoBehaviour
{
    //Example You must survive till the end of the waves
    //Example You must find the wrong block
    //Example be the last block standing on the platform
    //Example climb to the top

    //TODO ensure conservation of mass, energy
    //This also means kinetic energy cannot be converted to energy/mass
    //or maybe not
    public GameRules gameRules;

    public static List<Type> possibleRules = new List<Type>() 
    { 
        typeof(Pacman)
    }; //TODO to turn this into a json that can be created and pulled dynamically

    public MenuManager menuManager;
    public PhysicsManager physicsManager;
    public SpawnManager spawnManager;
    public NetworkManager networkManager;
    public CageBehavior cage;

    //Mass Rules Check







}
