using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GhostPlayerController : Controller
{
    PlayerController Player
    {
        get { return GetComponent<PlayerController>(); }
    }

    readonly float GhostSpeed = 10;
    // Start is called before the first frame update

    void OnEnable()
    {
        Player.enabled = false;
        Start();
    }

    public override void Start()
    {
        transform.SetParent(null);
        gameObject.tag = "Ghost";
        GetComponent<SphereCollider>().enabled = true;
        if(GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
        }
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
    }

    protected void GhostPositionUpdate()
    {
        if(holdFlag)
        {
            Vector3 newPosition = transform.position + transform.forward * GhostSpeed;
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime); //holdscalar is already created with time.delta
        }
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Colliding for ghost");
        if (col.gameObject.CompareTag("Block"))
        {
            ClusterBehavior cluster = col.gameObject.GetComponent<BlockBehavior>().cluster;
            if(cluster.driver.IsAI)
            {
                cluster.DetachDriver(newDriver: Player);
            }
        }
    }

}
