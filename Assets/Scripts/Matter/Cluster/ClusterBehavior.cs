using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

//Aggregates blocks
public class ClusterBehavior : GameBehavior
{
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

    public List<GameObject> childBlocks //TODO remove this
    {
        get
        {
            List<GameObject> childBlocks = new List<GameObject>();
            foreach (Transform child in transform)
            {
                if (child.CompareTag("Block"))
                {
                    childBlocks.Add(child.gameObject);
                }
            }
            return childBlocks;
        }
    }

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

    [ReadOnly] public int prevBlockCount = 0;
    public float totalMass = 0; //We're going to treat each block as having the same mass.
    public float averageDrag = 0; //We're going to treat each block as having the same mass.
    float displacementFactor = 1.2f;
    float diagonal;
    Vector3 centerOfMass = Vector3.zero;

    public void RemoveBlock(BlockBehavior block)
    {
        block.transform.parent = null;
        blocks.Remove(block);
        if (!DeathCheck())
        {
            UpdateCenterOfBlocks();
        }
        else
        {
            Death();
        }
    }

    public void Merge(ClusterBehavior cluster)
    {
        //TODO do we need mutex locks?
        if(cluster != null)
        {
            ClusterBehavior parentCluster = cluster.blocks.Count > blocks.Count ? cluster : this;
            ClusterBehavior childCluster = cluster.blocks.Count <= blocks.Count ? cluster : this;

            foreach (Transform child in childCluster.gameObject.transform)
            {
                child.parent = parentCluster.transform;
            }
            if (childCluster.parentPlayer != null)
            {
                childCluster.parentPlayer.transform.parent = parentCluster.transform;
            }
            parentCluster.UpdateCenterOfBlocks();
            Destroy(childCluster.gameObject);
        }
    }

    void Update()
    {
        UpdateCenterOfBlocks();
        PositionUpdate();
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
        Destroy(gameObject);
    }

    void DetachBlocks()
    {
        foreach(Transform child in transform)
        {
            if(child.CompareTag("Block"))
            {
                child.parent = null;
            }
        }
    }

    GameObject GetFirstBlock()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Block"))
            {
                return child.gameObject;
            }
        }
        return null;
    }


    void UpdateCenterOfBlocks()
    {
        SlotManagerBehavior currentBlockSlotManagerBehavior;
        Queue<SlotManagerBehavior> BlockSlotManagerBehaviorQueue = new Queue<SlotManagerBehavior>();
        HashSet<BlockBehavior> seenBlocks = new HashSet<BlockBehavior>();

        GameObject firstBlock = GetFirstBlock();
        if (firstBlock == null)
        {
            blocks = seenBlocks;
            DeathCheck();
            return;
        }
        else
        {
            BlockSlotManagerBehaviorQueue.Enqueue(firstBlock.GetComponent<BlockBehavior>().slotManager);
        }

        DetachBlocks();

        float drag = 0;
        float mass = 0;
        Vector3 currentCenterOfMass = Vector3.zero;
        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;

        while (BlockSlotManagerBehaviorQueue.Count != 0)
        {
            currentBlockSlotManagerBehavior = BlockSlotManagerBehaviorQueue.Dequeue();
            if (currentBlockSlotManagerBehavior != null && !seenBlocks.Contains(currentBlockSlotManagerBehavior.block) && currentBlockSlotManagerBehavior.slots != null)
            {
                BlockBehavior currentBlock = currentBlockSlotManagerBehavior.block;
                currentBlock.transform.SetParent(gameObject.transform);
                seenBlocks.Add(currentBlock);
                Vector3 currentBlockPosition = currentBlock.transform.position;
                mass += 1;
                drag += currentBlock.GetComponent<Rigidbody>().drag;
                currentCenterOfMass += currentBlockPosition;

                min.x = System.Math.Min(min.x, currentBlockPosition.x);
                min.y = System.Math.Min(min.y, currentBlockPosition.y);
                min.z = System.Math.Min(min.z, currentBlockPosition.z);

                max.x = System.Math.Max(max.x, currentBlockPosition.x);
                max.y = System.Math.Max(max.y, currentBlockPosition.y);
                max.z = System.Math.Max(max.z, currentBlockPosition.z);
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
        currentCenterOfMass /= mass;
        blocks = seenBlocks;
        centerOfMass = currentCenterOfMass; //TODO don't do this if we want third person and 1st person
        totalMass = mass;
        averageDrag = drag / blocks.Count;
        prevBlockCount = blocks.Count;
    }

    void PositionUpdate()
    {
        if(childBlocks.Count == blocks.Count)
        {
            foreach (BlockBehavior block in blocks)
            {
                block.transform.parent = null;
            }
            transform.position = centerOfMass;
            foreach (BlockBehavior block in blocks)
            {
                block.transform.parent = transform;
            }
        }
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
