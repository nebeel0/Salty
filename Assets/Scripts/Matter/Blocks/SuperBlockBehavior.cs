using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

//Aggregates blocks
public class SuperBlockBehavior : MonoBehaviour
{
    public GameObject mainBlock = null;
    public GameObject blockRef;
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
            if(transform.parent != null && transform.parent.CompareTag("Player"))
            {
                return transform.parent.gameObject;
            }
            return null;
        }
    }


    [ReadOnly]
    public int prevBlockCount = 0;
    Vector3 prevCenterOfMass = Vector3.zero;
    //TODO multiple cameras per game Object, multiple players on one block?
    public float totalMass = 0; //We're going to treat each block as having the same mass.
    public float averageDrag = 0; //We're going to treat each block as having the same mass.
    float displacementFactor = 1.2f;
    float diagonal;
    Vector3 centerOfMass;

    HashSet<GameObject> blocks = new HashSet<GameObject>();
    float centerUpdateCooldownMax;
    float centerUpdateCooldownTimer;

    void Start()
    {
        centerUpdateCooldownMax = blocks.Count + 1;
        centerUpdateCooldownTimer = centerUpdateCooldownMax;
    }


    void Update()
    {
        //TODO if player is null, use an AI player
        DeathCheck();
        CenterUpdateCooldownUpdate();
        if (centerUpdateCooldownTimer > 0)
        {
            UpdateCenterOfBlocks();
            centerUpdateCooldownTimer = centerUpdateCooldownMax;
        }
        PositionUpdate();
    }

    void DeathCheck()
    {
        bool noPlayers = childPlayers.Count == 0 && parentPlayer == null;
        if (mainBlock == null || noPlayers)
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

    void BlockUpdate(BlockBehavior blockBehavior)
    {
        blockBehavior.gameObject.transform.SetParent(gameObject.transform);
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

    void UpdateCenterOfBlocks()
    {
        DetachBlocks();
        BlockBehavior currentBlockBehavior;
        Queue<BlockBehavior> blockBehaviorQueue = new Queue<BlockBehavior>();
        HashSet<GameObject> seenBlocks = new HashSet<GameObject>();
        blockBehaviorQueue.Enqueue(mainBlock.GetComponent<BlockBehavior>());

        float drag = 0;
        float mass = 0;
        Vector3 currentCenterOfMass = Vector3.zero;
        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;

        while (blockBehaviorQueue.Count != 0)
        {
            currentBlockBehavior = blockBehaviorQueue.Dequeue();
            if (currentBlockBehavior != null && !seenBlocks.Contains(currentBlockBehavior.gameObject) && currentBlockBehavior.connectedBlocks != null)
            {
                BlockUpdate(currentBlockBehavior);
                seenBlocks.Add(currentBlockBehavior.gameObject);
                Vector3 currentRelativePosition = mainBlock.transform.position - currentBlockBehavior.gameObject.transform.position;
                mass += 1;
                drag += currentBlockBehavior.gameObject.GetComponent<Rigidbody>().drag;
                currentCenterOfMass += currentRelativePosition;

                min.x = System.Math.Min(min.x, currentRelativePosition.x);
                min.y = System.Math.Min(min.y, currentRelativePosition.y);
                min.z = System.Math.Min(min.z, currentRelativePosition.z);

                max.x = System.Math.Max(max.x, currentRelativePosition.x);
                max.y = System.Math.Max(max.y, currentRelativePosition.y);
                max.z = System.Math.Max(max.z, currentRelativePosition.z);
                foreach (BlockBehavior.BlockConnection blockConnection in currentBlockBehavior.connectedBlocks.Values)
                {
                    if (blockConnection.Valid())
                    {
                        if (!seenBlocks.Contains(blockConnection.otherBlock) && blockConnection.otherBlock != null)
                        {
                            blockBehaviorQueue.Enqueue(blockConnection.otherBlock.GetComponent<BlockBehavior>());
                        }
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
        transform.position = mainBlock.transform.position - prevCenterOfMass;
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
