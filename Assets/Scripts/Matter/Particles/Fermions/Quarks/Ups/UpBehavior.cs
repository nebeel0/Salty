using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpBehavior : QuarkBehavior
{
    public override void Start()
    {
        particleType = ParticleUtils.quarkPos;
        base.Start();
    }

}
