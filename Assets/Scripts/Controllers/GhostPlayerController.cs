using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GhostPlayerController : Controller
{
    public GameObject SuperBlockRef;
    PlayerController playerController
    {
        get { return GetComponent<PlayerController>(); }
    }
    float ghostSpeed = 10;
    // Start is called before the first frame update

    void OnEnable()
    {
        Start();
    }

    public override void Start()
    {
        gameObject.tag = "Ghost";
        GetComponent<SphereCollider>().enabled = true;
        gameObject.AddComponent<Rigidbody>();
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
        primaryCameraRootPosition = new Vector3(0, 0, 0);
        target = transform;
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        GhostPositionUpdate();
        DeathCheck();
    }

    protected void GhostPositionUpdate()
    {
        if(holdFlag)
        {
            Vector3 newPosition = transform.position + transform.forward * ghostSpeed;
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime); //holdscalar is already created with time.delta
        }
    }

    void DeathCheck()
    {
        if(transform.childCount <= 0)
        {
            Destroy(gameObject);
        }
    }


    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Colliding for ghost");
        if (col.gameObject.CompareTag("Block"))
        {
            // TODO super block permission check to join
            if(col.gameObject.GetComponent<BlockBehavior>().inSuperBlock)
            {

            }
            else
            {
                GameObject mainBlock = col.gameObject;
                GameObject superBlock = Instantiate(SuperBlockRef);
                transform.SetParent(superBlock.transform);
                superBlock.GetComponent<SuperBlockBehavior>().mainBlock = mainBlock;

                playerController.enabled = true;
                enabled = false;
            }
        }
    }

}
