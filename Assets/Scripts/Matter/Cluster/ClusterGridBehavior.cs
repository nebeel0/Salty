using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterGridBehavior : GameBehavior
{
    // Start is called before the first frame update
    public GameObject BlockRef;
    public Vector3 blockDimension = Vector3.one;
    public float displacementFactor = 1.1f;
    public bool createGridFlag;

    public override void Start()
    {
        base.Start();
    }

    public void Update()
    {
        if(createGridFlag)
        {
            CreateGrid();
        }
    }

    public void CreateGrid()
    {
        displacementFactor = Mathf.Max(displacementFactor, 1.1f); //cannot be less than 1.1

        Vector3 dimensions = transform.localScale;
        Vector3 numBlocks = new Vector3();

        for (int i = 0; i < 3; i++)
        {
            float neededSpace = blockDimension[i] * displacementFactor;
            numBlocks[i] = (int)(dimensions[i] / neededSpace);
        }


        for (int blockX = 0; blockX < numBlocks.x; blockX++)
        {
            float neededSpace_x = blockDimension.x * displacementFactor;
            float currX = -dimensions.x / 2.0f + neededSpace_x / 2.0f + neededSpace_x * blockX;
            currX /= transform.localScale.x;

            for (int blockY = 0; blockY < numBlocks.y; blockY++)
            {
                float neededSpace_y = blockDimension.y * displacementFactor;
                float currY = -dimensions.y / 2.0f + neededSpace_y / 2.0f + neededSpace_y * blockY;
                currY /= transform.localScale.y;

                for (int blockZ = 0; blockZ < numBlocks.z; blockZ++)
                {
                    float neededSpace_z = blockDimension.z * displacementFactor;
                    float currZ = -dimensions.z / 2.0f + neededSpace_z / 2.0f + neededSpace_z * blockZ;
                    currZ /= transform.localScale.z;

                    Vector3 blockPosition = new Vector3(currX, currY, currZ);
                    BlockBehavior instatiatedBlock = gameMaster.spawnManager.CreateBlock(transform);
                    instatiatedBlock.gameMaster = gameMaster;
                    instatiatedBlock.BeginnerElementFlag = true;
                    instatiatedBlock.slotManager.displacementFactor = displacementFactor;
                    instatiatedBlock.slotManager.slotLockEnabled = true;
                    instatiatedBlock.transform.localPosition = blockPosition;
                    instatiatedBlock.transform.parent = null;
                    instatiatedBlock.transform.localScale = blockDimension;
                }
            }
        }
        Destroy(gameObject);
    }
}
