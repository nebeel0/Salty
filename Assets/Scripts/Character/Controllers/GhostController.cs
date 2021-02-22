using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GhostController : MonoBehaviour
{
    //readonly float GhostSpeed = 10;
    //// Start is called before the first frame update

    //void OnEnable()
    //{
    //    Player.enabled = false;
    //    //Start();
    //}

    ////public void Start()
    ////{
    ////    transform.SetParent(null);
    ////    gameObject.tag = "Ghost";
    ////    GetComponent<SphereCollider>().enabled = true;
    ////    Player.primaryCameraRootPosition = new Vector3(0, 0, 0);
    ////    Player.rotationTarget = transform;
    ////    base.Start();
    ////}

    ////// Update is called once per frame
    ////protected override void Update()
    ////{
    ////    base.Update();
    ////    GhostPositionUpdate();
    ////}

    ////protected void GhostPositionUpdate()
    ////{
    ////    if(holdFlag)
    ////    {
    ////        Vector3 newPosition = transform.position + transform.forward * GhostSpeed;
    ////        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime); //holdscalar is already created with time.delta
    ////    }
    ////}

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Block"))
    //    {
    //        Debug.Log("Colliding for ghost");
    //        ClusterBehavior cluster = other.gameObject.GetComponent<BlockBehavior>().cluster;
    //        if (cluster.driver.GetComponent<CharacterBehavior>().IsAI)
    //        {
    //            cluster.DetachDriver(newDriver: Player);
    //        }
    //    }
    //}

}
