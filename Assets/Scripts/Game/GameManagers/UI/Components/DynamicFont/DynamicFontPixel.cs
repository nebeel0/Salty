using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicFontPixel : QuantumMaterialBehavior
{
    Material pixelMaterial
    {
        get { return GetComponent<MeshRenderer>().material;}
    }

    bool turnedOn;

    protected override void Start()
    {
        turnedOn = GetComponent<MeshRenderer>().enabled == true;
        if (turnedOn)
        {
            base.Start();
        }
    }

    protected override void Update()
    {
        if (turnedOn)
        {
            base.Update();
        }
    }

    public bool On()
    {
        return pixelMaterial.color.a == 1;
    }


    public void Toggle()
    {
        Color currentColor = pixelMaterial.color;
        if(currentColor.a == 1 || GetComponent<MeshRenderer>().enabled == true)
        {
            GetComponent<MeshRenderer>().enabled = false;
            pixelMaterial.color = Color.clear;

        }
        else
        {
            GetComponent<MeshRenderer>().enabled = true;
            pixelMaterial.color = Color.white;
        }
    }
}
