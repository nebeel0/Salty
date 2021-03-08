using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockUtils
{
    public static bool IsBlock(GameObject gameObject)
    {
        return gameObject.GetComponent<BlockBehavior>() != null;
    }

}
