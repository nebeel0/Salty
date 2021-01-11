using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

//Aggregates blocks
public class ClusterBehavior : GameBehavior
{
    ClusterMessageBehavior clusterMessageBehavior
    {
        get { return GetComponent<ClusterMessageBehavior>(); }
    }

    public HashSet<BlockBehavior> blocks = new HashSet<BlockBehavior>();
    public List<GameObject> childPlayers
    {
        get
        {
            List<GameObject> players = new List<GameObject>();
            foreach (Transform child in transform)
            {
                if (child.CompareTag("Player"))
                {
                    players.Add(child.gameObject);
                }
            }
            return players;
        }
    }
    public GameObject parentPlayer
    {
        get
        {
            if (transform.parent != null && transform.parent.CompareTag("Player"))
            {
                return transform.parent.gameObject;
            }
            return null;
        }
    }
    public BlockBehavior trackingBlock;

    public bool IsOccupying()
    {
        foreach (BlockBehavior block in blocks)
        {
            if (block.slotManager.IsOccupying())
            {
                return true;
            }
        }
        return false;
    }

    public float totalMass; //We're going to treat each block as having the same mass.
    public float averageDrag; //We're going to treat each block as having the same mass.
    float displacementFactor = 1.2f;
    float diagonal;
    Vector3 centerOfMass;

    public void AddBlock(BlockBehavior block)
    {
        if (block.cluster != null && block.cluster != this)
        {
            ClusterBehavior parentCluster = block.cluster.blocks.Count > blocks.Count ? block.cluster : this;
            ClusterBehavior childCluster = block.cluster.blocks.Count <= blocks.Count ? block.cluster : this;

            foreach (Transform child in childCluster.gameObject.transform)
            {
                child.parent = parentCluster.transform;
            }
            if (childCluster.parentPlayer != null)
            {
                childCluster.parentPlayer.transform.parent = parentCluster.transform;
            }
            parentCluster.UpdateCenter();
            childCluster.Death();
        }
    }

    public void RemoveBlock(BlockBehavior block)
    {
        blocks.Remove(block);
        if (!DeathCheck())
        {
            UpdateCenter();
        }
        else
        {
            Death();
        }
    }

    bool DeathCheck()
    {
        if (blocks.Count == 0)
        {
            return true;
        }
        return false;
    }

    void Death()
    {
        transform.DetachChildren();
        gameMaster.SystemClusters.Remove(this);
        Destroy(gameObject);
    }


    public void UpdateCenter()
    {
        SlotManagerBehavior currentBlockSlotManagerBehavior;
        Queue<SlotManagerBehavior> BlockSlotManagerBehaviorQueue = new Queue<SlotManagerBehavior>();
        HashSet<BlockBehavior> seenBlocks = new HashSet<BlockBehavior>();

        averageDrag = 0;
        totalMass = 0;
        centerOfMass = Vector3.zero;
        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;
        Vector3 trackingBlockCenterOffset = Vector3.zero;
        float closestDistance = -1;

        BlockSlotManagerBehaviorQueue.Enqueue(blocks.First().slotManager);
        while (BlockSlotManagerBehaviorQueue.Count != 0)
        {
            currentBlockSlotManagerBehavior = BlockSlotManagerBehaviorQueue.Dequeue();
            if (currentBlockSlotManagerBehavior != null && !seenBlocks.Contains(currentBlockSlotManagerBehavior.block) && currentBlockSlotManagerBehavior.slots != null)
            {
                BlockBehavior currentBlock = currentBlockSlotManagerBehavior.block;
                currentBlock.cluster = this;
                seenBlocks.Add(currentBlock);
                Vector3 currentBlockPosition = currentBlock.transform.position;
                totalMass += 1;
                averageDrag += currentBlock.GetComponent<Rigidbody>().drag;
                centerOfMass += currentBlockPosition;

                min.x = System.Math.Min(min.x, currentBlockPosition.x);
                min.y = System.Math.Min(min.y, currentBlockPosition.y);
                min.z = System.Math.Min(min.z, currentBlockPosition.z);

                max.x = System.Math.Max(max.x, currentBlockPosition.x);
                max.y = System.Math.Max(max.y, currentBlockPosition.y);
                max.z = System.Math.Max(max.z, currentBlockPosition.z);

                Vector3 tmpCenterOfMass = centerOfMass / totalMass;

                float currDistance = Vector3.Distance(tmpCenterOfMass, currentBlockPosition);
                if (currDistance < closestDistance || closestDistance == -1)
                {
                    closestDistance = currDistance;
                    trackingBlock = currentBlock;
                }


                foreach (SlotBehavior slot in currentBlockSlotManagerBehavior.slots.Values)
                {
                    if (!seenBlocks.Contains(slot.OccupantBlock) && slot.IsOccupied())
                    {
                        BlockSlotManagerBehaviorQueue.Enqueue(slot.OccupantBlock.GetComponent<BlockBehavior>().slotManager);
                    }
                }
            }
        }

        diagonal = Vector3.Distance(min, max) * displacementFactor;

        HashSet<BlockBehavior> removedBlocks = new HashSet<BlockBehavior>();
        foreach(BlockBehavior originalBlock in blocks)
        {
            if(!seenBlocks.Contains(originalBlock))
            {
                removedBlocks.Add(originalBlock);
            }
        }
        if(removedBlocks.Count > 0)
        {
            gameMaster.CreateCluster(removedBlocks);
        }

        blocks = seenBlocks;
        averageDrag = averageDrag / blocks.Count;

        centerOfMass /= totalMass;
        trackingBlockCenterOffset = centerOfMass - trackingBlock.transform.position;
        transform.parent = trackingBlock.transform;
        transform.localPosition = trackingBlockCenterOffset;
        clusterMessageBehavior.UpdateRadius();
    }

    public void DistributeForce(Vector3 forceVector, ForceMode forceMode)
    {
        Vector3 distributedForce = forceVector / blocks.Count;
        foreach (BlockBehavior block in blocks)
        {
            block.GetComponent<Rigidbody>().AddForce(distributedForce, forceMode);
        }
    }

    public void Brake()
    {
        foreach(BlockBehavior block in blocks)
        {
            block.GetComponent<Rigidbody>().velocity = Vector3.zero;
            block.GetComponent<Rigidbody>().freezeRotation = true;
        }
    }
    public void UnBrake()
    {
        foreach (BlockBehavior block in blocks)
        {
            block.GetComponent<Rigidbody>().freezeRotation = false;
        }
    }
}
