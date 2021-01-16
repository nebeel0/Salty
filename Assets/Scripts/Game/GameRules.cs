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
    public List<PlayerController> players;

    // Examples of states, if all particles are in a collider, or a game object collects a certain number of particles
    public abstract void Start();

    public abstract void Update();

    public abstract void OnPlayerJoined(PlayerInput playerInput);

    public abstract void OnPlayerLeft(PlayerInput playerInput);

    public abstract bool TransitionCondition();

    public abstract bool EndCondition();

    public virtual bool SlotMessageCheck(SlotBehavior slot)
    {
        return true;
    }

    public bool ClusterMessageCheck(ClusterBehavior cluster) //Todo Move to GameRules
    {
        return cluster.totalMass >= 0.25f * gameMaster.physicsManager.TotalSystemMass() && cluster.totalMass >= 16; //TODO fix arbitrary number, squares?
    }

}

