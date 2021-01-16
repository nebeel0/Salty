using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    public GameMaster gameMaster;

    // Start is called before the first frame update
    static public int messageColliderLayer = 17;


    void Start()
    {
        SetBasePhysicsRules();
    }

    void SetBasePhysicsRules()
    {
        foreach (KeyValuePair<string, ParticleType> entry in ParticleUtils.possibleTypes)
        {
            foreach (string ignoredCollision in entry.Value.ignoredCollisions)
            {
                Physics.IgnoreLayerCollision(layer1: ParticleUtils.possibleTypes[entry.Key].layer, layer2: ParticleUtils.possibleTypes[ignoredCollision].layer);
            }
        }
        Physics.IgnoreLayerCollision(layer1: messageColliderLayer, layer2: messageColliderLayer);
        Physics.IgnoreLayerCollision(layer1: ParticleUtils.blockLayer, layer2: ParticleUtils.noBlockCollisionLayer);
        Physics.IgnoreLayerCollision(layer1: ParticleUtils.noBlockCollisionLayer, layer2: ParticleUtils.noBlockCollisionLayer);
        Physics.IgnoreLayerCollision(layer1: ParticleUtils.noBlockCollisionLayer, layer2: ParticleUtils.noBlockCollisionLayer);
    }

    public float TotalSystemMass()
    {
        float totalSystemMass = 0;
        foreach (ClusterBehavior cluster in gameMaster.spawnManager.SystemClusters)
        {
            totalSystemMass += cluster.totalMass;
        }
        return totalSystemMass;
    }



}
