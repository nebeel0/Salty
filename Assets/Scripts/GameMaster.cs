using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    Vector3 Min = new Vector3(0, 0, 0);
    Vector3 Max = new Vector3(20, 20, 20);
    ParticleBehavior particleEnv = new ParticleBehavior();
    BlockBehavior blockEnv = new BlockBehavior();
    public int seed = 10;

    void SetPhysicsRules()
    {
        foreach (KeyValuePair<string, ParticleBehavior.ParticleState> entry in particleEnv.possibleStates)
        {
            foreach(string ignoredCollision in entry.Value.ignoredCollisions)
            {
                Physics.IgnoreLayerCollision(layer1: particleEnv.possibleStates[entry.Key].layer, layer2: particleEnv.possibleStates[ignoredCollision].layer);
            }
        }
        Physics.IgnoreLayerCollision(layer1: particleEnv.blockLayer, layer2: particleEnv.noBlockCollisionLayer);
        Physics.IgnoreLayerCollision(layer1: particleEnv.noBlockCollisionLayer, layer2: particleEnv.noBlockCollisionLayer);
    }

    void Start()
    {
        SetPhysicsRules();
        //for(int i = 0; i < seed; i++)
        //{
        //    GameObject superBlock = new GameObject();
        //    superBlock.AddComponent<SuperBlock>();
        //}
        GetComponent<AudioSource>().Play();
    }

    void Update()
    {
        
    }
}
