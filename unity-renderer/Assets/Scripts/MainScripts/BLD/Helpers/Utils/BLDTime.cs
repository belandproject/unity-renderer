using System.Diagnostics;
using UnityEngine;

public static class BLDTime
{
    static BLDTime() { }

    public static float realtimeSinceStartup { get { return Time.realtimeSinceStartup; } }
}