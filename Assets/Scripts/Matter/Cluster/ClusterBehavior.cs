using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

//Aggregates blocks
public class ClusterBehavior : GameBehavior
{
    public bool gravityEnabled = false;
    public SphereCollider GravitySphere;
    public void UpdateGravitySphereRadius()
    {
        bool largeRelativeMass = totalMass >= 0.25f * gameMaster.physicsManager.TotalSystemMass() && totalMass >= 1000;
        if (largeRelativeMass && gravityEnabled)
        {
            GravitySphere.enabled = true;
            GravitySphere.radius = Mathf.Pow(2 * Vector3Utils.GetRadiusFromVolume(totalMass), 2);
        }
        else
        {
            GravitySphere.enabled = false;
        }
    }

    public HashSet<BlockBehavior> blocks = new HashSet<BlockBehavior>();
    public BlockBehavior trackingBlock;
    Vector3 offset;
    public bool IsOccupying()
    {
        foreach (BlockBehavior block in blocks)
        {
            if (block.GetSlotManager().IsOccupying())
            {
                return true;
            }
        }
        return false;
    }

    public float totalMass; //We're going to treat each block as having the same mass.
    public float averageDrag; //We're going to treat each block as having the same mass.
    public float diagonal;

    public void AddBlock(BlockBehavior block)
    {
        if (block.cluster != null && block.cluster != this && block.cluster.blocks.Count > 0 && blocks.Count > 0)
        {
            ClusterBehavior parentCluster = block.cluster.totalMass > totalMass ? block.cluster : this;
            ClusterBehavior childCluster = block.cluster.totalMass <= totalMass ? block.cluster : this;

            parentCluster.trackingBlock.name = "Block";
            childCluster.trackingBlock.name = "Block";

            foreach (BlockBehavior childBlock in childCluster.blocks)
            {
                childBlock.cluster = parentCluster;
                if (parentCluster.trackingBlock != childBlock)
                {
                    childBlock.name = "Block";
                }
                parentCluster.blocks.Add(childBlock);
                Physics.IgnoreCollision(childBlock.Collider, childCluster.GravitySphere, false);
                Physics.IgnoreCollision(childBlock.Collider, parentCluster.GravitySphere);
            }

            parentCluster.BFSRefresh();
            childCluster.blocks.Clear();
            childCluster.Death();
            parentCluster.UpdateGravitySphereRadius();
        }
    }
    public void RemoveBlocks(HashSet<BlockBehavior> removedBlocks)
    {
        BlockBehavior[] toBeRemovedBlocks = new BlockBehavior[removedBlocks.Count];
        removedBlocks.CopyTo(toBeRemovedBlocks);
        for(int i = 0; i < toBeRemovedBlocks.Length; i++)
        {
            if(toBeRemovedBlocks[i] != trackingBlock)
            {
                toBeRemovedBlocks[i].GetSlotManager().ReleaseBlocks();
                blocks.Remove(toBeRemovedBlocks[i]);
            }
        }
    }

    void Update() //Garbage Cleanup
    {
        if(DeathCheck())
        {
            Death();
        }
    }

    bool DeathCheck()
    {
        if (blocks.Count == 0 || transform.parent == null)
        {
            return true;
        }
        return false;
    }

    void Death()
    {
        transform.DetachChildren();
        gameMaster.spawnManager.SystemClusters.Remove(this);
        Destroy(gameObject);
    }

    public void BFSRefresh()
    {
        BlockBehavior currentBlock;
        Queue<BlockBehavior> BlockQueue = new Queue<BlockBehavior>();
        HashSet<BlockBehavior> seenBlocks = new HashSet<BlockBehavior>();

        averageDrag = 0;
        totalMass = 0;
        Vector3 currentCenter = Vector3.zero;

        BlockQueue.Enqueue(blocks.First());
        while (BlockQueue.Count != 0)
        {
            currentBlock = BlockQueue.Dequeue();
            if (currentBlock != null && !seenBlocks.Contains(currentBlock) && currentBlock.GetSlotManager().GetSlots() != null)
            {
                currentBlock.cluster = this;
                seenBlocks.Add(currentBlock);
                Physics.IgnoreCollision(currentBlock.Collider, GravitySphere);
                Vector3 currentBlockPosition = currentBlock.transform.position;
                totalMass += 1;
                averageDrag += currentBlock.GetComponent<Rigidbody>().drag;
                currentCenter += currentBlockPosition;

                foreach (SlotBehavior slot in currentBlock.GetSlotManager().GetSlots().Values)
                {
                    if (!seenBlocks.Contains(slot.GetOccupantBlock()) && slot.IsOccupied())
                    {
                        BlockQueue.Enqueue(slot.GetOccupantBlock());
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
                Physics.IgnoreCollision(originalBlock.Collider, GravitySphere, false);
            }
        }
        if(removedBlocks.Count > 0)
        {
            gameMaster.spawnManager.CreateCluster(removedBlocks);
        }
        blocks = seenBlocks;
        averageDrag = averageDrag / blocks.Count;
        currentCenter /= totalMass;
        UpdateCenter(currentCenter);
    }
    Vector3 GetFurthestBlock(Vector3 root)
    {
        Vector3 max = blocks.First().transform.position;
        float distance = Vector3.Distance(root, max);
        foreach (BlockBehavior block in blocks)
        {
            Vector3 currentPosition = block.transform.position;
            float currDistance = Vector3.Distance(root, currentPosition);
            if (currDistance > distance)
            {
                max = currentPosition;
                distance = currDistance;
            }
        }
        return max;
    }
    void UpdateDiagonal()
    {
        Vector3 furthestFromMiddle = GetFurthestBlock(trackingBlock.transform.position);
        Vector3 furthestFromFurthest = GetFurthestBlock(furthestFromMiddle);
        diagonal = Vector3.Distance(furthestFromFurthest, furthestFromMiddle) + 1;
    }
    void UpdateCenter(Vector3 currentCenter)
    {
        float closestDistance = -1;
        foreach (BlockBehavior currentBlock in blocks)
        {
            float currDistance = Vector3.Distance(currentCenter, currentBlock.transform.position);
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
        offset = trackingBlock.transform.InverseTransformPoint(currentCenter);
        UpdatePosition();
        UpdateDiagonal();
    }
    void UpdatePosition()
    {
        transform.parent = trackingBlock.transform;
        transform.localPosition = offset;
        transform.localEulerAngles = Vector3.zero;
    }


    void OnTriggerStay(Collider other)
    {
        if (BlockUtils.IsBlock(other.gameObject))
        {
            BlockBehavior otherBlock = other.gameObject.GetComponent<BlockBehavior>();
            if (otherBlock != null)
            {
                if (!otherBlock.cluster.IsOccupying() && !IsOccupying() && otherBlock.cluster != this)
                {
                    Vector3Utils.Attract(other: other, transform: transform);
                }
            }
        }
    }

}
