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

    GameRules gameRules;

    ParticleBehavior particleEnv = new ParticleBehavior();
    static public GameObject playerRef;
    static public GameObject superBlockRef;
    static public GameObject blockRef;
    static public GameObject particleRef;
    [ReadOnly]
    public float timeCounter = 0;

    private void OnEnable()
    {
        Start();
    }


    void Start()
    {
        //Add Cage Ref
        SetBasePhysicsRules();
    }

    void Update()
    {
    }
    void SetBasePhysicsRules()
    {
        foreach (KeyValuePair<string, ParticleBehavior.ParticleState> entry in particleEnv.possibleStates)
        {
            foreach (string ignoredCollision in entry.Value.ignoredCollisions)
            {
                Physics.IgnoreLayerCollision(layer1: particleEnv.possibleStates[entry.Key].layer, layer2: particleEnv.possibleStates[ignoredCollision].layer);
            }
        }
        Physics.IgnoreLayerCollision(layer1: particleEnv.blockLayer, layer2: particleEnv.noBlockCollisionLayer);
        Physics.IgnoreLayerCollision(layer1: particleEnv.noBlockCollisionLayer, layer2: particleEnv.noBlockCollisionLayer);
    }
    //Spawn Utils
    static void SpawnParticles(int seed, Vector3 bounds)
    {
        for (int i = 0; i < seed; i++)
        {
            GameObject block = Instantiate(blockRef);
            block.transform.position = RandomBoundedVector3(bounds);
            block.transform.eulerAngles = RandomEulerAngles();
        }

        for (int i = 0; i < seed; i++)
        {
            GameObject lepton = Instantiate(particleRef);
            ParticleBehavior leptonBehavior = lepton.GetComponent<ParticleBehavior>();
            leptonBehavior.particleStateType = "leptonNeg";
            lepton.transform.position = RandomBoundedVector3(bounds);
        }
    }

    //Random Utils
    static Vector3 RandomEulerAngles()
    {
        float x = Random.Range(-90, 90);
        float y = Random.Range(-90, 90);
        float z = Random.Range(-90, 90);
        return new Vector3(x, y, z);
    }
    static Vector3 RandomBoundedVector3(Vector3 dimensions)
    {
        float x = Random.Range(dimensions.x * -1, dimensions.x);
        float y = Random.Range(dimensions.y * -1, dimensions.y);
        float z = Random.Range(dimensions.z * -1, dimensions.z);
        return new Vector3(x, y, z);
    }

}
