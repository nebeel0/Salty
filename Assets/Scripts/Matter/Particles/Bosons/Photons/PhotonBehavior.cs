using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonBehavior : BosonBehavior
{
    //Collider should be a trigger
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        particleType = "pBoson";
        gameObject.layer = ParticleUtils.pBosonLayer;
        SetParticleColor();
    }

    protected override void OnTriggerEnter(Collider other)
    {

    }

}
