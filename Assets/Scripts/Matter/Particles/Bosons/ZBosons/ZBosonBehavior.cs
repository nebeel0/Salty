using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZBosonBehavior : BosonBehavior
{
    protected override void Start()
    {
        base.Start();
        particleType = "zBoson";
        gameObject.layer = ParticleUtils.zBosonLayer;
        SetParticleColor();
    }

    protected void TransferMomentum(ParticleBehavior particle)
    {

    }

}
