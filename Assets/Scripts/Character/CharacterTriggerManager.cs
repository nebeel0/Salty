using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Unity;

public class CharacterTriggerManager: CharacterManagerBehavior
{
    //Triggers
    public bool SlotMessageCheck(SlotBehavior slot)
    {
        return true;
    }

    public void ClusterMessageTrigger(Collider other) //Todo Move to GameRules
    {
        if (other.CompareTag("Block"))
        {
            BlockBehavior otherBlock = other.gameObject.GetComponent<BlockBehavior>();
            if(otherBlock != null && Cluster != null)
            {
                if (!otherBlock.cluster.IsOccupying() && !Cluster.IsOccupying() && otherBlock.cluster != Cluster)
                {
                    Vector3Utils.Attract(other: other, transform: transform);
                }
            }
        }
    }

    public void ClusterCollisionTrigger(Collider other) //Todo Move to GameRules
    {
        if (other.CompareTag("Block"))
        {
            BlockBehavior otherBlock = other.gameObject.GetComponent<BlockBehavior>();
            if (!otherBlock.cluster.IsOccupying() && !Cluster.IsOccupying() && otherBlock.cluster != Cluster)
            {
                Vector3Utils.Attract(other: other, transform: transform);
            }
        }
    }

}
