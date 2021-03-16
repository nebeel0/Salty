using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RotationUtils
{
    public class RotationState
    {
        public float yaw;
        public float pitch;
        public float roll;

        public void SetFromTransform(Transform t)
        {
            pitch = t.eulerAngles.x;
            yaw = t.eulerAngles.y;
            roll = t.eulerAngles.z;
        }

        public void SetFromEulerAngles(Vector3 eulerAngles)
        {
            pitch = eulerAngles.x;
            yaw = eulerAngles.y;
            roll = eulerAngles.z;
        }

        public void Copy(RotationState target)
        {
            yaw = target.yaw;
            pitch = target.pitch;
            roll = target.roll;
        }

        public void LerpTowards(RotationState target, float rotationLerpPct)
        {
            yaw = Mathf.LerpAngle(yaw, target.yaw, rotationLerpPct);
            pitch = Mathf.LerpAngle(pitch, target.pitch, rotationLerpPct);
            roll = Mathf.LerpAngle(roll, target.roll, rotationLerpPct);
        }
        public void LerpTowardsNeutral(float rotationLerpPct)
        {
            pitch = Mathf.LerpAngle(roll, 0, rotationLerpPct);
            roll = Mathf.LerpAngle(roll, 0, rotationLerpPct);
        }

        public void LerpTowardsZero(float rotationLerpPct)
        {
            pitch = Mathf.LerpAngle(roll, 0, rotationLerpPct);
            roll = Mathf.LerpAngle(roll, 0, rotationLerpPct);
            yaw = Mathf.LerpAngle(roll, 0, rotationLerpPct);
        }

        public void UpdateTransform(Transform t)
        {
            t.rotation = Quaternion.Euler(new Vector3(pitch, yaw, roll));
        }

        public void UpdateLocalTransform(Transform t)
        {
            t.localRotation = Quaternion.Euler(new Vector3(pitch, yaw, roll));
        }
    }


    public static void UpdateRotation(Transform target, Vector3 delta, float lerpPercentage)
    {
        RotationState rotationTarget = new RotationState();
        RotationState interpolationHolder = new RotationState();

        rotationTarget.SetFromTransform(target);
        interpolationHolder.SetFromTransform(target);

        rotationTarget.yaw += delta.y;
        rotationTarget.yaw = Mathf.Clamp(rotationTarget.yaw, -90, 90);
        if(rotationTarget.yaw > 90)
        {
            rotationTarget.yaw = 90;
        }
        else if(rotationTarget.yaw < -90)
        {
            rotationTarget.yaw = 270;
        }
        rotationTarget.pitch += delta.x;

        interpolationHolder.LerpTowards(rotationTarget, lerpPercentage);
        interpolationHolder.UpdateTransform(target);
    }

    public static void LerpTowardsNeutral(Transform target, float rotationLerpPct)
    {
        RotationState rotationTarget = new RotationState();
        RotationState interpolationHolder = new RotationState();

        rotationTarget.LerpTowardsNeutral(1);
        interpolationHolder.LerpTowards(rotationTarget, 1);
        interpolationHolder.UpdateTransform(target);
    }
}

