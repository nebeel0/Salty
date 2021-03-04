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
        public void OnGhostMode()
        {
            //TODO implement
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Block"))
            {
                //Debug.Log("Colliding for ghost");
                ClusterBehavior cluster = other.gameObject.GetComponent<BlockBehavior>().cluster;
                if(NoPlayers(cluster))
                {
                    Player.AttachPlayer(cluster);
                }

            }
        }

        bool NoPlayers(ClusterBehavior cluster)
        {
            for(int i =0; i < cluster.transform.childCount; i++)
            {
                if(cluster.transform.GetChild(i).GetComponent<PlayerControlManager>() != null)
                {
                    return false;
                }
            }
            return true;
        }

    }
}


