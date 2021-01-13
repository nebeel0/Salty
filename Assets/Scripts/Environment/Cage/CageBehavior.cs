using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageBehavior : GameBehavior
{
    //TODO support non box shaped cages
    public Vector3 dimensions = new Vector3(500, 500, 500);
    public int resolution=10;  //default resolution of a plane is 10x10
    public GameObject cagePlaneRef;
    public int randomChance;
    public int coolDownTime = 120;
    public float fadeOutPeriodMax = 10;
    public int loadingTimeMax;
    public bool grayScale;
    public bool allOn;

    List<GameObject> planes = new List<GameObject>();

    float scalingFactor
    {
        get { return transform.localScale.x; } //Should be square-esque TODO shift so rectangles can exist
    }
    void Start()
    {
        if(resolution < 10)
        {
            Debug.LogError("Resolution can't be lower than 10");
        }
        transform.localPosition = Vector3.zero;
        Vector3 savedEulerAngles = transform.eulerAngles;
        transform.eulerAngles = Vector3.zero;
        dimensions = new Vector3(RoundToResolution(dimensions.x), RoundToResolution(dimensions.y), RoundToResolution(dimensions.z));
        //opposites rotated around same axis
        planes.Add(GeneratePlane(0, 1, false));  //Rotates along x axis and moved along z
        planes.Add(GeneratePlane(1, 2, false));  //Rotates along x axis and y axis and moved along x
        planes.Add(GeneratePlane(0, 2, false));  //moved along y
        planes.Add(GeneratePlane(0, 1, true));  //Rotates along x axis and moved along z
        planes.Add(GeneratePlane(1, 2, true));  //Rotates along x axis and y axis and moved along x
        planes.Add(GeneratePlane(0, 2, true));  //moved along y
        transform.eulerAngles = savedEulerAngles;
    }

    GameObject GeneratePlane(int dim_1, int dim_2, bool opposite)
    {
        int planeType = 3 - dim_1 - dim_2;
        GameObject plane = Instantiate(cagePlaneRef, transform);
        CagePlaneBehavior planeBehavior= plane.GetComponent<CagePlaneBehavior>();

        planeBehavior.randomChance = randomChance;
        planeBehavior.loadingTimeMax = loadingTimeMax;
        planeBehavior.allOn = allOn;
        planeBehavior.coolDownTime = coolDownTime;
        planeBehavior.fadeOutPeriodMax = fadeOutPeriodMax;
        planeBehavior.grayScale = grayScale;
        planeBehavior.SetDimensions(new Vector2(dimensions[dim_1], dimensions[dim_2]));
        planeBehavior.SetResolution(resolution);

        switch (planeType)
        {
            case 2:
                plane.transform.RotateAround(transform.position, Vector3.right, 90);
                break;
            case 0:
                plane.transform.RotateAround(transform.position, Vector3.right, 90);
                plane.transform.RotateAround(transform.position, Vector3.up, 90);
                break;
            case 1:
                break;
        }

        float displaceDistance = dimensions[planeType] / 2;
        Vector3 rotatedAxis = Vector3.zero;
        Vector3 displacement = Vector3.zero;
        rotatedAxis[dim_1] = 1;
        displacement[planeType] = displaceDistance;
        if (opposite)
        {
            plane.transform.RotateAround(transform.position, rotatedAxis, 180);
            displacement *= -1;
        }
        plane.transform.RotateAround(transform.position, rotatedAxis, 180);
        plane.transform.localPosition += displacement*scalingFactor;
        return plane;
    }


    int RoundToResolution(float param)
    {
        int paramInt = (int)param; 
        int result = (paramInt / resolution) * resolution;
        return System.Math.Max(resolution, result);
    }
}
