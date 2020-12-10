using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperBlockBehavior : MonoBehaviour
{
    public GameObject blockRef;

    List<GameObject> blocks;

    // Start is called before the first frame update
    public void Start()
    {
        if (blocks is null)
        {
            blocks = new List<GameObject>();
            GameObject block = Instantiate(blockRef, transform);
            blocks.Add(block);
        }
    }

    public void Update()
    {
        //Vector3 total = Vector3.zero;
        //foreach(GameObject block in blocks)
        //{
        //    total += block.transform.position;
        //}
        //total /= blocks.Count;
        //transform.position = total;
    }

}
