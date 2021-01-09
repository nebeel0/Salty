using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorUtils
{
    public static bool ApproxEqual(Color color1, Color color2, float threshold = 0.001f)
    {
        bool redSame = Mathf.Abs(color1.r - color2.r) < threshold;
        bool greenSame = Mathf.Abs(color1.g - color2.g) < threshold;
        bool blueSame = Mathf.Abs(color1.b - color2.b) < threshold;
        bool alphaSame = Mathf.Abs(color1.a - color2.a) < threshold;
        return redSame && greenSame && blueSame && alphaSame;
    }

}

