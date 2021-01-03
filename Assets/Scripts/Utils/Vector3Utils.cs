using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Utils
{
    //Vector Utils
    public static bool V3Equal(Vector3 a, Vector3 b, float threshold = 0.001f)
    {
        return Vector3.SqrMagnitude(a - b) < threshold;
    }
}
