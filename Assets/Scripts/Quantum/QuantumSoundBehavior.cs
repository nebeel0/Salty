using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantumSoundBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip loadingSound;
    public AudioClip lockingSound;
    public AudioClip flyingSound;
    public AudioClip bouncingSound;

    public AudioSource audio;

    void Start()
    {
    }

    void PlayCollisionSound()
    {
        audio.PlayOneShot(bouncingSound, 0.25f);
    }

}
