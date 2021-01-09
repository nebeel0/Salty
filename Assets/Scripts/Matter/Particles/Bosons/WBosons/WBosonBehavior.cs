using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WBosonBehavior : BosonBehavior
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        particleType = "wBoson";
        gameObject.layer = ParticleUtils.wBosonLayer;
        SetParticleColor();
    }
}
