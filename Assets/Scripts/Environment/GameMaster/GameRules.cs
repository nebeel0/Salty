using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.InputSystem;


class GameRules : MonoBehaviour
{
    // Base rules are that if one player controls everything, game over.
    // Otherwise its state based

    // Examples of states, if all particles are in a collider, or a game object collects a certain number of particles
    public void Start()
    {
        //Load starting subenvironment
        //Load players
        //Number of Particles/Blocks,cage, time limit, depends on how many players.

        gameObject.GetComponent<AudioSource>().Play();
    }

    public void Update()
    {
        //Load starting subenvironment
        //Load players
        //
    }

    void StartCondition()
    {
        //Don't start the game till the start condition is satistified.
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {

    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        //Default do nothing when players leave, besides removing them from the count
    }

    void WinCondition()
    {

    }

    void TransitionCondition()
    {

    }

    void EndCondition()
    {

    }
}

