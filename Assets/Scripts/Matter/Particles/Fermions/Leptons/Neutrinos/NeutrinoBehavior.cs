using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutrinoBehavior : LeptonBehavior
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        gameObject.layer = ParticleUtils.leptonNeutralLayer;
        particleType = "leptonNeutral";
        SetParticleColor();
    }
}
