using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CagePanelBehavior : QuantumMaterialBehavior
{
    //TODO be able to control colors from parent
    //TODO toggle higher resolution
    //TODO if same color as gameObject interacting with it, bounce.
    //TODO if opposite color disinintegrate
    public int resolution;

    protected override void Start()
    {
        transform.localScale = new Vector3(resolution/10.0f, 1, resolution/10.0f);  //plane is already 10x normal scale
        base.Start();
    }

}
