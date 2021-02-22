using System.Collections.Generic;
using UnityEngine;

public class ActionParams
{
    public Vector3 direction;
    public float scalar;
    public bool lockOnFlag;
    public float createdTime; //https://docs.unity3d.com/ScriptReference/Time-time.html

    public ActionParams(Vector3 direction, float scalar, bool lockOnFlag)
    {
        this.direction = direction;
        this.scalar = scalar;
        this.lockOnFlag = lockOnFlag;
        createdTime = Time.time; // TODO If we're simulating everything we'll need to do an advanced simulation that takes into account lost mass, since if we export a particle, and then travel, it'll be a different path, just copy the current state, and actually run everything?
    }
}
