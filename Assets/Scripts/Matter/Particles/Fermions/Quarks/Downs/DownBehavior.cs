using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownBehavior : QuarkBehavior
{
    public override void Start()
    {
        particleType = ParticleUtils.quarkNeg;
        base.Start();
    }
}
