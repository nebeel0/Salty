using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Aggregates blocks
public class SuperBlockBehavior : MonoBehaviour
{
    public GameObject mainBlock = null;
    public GameObject blockRef;
    public GameObject player;

    int prevBlockCount = 0;
    Vector3 prevCenterOfMass = Vector3.zero;
    //TODO multiple cameras per game Object, multiple players on one block?
    float totalMass = 0; //We're going to treat each block as having the same mass.
    float displacementFactor = 1.2f;
    float diagonal;
    Vector3 centerOfMass;

    HashSet<GameObject> blocks = new HashSet<GameObject>();
    float centerUpdateCooldownMax;
    float centerUpdateCooldownTimer;

    void Start()
    {
        if(mainBlock == null)
        {
            mainBlock = Instantiate(blockRef);
            mainBlock.transform.SetParent(gameObject.transform);
        }
        //TODO if player is null, use an AI player
        centerUpdateCooldownMax = blocks.Count + 1;
        centerUpdateCooldownTimer = centerUpdateCooldownMax;
    }


    void Update()
    {
        //TODO if player is null, use an AI player
        DeathCheck();
        CenterUpdateCooldownUpdate();
        if(centerUpdateCooldownTimer > 0)
        {
            UpdateCenterOfBlocks();
            centerUpdateCooldownTimer = centerUpdateCooldownMax;
        }
        PositionUpdate();
    }

    void DeathCheck()
    {
        if(mainBlock == null)
        {
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

    void UpdateCenterOfBlocks()
    {
        bool playerIsChild = player.transform.parent == transform;
        transform.DetachChildren();
        if(playerIsChild)
        {
            player.transform.SetParent(transform);
        }

        BlockBehavior currentBlockBehavior;
        Queue<BlockBehavior> blockBehaviorQueue = new Queue<BlockBehavior>();
        HashSet<GameObject> seenBlocks = new HashSet<GameObject>();
        blockBehaviorQueue.Enqueue(mainBlock.GetComponent<BlockBehavior>());

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
        prevBlockCount = blocks.Count;
    }

    void PositionUpdate()
    {
        Debug.Log(prevCenterOfMass);
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
}
