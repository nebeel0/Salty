using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

//Aggregates blocks
public class ClusterBehavior : MonoBehaviour
{
    public HashSet<GameObject> blocks = new HashSet<GameObject>();

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


    [ReadOnly] public int prevBlockCount = 0;
    Vector3 prevCenterOfMass = Vector3.zero;
    //TODO multiple cameras per game Object, multiple players on one block?
    public float totalMass = 0; //We're going to treat each block as having the same mass.
    public float averageDrag = 0; //We're going to treat each block as having the same mass.
    float displacementFactor = 1.2f;
    float diagonal;
    Vector3 centerOfMass = Vector3.zero;

    float centerUpdateCooldownMax;
    float centerUpdateCooldownTimer;

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

    void Start()
    {
        centerUpdateCooldownMax = blocks.Count;
        centerUpdateCooldownTimer = centerUpdateCooldownMax;
    }


    void Update()
    {
        CenterUpdateCooldownUpdate();
        if (centerUpdateCooldownTimer <= 0)
        {
            UpdateCenterOfBlocks();
            centerUpdateCooldownTimer = centerUpdateCooldownMax;
        }
        PositionUpdate();
    }

    void DeathCheck()
    {
        if (blocks.Count == 0)
        {
            transform.DetachChildren();
            Destroy(gameObject);
        }
    }

    void CenterUpdateCooldownUpdate()
    {
        if (centerUpdateCooldownTimer > 0)
        {
            centerUpdateCooldownTimer -= Time.deltaTime;
        }
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
        BlockConnectionBehavior currentBlockConnectionBehavior;
        Queue<BlockConnectionBehavior> BlockConnectionBehaviorQueue = new Queue<BlockConnectionBehavior>();
        HashSet<GameObject> seenBlocks = new HashSet<GameObject>();

        GameObject firstBlock = GetFirstBlock();
        if (firstBlock == null)
        {
            blocks = seenBlocks;
            DeathCheck();
            return;
        }
        else
        {
            BlockConnectionBehaviorQueue.Enqueue(firstBlock.GetComponent<BlockConnectionBehavior>());
        }

        DetachBlocks();

        float drag = 0;
        float mass = 0;
        Vector3 currentCenterOfMass = Vector3.zero;
        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;

        while (BlockConnectionBehaviorQueue.Count != 0)
        {
            currentBlockConnectionBehavior = BlockConnectionBehaviorQueue.Dequeue();
            if (currentBlockConnectionBehavior != null && !seenBlocks.Contains(currentBlockConnectionBehavior.gameObject) && currentBlockConnectionBehavior.connectedBlocks != null)
            {
                currentBlockConnectionBehavior.gameObject.transform.SetParent(gameObject.transform);
                seenBlocks.Add(currentBlockConnectionBehavior.gameObject);
                Vector3 currentRelativePosition = Vector3.zero - currentBlockConnectionBehavior.gameObject.transform.position;
                mass += 1;
                drag += currentBlockConnectionBehavior.gameObject.GetComponent<Rigidbody>().drag;
                currentCenterOfMass += currentRelativePosition;

                min.x = System.Math.Min(min.x, currentRelativePosition.x);
                min.y = System.Math.Min(min.y, currentRelativePosition.y);
                min.z = System.Math.Min(min.z, currentRelativePosition.z);

                max.x = System.Math.Max(max.x, currentRelativePosition.x);
                max.y = System.Math.Max(max.y, currentRelativePosition.y);
                max.z = System.Math.Max(max.z, currentRelativePosition.z);
                foreach (BlockConnectionBehavior.BlockConnection blockConnection in currentBlockConnectionBehavior.connectedBlocks.Values)
                {
                    if (!seenBlocks.Contains(blockConnection.otherBlock) && blockConnection.otherBlock != null)
                    {
                        BlockConnectionBehaviorQueue.Enqueue(blockConnection.otherBlock.GetComponent<BlockConnectionBehavior>());
                    }
                }
            }
        }

        diagonal = Vector3.Distance(min, max) * displacementFactor;
        currentCenterOfMass /= mass;
        blocks = seenBlocks;
        if(blocks.Count != prevBlockCount)
        {
            centerOfMass = currentCenterOfMass; //TODO don't do this if we want third person and 1st person
        }
        totalMass = mass;
        averageDrag = drag / blocks.Count;
        prevBlockCount = blocks.Count;
    }

    void PositionUpdate()
    {
        foreach (GameObject block in blocks)
        {
            block.transform.parent = null;
        }
        prevCenterOfMass = Vector3.Lerp(prevCenterOfMass, centerOfMass, Time.deltaTime);
        transform.position = prevCenterOfMass;
        foreach (GameObject block in blocks)
        {
            block.transform.parent = transform;
        }
    }

    public void DistributeForce(Vector3 forceVector, ForceMode forceMode)
    {
        Vector3 distributedForce = forceVector / blocks.Count;
        foreach (GameObject block in blocks)
        {
            block.GetComponent<Rigidbody>().AddForce(distributedForce, forceMode);
        }
    }

    public void Brake()
    {
        foreach(GameObject block in blocks)
        {
            block.GetComponent<Rigidbody>().velocity = Vector3.zero;
            block.GetComponent<Rigidbody>().freezeRotation = true;
        }
    }
    public void UnBrake()
    {
        foreach (GameObject block in blocks)
        {
            block.GetComponent<Rigidbody>().freezeRotation = false;
        }
    }
}
