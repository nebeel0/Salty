using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonBehavior : BosonBehavior
{
    public override void Start()
    {
        particleType = ParticleUtils.pBoson;
        base.Start();
    }

    protected override void OnTriggerEnter(Collider other)
    {

    }

}
