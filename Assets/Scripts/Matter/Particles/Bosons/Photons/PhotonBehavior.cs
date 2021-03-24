using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonBehavior : BosonBehavior
{
    public override void Start()
    {
        gameObject.name = "Photon";
        particleType = ParticleUtils.pBoson;
        base.Start();
        rigidbody.drag = 0;
        rigidbody.velocity = 10 * transform.forward;
    }

    protected override void OnCollisionEnter(Collision col)
    {
        Debug.Log("Colliding into " + col.gameObject.name);
    }


    protected override void OnTriggerEnter(Collider other)
    {

    }

}
