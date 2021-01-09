using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownBehavior : QuarkBehavior
{
    protected override void Start()
    {
        base.Start();
        particleType = "quarkNeg";
        SetParticleColor();
    }
}
