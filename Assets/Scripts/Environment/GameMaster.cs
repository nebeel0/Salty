using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    ParticleBehavior particleEnv = new ParticleBehavior();
    public int seed = 1;
    public GameObject playerRef;
    public GameObject superBlockRef;
    public GameObject blockRef;

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
        for (int i = 0; i < seed; i++)
        {
            GameObject block = Instantiate(blockRef);
            block.transform.position = RandomBoundedVector3(new Vector3(50,50,50));
            block.transform.eulerAngles = RandomEulerAngles();
        }
        GetComponent<AudioSource>().Play();
        SpawnPlayer(Vector3.zero);
    }

    void Update()
    {
        
    }

    Vector3 RandomEulerAngles()
    {
        float x = Random.Range(-90, 90);
        float y = Random.Range(-90, 90);
        float z = Random.Range(-90, 90);
        return new Vector3(x, y, z);
    }

    Vector3 RandomBoundedVector3(Vector3 dimensions)
    {
        float x = Random.Range(dimensions.x * -1, dimensions.x);
        float y = Random.Range(dimensions.y * -1, dimensions.y);
        float z = Random.Range(dimensions.z * -1, dimensions.z);
        return new Vector3(x, y, z);
    }

    GameObject SpawnPlayer(Vector3 position)
    {
        GameObject superBlock = Instantiate(superBlockRef);
        GameObject player = Instantiate(playerRef, superBlock.transform);
        player.GetComponent<PlayerController>().superBlock = superBlock;
        superBlock.GetComponent<SuperBlockBehavior>().player = player;
        superBlock.transform.position = position;
        return superBlock;
    }

}
