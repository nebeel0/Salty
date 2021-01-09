using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpBehavior : QuarkBehavior
{
    protected override void Start()
    {
        base.Start();
        particleType = "quarkPos";
        SetParticleColor();
    }

}
