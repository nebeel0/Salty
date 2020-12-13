using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class UserController : Controller
{
    float totalMass; //We're going to treat each block as having the same mass.
    float displacementFactor = 1.2f;
    HashSet<GameObject> blocks;
    float centerUpdateCooldownMax;
    float centerUpdateCooldownTimer;

    public override void Start()
    {
        base.Start();
        totalMass = 0;
        primaryCameraRootPosition = Vector3.zero;
        blocks = new HashSet<GameObject>();
        centerUpdateCooldownMax = blocks.Count+1;
        centerUpdateCooldownTimer = centerUpdateCooldownMax;
    }

    public override void Update()
    {
        base.Update();
        CooldownUpdate();
        if (centerUpdateCooldownTimer <= 0)
        {
            UpdateCenterOfBlocks();
        }
    }

    void CooldownUpdate()
    {
        if(centerUpdateCooldownTimer > 0)
        {
            centerUpdateCooldownTimer -= Time.deltaTime;
        }
        else
        {
            centerUpdateCooldownTimer = centerUpdateCooldownMax;
        }
    }

    void UpdateCenterOfBlocks()
    {
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
            if (currentBlockBehavior != null && !seenBlocks.Contains(currentBlockBehavior.gameObject))
            {
                seenBlocks.Add(currentBlockBehavior.gameObject);
                Vector3 currentPosition = currentBlockBehavior.gameObject.transform.position;
                mass += 1;
                currentCenterOfMass += currentPosition;

                min.x = System.Math.Min(min.x, currentPosition.x);
                min.y = System.Math.Min(min.y, currentPosition.y);
                min.z = System.Math.Min(min.z, currentPosition.z);

                max.x = System.Math.Max(max.x, currentPosition.x);
                max.y = System.Math.Max(max.y, currentPosition.y);
                max.z = System.Math.Max(max.z, currentPosition.z);
                foreach (BlockBehavior.BlockConnection blockConnection in currentBlockBehavior.connectedBlocks.Values)
                {
                    if(blockConnection.Valid())
                    {
                        if(!seenBlocks.Contains(blockConnection.otherBlock) && blockConnection.otherBlock != null)
                        {
                            blockBehaviorQueue.Enqueue(blockConnection.otherBlock.GetComponent<BlockBehavior>());
                        }
                    }
                }
            }
        }

        primaryCameraDisplacement = Vector3.Distance(min, max) * displacementFactor;
        currentCenterOfMass /= mass;
        Vector3 displacement = mainBlock.GetComponent<BlockBehavior>().mainBlock.transform.position - currentCenterOfMass;
        primaryCameraRootPosition = mainBlock.GetComponent<BlockBehavior>().gameObject.transform.localPosition + displacement;
        blocks = seenBlocks;
        totalMass = mass;
    }




}
