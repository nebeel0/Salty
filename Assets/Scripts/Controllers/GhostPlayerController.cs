using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GhostPlayerController : Controller
{
    PlayerController playerController;
    float ghostSpeed = 10;
    // Start is called before the first frame update

    void OnEnable()
    {
        Start();
    }

    public override void Start()
    {
        playerController = GetComponent<PlayerController>();
        transform.SetParent(null);
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
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
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
            ClusterBehavior cluster = col.gameObject.GetComponent<BlockBehavior>().cluster;
            playerController.cluster = cluster;
            playerController.enabled = true;
            enabled = false;
        }
    }

}
