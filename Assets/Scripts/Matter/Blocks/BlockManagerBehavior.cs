using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManagerBehavior : MonoBehaviour
{
    public BlockBehavior BlockBehavior
    {
        get { 
            if(transform.parent != null && transform.parent.CompareTag("Block"))
            {
                return transform.parent.gameObject.GetComponent<BlockBehavior>();
            }
            return null;
        }
    }
}
