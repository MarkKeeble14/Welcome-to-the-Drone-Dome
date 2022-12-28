using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringHelper
{
    public static string ToDetailedString(this Vector2 v)
    {
        return System.String.Format("{0}, {1}", v.x, v.y);
    }

    public static string ToDetailedString(this float v)
    {
        return System.String.Format("{0}", v);
    }
}
