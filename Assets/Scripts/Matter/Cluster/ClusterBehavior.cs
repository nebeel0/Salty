using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

//Aggregates blocks
public class ClusterBehavior : GameBehavior
{
    public SphereCollider MessageSphere;
    public void UpdateMessageSphereRadius()
    {
        if (gameMaster.ClusterMessageCheck(this))
        {
            MessageSphere.enabled = true;
            MessageSphere.radius = Mathf.Pow(2 * Vector3Utils.GetRadiusFromVolume(totalMass), 2);
        }
        else
        {
            MessageSphere.enabled = false;
        }
    }

    public HashSet<BlockBehavior> blocks = new HashSet<BlockBehavior>();
    public PlayerController driver;
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

    public void DetachDriver(PlayerController newDriver=null)
    {
        if(newDriver != driver)
        {
            if(driver != null)
            {
                driver.cluster = null;
                driver.Ghost.enabled = true;
            }

            driver = newDriver == null ? gameMaster.spawnManager.CreateAIPlayer() : newDriver;
            driver.cluster = this;
            driver.enabled = true;
            UpdateMessageSphereRadius();
        }
    }

    public void AddBlock(BlockBehavior block)
    {
        if (block.cluster != null && block.cluster != this && block.cluster.blocks.Count > 0 && blocks.Count > 0)
        {
            ClusterBehavior parentCluster = block.cluster.totalMass > totalMass ? block.cluster : this;
            ClusterBehavior childCluster = block.cluster.totalMass <= totalMass ? block.cluster : this;

            //bool parentIsAI = parentCluster.driver.GetComponent<AIController>() != null;
            //bool childIsAI = childCluster.driver.GetComponent<AIController>() != null;
            //bool oneRealDriverCheck = (parentIsAI && !childIsAI) || (childIsAI && !parentIsAI);

            //PlayerController primaryPlayer; //Identify Real Player
            //PlayerController discardedPlayer;

            //if (oneRealDriverCheck)
            //{
            //    primaryPlayer = parentIsAI ? childCluster.driver : parentCluster.driver; //Identify Real Player
            //    discardedPlayer = parentIsAI ? parentCluster.driver : childCluster.driver;
            //} 
            //else //In cases where the particants are both AI, or both players, the larger ones absorbs the other
            //{
            //    primaryPlayer = parentCluster.driver;
            //    discardedPlayer = childCluster.driver;
            //}
            //primaryPlayer.cluster = null;
            //discardedPlayer.cluster = null;
            //parentCluster.DetachDriver(primaryPlayer);
            //discardedPlayer.OnGhostMode();


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
                Physics.IgnoreCollision(childBlock.collider, childCluster.MessageSphere, false);
                Physics.IgnoreCollision(childBlock.collider, parentCluster.MessageSphere);
            }

            parentCluster.BFSRefresh();
            childCluster.blocks.Clear();
            childCluster.Death();
            parentCluster.driver.ResetParenting();
            parentCluster.UpdateMessageSphereRadius();
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
        Vector3 currentCenter = Vector3.zero;

        BlockQueue.Enqueue(blocks.First());
        while (BlockQueue.Count != 0)
        {
            currentBlock = BlockQueue.Dequeue();
            if (currentBlock != null && !seenBlocks.Contains(currentBlock) && currentBlock.slotManager.slots != null)
            {
                currentBlock.cluster = this;
                seenBlocks.Add(currentBlock);
                Physics.IgnoreCollision(currentBlock.collider, MessageSphere);
                Vector3 currentBlockPosition = currentBlock.transform.position;
                totalMass += 1;
                averageDrag += currentBlock.GetComponent<Rigidbody>().drag;
                currentCenter += currentBlockPosition;

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
                Physics.IgnoreCollision(originalBlock.collider, MessageSphere, false);
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

    public void UpdatePosition()
    {
        transform.parent = trackingBlock.transform;
        transform.localPosition = offset;
        transform.localEulerAngles = Vector3.zero;
    }

    //Possible Actions
    //TODO fire particle
    //TODO fire energy
    public void SetSize(float scaleFactor)
    {
        foreach (BlockBehavior block in blocks)
        {
            block.transform.localScale = transform.localScale * scaleFactor;
        }
    }

    public void SetColor(Color color)
    {
        foreach (BlockBehavior block in blocks)
        {
            block.SetColor(color);
        }
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

    //Character Routing Actions

    void OnTriggerStay(Collider other)
    {
        //if(driver != null)
        //{
        //    driver.Character.CharacterAutomationManager.ClusterMessageTrigger(other);
        //}
    }

}
