using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutrinoBehavior : LeptonBehavior
{
    public override void Start()
    {
        particleType = ParticleUtils.leptonNeutral;
        base.Start();
    }
}
