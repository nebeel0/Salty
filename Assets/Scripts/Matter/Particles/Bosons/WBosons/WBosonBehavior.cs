using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WBosonBehavior : BosonBehavior
{
    public override void Start()
    {
        particleType = ParticleUtils.wBoson;
        base.Start();
    }

}
