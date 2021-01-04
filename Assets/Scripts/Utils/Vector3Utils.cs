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

    public static float GetRadiusFromVolume(float volume)
    {
        float radius = volume * 3.0f / 4.0f;
        radius /= Mathf.PI;
        radius = Mathf.Pow(radius, 1.0f / 3.0f);
        return radius;
    }
}
