using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageSetup : MonoBehaviour
{
    //TODO support non box shaped cages
    public Vector3 dimensions = new Vector3(500, 500, 500);
    public int resolution=10;  //default resolution of a plane is 10x10

    public GameObject cagePlaneRef;

    public List<GameObject> planes = new List<GameObject>();
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        dimensions = new Vector3(RoundToResolution(dimensions.x), RoundToResolution(dimensions.y), RoundToResolution(dimensions.z));
        //opposites rotated around same axis
        planes.Add(GeneratePlane(0, 1, false));  //Rotates along x axis and moved along z
        planes.Add(GeneratePlane(1, 2, false));  //Rotates along x axis and y axis and moved along x
        planes.Add(GeneratePlane(0, 2, false));  //moved along y
        planes.Add(GeneratePlane(0, 1, true));  //Rotates along x axis and moved along z
        planes.Add(GeneratePlane(1, 2, true));  //Rotates along x axis and y axis and moved along x
        planes.Add(GeneratePlane(0, 2, true));  //moved along y
    }

    GameObject GeneratePlane(int dim_1, int dim_2, bool opposite)
    {
        int planeType = 3 - dim_1 - dim_2;
        GameObject plane = Instantiate(cagePlaneRef, transform);
        CagePlaneBehavior planeBehavior= plane.GetComponent<CagePlaneBehavior>();
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
        Vector3 rotatedAxis = new Vector3(0, 0, 0);
        Vector3 displacement = new Vector3(0, 0, 0);
        rotatedAxis[dim_1] = 1;
        displacement[planeType] = displaceDistance;
        if (opposite)
        {
            plane.transform.RotateAround(transform.position, rotatedAxis, 180);
            displacement *= -1;
        }
        plane.transform.RotateAround(transform.position, rotatedAxis, 180);
        plane.transform.position += displacement;
        return plane;
    }


    void Update()
    {
        
    }

    int RoundToResolution(float param)
    {
        int paramInt = (int)param; 
        int result = (paramInt / resolution) * resolution;
        return System.Math.Max(resolution, result);
    }
}
