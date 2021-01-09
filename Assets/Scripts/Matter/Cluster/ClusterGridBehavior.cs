using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterGridBehavior : GameBehavior
{
    // Start is called before the first frame update
    public GameObject BlockRef;
    public Vector3 blockDimension = Vector3.one;
    public float displacementFactor = 1.1f;

    void Start()
    {
        displacementFactor = Mathf.Max(displacementFactor, 1.1f); //cannot be less than 1.1

        Vector3 dimensions = transform.localScale;
        Vector3 numBlocks = new Vector3();

        for(int i=0; i < 3; i++)
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
                    GameObject instatiatedBlock = Instantiate(BlockRef, transform);
                    instatiatedBlock.GetComponent<BlockBehavior>().gameMaster = gameMaster;
                    instatiatedBlock.GetComponent<BlockBehavior>().BeginnerElementFlag = true;
                    instatiatedBlock.GetComponent<BlockBehavior>().slotManager.displacementFactor = displacementFactor;
                    instatiatedBlock.transform.localPosition = blockPosition;
                    instatiatedBlock.transform.parent = null;
                    instatiatedBlock.transform.localScale = blockDimension;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
