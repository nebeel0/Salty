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
    public List<PlayerController> childPlayers
    {
        get
        {
            List<PlayerController> players = new List<PlayerController>();
            foreach (Transform child in transform)
            {
                if (child.CompareTag("Player"))
                {
                    players.Add(child.gameObject.GetComponent<PlayerController>());
                }
            }
            return players;
        }
    }
    public PlayerController parentPlayer
    {
        get
        {
            if (transform.parent != null && transform.parent.CompareTag("Player"))
            {
                return transform.parent.gameObject.GetComponent<PlayerController>();
            }
            return null;
        }
    }
    public BlockBehavior trackingBlock;
    Vector3 offset;
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
    public float diagonal;
    Vector3 min = Vector3.zero;
    Vector3 max = Vector3.zero;

    Vector3 centerOfMass;

    public void AddBlock(BlockBehavior block)
    {
        if (block.cluster != null && block.cluster != this && block.cluster.blocks.Count > 0 && blocks.Count > 0)
        {
            ClusterBehavior parentCluster = block.cluster.totalMass > totalMass ? block.cluster : this;
            ClusterBehavior childCluster = block.cluster.totalMass <= totalMass ? block.cluster : this;

            parentCluster.trackingBlock.name = "Block";
            childCluster.trackingBlock.name = "Block";

            parentCluster.averageDrag = parentCluster.averageDrag * parentCluster.totalMass + childCluster.averageDrag * childCluster.totalMass;
            parentCluster.averageDrag /= (parentCluster.totalMass + childCluster.totalMass);

            parentCluster.centerOfMass = (parentCluster.transform.position * parentCluster.totalMass) + (childCluster.transform.position * childCluster.totalMass);
            parentCluster.centerOfMass /= (parentCluster.totalMass + childCluster.totalMass);

            parentCluster.totalMass = childCluster.totalMass + parentCluster.totalMass;

            foreach (BlockBehavior childBlock in childCluster.blocks)
            {
                childBlock.cluster = parentCluster;
                if(parentCluster.trackingBlock != childBlock)
                {
                    childBlock.name = "Block";
                }
                parentCluster.blocks.Add(childBlock);
                Physics.IgnoreCollision(childBlock.collider, childCluster.clusterMessageBehavior.messageSphere, false);
                Physics.IgnoreCollision(childBlock.collider, parentCluster.clusterMessageBehavior.messageSphere);
            }
            foreach (PlayerController player in childCluster.childPlayers)
            {
                player.cluster = parentCluster;
                player.ResetParenting();
            }

            if (childCluster.parentPlayer != null)
            {
                childCluster.parentPlayer.cluster = parentCluster;
                childCluster.parentPlayer.ResetParenting();
            }

            parentCluster.UpdateCenter();

            parentCluster.UpdateMax(childCluster.max);
            parentCluster.UpdateMin(childCluster.min);
            parentCluster.UpdateDiagonal();

            childCluster.blocks.Clear();
            childCluster.Death();
        }
    }

    public void RemoveBlock(BlockBehavior block)
    {
        blocks.Remove(block);
        if (!DeathCheck())
        {
            BFSRefresh();
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
        gameMaster.spawnManager.SystemClusters.Remove(this);
        Debug.Log("Remaining Clusters: " + gameMaster.spawnManager.SystemClusters.Count().ToString());
        Destroy(gameObject);
    }


    public void BFSRefresh()
    {
        BlockBehavior currentBlock;
        Queue<BlockBehavior> BlockQueue = new Queue<BlockBehavior>();
        HashSet<BlockBehavior> seenBlocks = new HashSet<BlockBehavior>();

        averageDrag = 0;
        totalMass = 0;
        centerOfMass = Vector3.zero;

        BlockQueue.Enqueue(blocks.First());
        while (BlockQueue.Count != 0)
        {
            currentBlock = BlockQueue.Dequeue();
            if (currentBlock != null && !seenBlocks.Contains(currentBlock) && currentBlock.slotManager.slots != null)
            {
                currentBlock.cluster = this;
                seenBlocks.Add(currentBlock);
                Physics.IgnoreCollision(currentBlock.collider, clusterMessageBehavior.messageSphere);
                Vector3 currentBlockPosition = currentBlock.transform.position;
                totalMass += 1;
                averageDrag += currentBlock.GetComponent<Rigidbody>().drag;
                centerOfMass += currentBlockPosition;

                UpdateMin(currentBlockPosition);
                UpdateMax(currentBlockPosition);

                foreach (SlotBehavior slot in currentBlock.slotManager.slots.Values)
                {
                    if (!seenBlocks.Contains(slot.OccupantBlock) && slot.IsOccupied())
                    {
                        BlockQueue.Enqueue(slot.OccupantBlock);
                    }
                }
            }
        }

        HashSet<BlockBehavior> removedBlocks = new HashSet<BlockBehavior>();
        foreach(BlockBehavior originalBlock in blocks)
        {
            if(!seenBlocks.Contains(originalBlock))
            {
                removedBlocks.Add(originalBlock);
                Physics.IgnoreCollision(originalBlock.collider, clusterMessageBehavior.messageSphere, false);
            }
        }
        if(removedBlocks.Count > 0)
        {
            gameMaster.spawnManager.CreateCluster(removedBlocks);
        }
        blocks = seenBlocks;
        averageDrag = averageDrag / blocks.Count;
        centerOfMass /= totalMass;
        UpdateDiagonal();
        UpdateCenter();
    }
    
    void UpdateMin(Vector3 point)
    {
        min.x = System.Math.Min(min.x, point.x);
        min.y = System.Math.Min(min.y, point.y);
        min.z = System.Math.Min(min.z, point.z);

    }

    void UpdateMax(Vector3 point)
    {
        max.x = System.Math.Max(max.x, point.x);
        max.y = System.Math.Max(max.y, point.y);
        max.z = System.Math.Max(max.z, point.z);
    }

    void UpdateDiagonal()
    {
        diagonal = Vector3.Distance(min, max);
        clusterMessageBehavior.UpdateRadius();
    }

    void UpdateCenter()
    {
        float closestDistance = -1;
        foreach (BlockBehavior currentBlock in blocks)
        {
            float currDistance = Vector3.Distance(centerOfMass, currentBlock.transform.position);
            if (currDistance < closestDistance || closestDistance == -1)
            {
                closestDistance = currDistance;
                trackingBlock = currentBlock;
            }
            else
            {
                currentBlock.name = "Block";
            }
        }
        trackingBlock.name = "Tracking Block";
        offset = centerOfMass - trackingBlock.transform.position;
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        transform.parent = trackingBlock.transform;
        transform.localPosition = offset;
        transform.localEulerAngles = Vector3.zero;
    }

    public void DistributeForce(Vector3 forceVector, ForceMode forceMode)
    {
        Vector3 distributedForce = forceVector / blocks.Count;
        //Vector3 distributedForce = forceVector;
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
