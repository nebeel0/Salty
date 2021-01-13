using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Utils
{
    //Vector Utils
    //euler's formula

    //TODO implement all platonic solids https://en.wikipedia.org/wiki/Platonic_solid

    public static Vector3 RelativeFromOther(Vector3 rootLocalPosition, Transform root, Transform other)
    {
        Vector3 worldPosition = root.TransformPoint(rootLocalPosition);
        return other.InverseTransformPoint(worldPosition);
    }

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
    public static int Vector3ToIdx(int i, int ii, int iii)
    {
        string binaryRep = string.Format("{0}{1}{2}", iii, ii, i);
        return System.Convert.ToInt32(binaryRep, 2);
    }

    public static Vector3 RandomEulerAngles()
    {
        float x = Random.Range(-90, 90);
        float y = Random.Range(-90, 90);
        float z = Random.Range(-90, 90);
        return new Vector3(x, y, z);
    }
    public static Vector3 RandomBoundedVector3(Vector3 dimensions)
    {
        float x = Random.Range(dimensions.x * -1, dimensions.x);
        float y = Random.Range(dimensions.y * -1, dimensions.y);
        float z = Random.Range(dimensions.z * -1, dimensions.z);
        return new Vector3(x, y, z);
    }

    public static Vector3 RandomTangent(Vector3 vector)
    {
        var normal = vector.normalized;
        var tangent = Vector3.Cross(normal, new Vector3(-normal.z, normal.x, normal.y));
        var bitangent = Vector3.Cross(normal, tangent);
        var angle = Random.Range(-Mathf.PI, Mathf.PI);
        return tangent * Mathf.Sin(angle) + bitangent * Mathf.Cos(angle);
    }
    public static Vector3 RandomCircle(Vector3 center, float radius) //gets a random point from a sphere around center
    {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }

    public static Vector3 PositionAlignment(GameObject otherObject, Transform transform)
    {
        Vector3 otherForce = transform.position - otherObject.transform.position;

        //float forceScalar = Vector3.Distance(transform.position, otherObject.transform.position);
        //otherForce = Mathf.Sqrt(forceScalar) * otherForce.normalized;
        return otherForce;
    }
    public static Vector3 RotationalAlignment(GameObject otherObject, Transform transform)
    {
        Quaternion AngleDifference = Quaternion.FromToRotation(otherObject.transform.up, transform.up);

        float AngleToCorrect = Quaternion.Angle(transform.rotation, otherObject.transform.rotation);
        Vector3 Perpendicular = Vector3.Cross(transform.up, transform.forward);
        if (Vector3.Dot(otherObject.transform.forward, Perpendicular) < 0)
            AngleToCorrect *= -1;
        Quaternion Correction = Quaternion.AngleAxis(AngleToCorrect, transform.up);

        Vector3 MainRotation = RectifyAngleDifference((AngleDifference).eulerAngles);
        Vector3 CorrectiveRotation = RectifyAngleDifference((Correction).eulerAngles);

        Vector3 torqueVector = (MainRotation - CorrectiveRotation / 2);
        //float torqueScalar = Vector3.Magnitude(torqueVector);
        //torqueVector = torqueVector.normalized * Mathf.Sqrt(torqueScalar);
        return torqueVector;
    }
    public static Vector3 RectifyAngleDifference(Vector3 angdiff)
    {
        if (angdiff.x > 180) angdiff.x -= 360;
        if (angdiff.y > 180) angdiff.y -= 360;
        if (angdiff.z > 180) angdiff.z -= 360;
        return angdiff;
    }

    public static void Align(GameObject rootObject, GameObject objectToAttract, Transform desiredTransform, float attractionFactor)
    {
        Vector3 force = PositionAlignment(objectToAttract, desiredTransform) * attractionFactor;
        Vector3 torque = RotationalAlignment(objectToAttract, desiredTransform);

        Rigidbody otherRigidBody = objectToAttract.GetComponent<Rigidbody>();
        otherRigidBody.AddForce(force, ForceMode.Force);
        otherRigidBody.AddTorque(torque, ForceMode.Force);

        Rigidbody rootRigidBody = rootObject.GetComponent<Rigidbody>();
        rootRigidBody.AddForce(force * -1, ForceMode.Force);
        rootRigidBody.AddTorque(torque * -1, ForceMode.Force);
    }
}
