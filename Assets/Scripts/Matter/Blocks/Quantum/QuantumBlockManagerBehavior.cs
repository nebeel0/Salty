using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantumBlockManagerBehavior : MonoBehaviour
{
    public QuantumBlockBehavior Block
    {
        get { 
            if(transform.parent != null && BlockUtils.IsBlock(transform.parent.gameObject))
            {
                return transform.parent.gameObject.GetComponent<QuantumBlockBehavior>();
            }
            return null;
        }
    }

    public GameMaster gameMaster
    {
        get
        {
            return Block.gameMaster;
        }
    }
}
