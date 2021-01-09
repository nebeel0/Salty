﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManagerBehavior : MonoBehaviour
{
    public BlockBehavior block
    {
        get { 
            if(transform.parent != null && transform.parent.CompareTag("Block"))
            {
                return transform.parent.gameObject.GetComponent<BlockBehavior>();
            }
            return null;
        }
    }

    public GameMaster gameMaster
    {
        get
        {
            return block.gameMaster;
        }
    }


}