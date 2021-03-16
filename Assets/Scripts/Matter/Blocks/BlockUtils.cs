using Matter.Block.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockUtils
{
    public static int noBlockCollisionLayer = 16;  //TODO make a better named layer
    public static int blockLayer = 8;


    public static bool IsBlock(GameObject gameObject)
    {
        return gameObject.GetComponent<BlockBehavior>() != null;
    }

    public static bool IsQuantumBlock(GameObject gameObject)
    {
        return gameObject.GetComponent<QuantumBlockBehavior>() != null;
    }
}
