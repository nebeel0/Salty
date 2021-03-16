using Matter.Block.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class GhostSubController : SubController
    {
        public void Start()
        {
            GetComponent<SphereCollider>().enabled = true;
        }

        void OnTriggerStay(Collider other)
        {
            if (BlockUtils.IsBlock(other.gameObject) && !Player.IsAI())
            {
                Debug.Log("Colliding for ghost");
                ClusterBehavior cluster = other.gameObject.GetComponent<BlockBehavior>().Cluster;
                if(cluster != null && NoPlayers(cluster))
                {
                    Player.AttachCluster(cluster);
                }

            }
        }

        bool NoPlayers(ClusterBehavior cluster)
        {
            for(int i =0; i < cluster.transform.childCount; i++)
            {
                PlayerControlManager clusterPlayer = cluster.transform.GetChild(i).GetComponent<PlayerControlManager>();
                if (clusterPlayer != null)
                {
                    if(clusterPlayer.IsAI())
                    {
                        Player.gameMaster.spawnManager.DestroyAI(clusterPlayer);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}


