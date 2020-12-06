using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperBlock : MonoBehaviour
{
    public GameObject blockRef;

    List<GameObject> blocks;

    // Start is called before the first frame update
    void Start()
    {
        if(blocks is null)
        {
            blocks = new List<GameObject>();
            GameObject block = Instantiate(blockRef);
            blocks.Add(block);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
