using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZBosonBehavior : BosonBehavior
{
    public override void Start()
    {
        particleType = ParticleUtils.zBoson;
        base.Start();
    }

    protected void TransferMomentum(ParticleBehavior particle)
    {

    }

}
