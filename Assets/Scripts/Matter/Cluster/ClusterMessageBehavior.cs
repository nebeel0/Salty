using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterMessageBehavior : MonoBehaviour
{
    //A sphere collider that collects messages
    //Base message is similar to gravity

    ClusterBehavior clusterBehavior
    {
        get { return GetComponent<ClusterBehavior>(); }
    }
    public SphereCollider messageSphere;

    public void Start()
    {
        SetUpColliders();
    }

    public void UpdateRadius()
    {
        if(clusterBehavior.gameMaster.GravityCheck(clusterBehavior))
        {
            messageSphere.enabled = true;
            messageSphere.radius = Mathf.Pow(2 * Vector3Utils.GetRadiusFromVolume(clusterBehavior.totalMass), 2);
        }
        else
        {
            messageSphere.enabled = false;
        }
    }

    void SetUpColliders()
    {
        if (messageSphere == null)
        {
            messageSphere = gameObject.AddComponent<SphereCollider>();
            messageSphere.isTrigger = true;
            messageSphere.radius = 2; //TODO pull information from cluster behavior
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Block"))
        {
            BlockBehavior otherBlock = other.gameObject.GetComponent<BlockBehavior>();
            if (!otherBlock.cluster.IsOccupying() && !clusterBehavior.IsOccupying() && otherBlock.cluster != clusterBehavior)
            {
                Attract(other);
            }
        }
    }

    void Attract(Collider other)
    {
        try
        {
            //Equal and opposite reactions
            GameObject otherObject = other.gameObject;
            Rigidbody otherRigidBody = otherObject.GetComponent<Rigidbody>();
            Vector3 otherForce = transform.position - otherObject.transform.position;

            float forceScalar = Vector3.Distance(transform.position, otherObject.transform.position);
            forceScalar = 10 * (1 / Mathf.Pow(forceScalar + 0.1f, 2));

            forceScalar = Mathf.Clamp(forceScalar, 0, 10);
            otherForce = forceScalar * otherForce.normalized;

            otherRigidBody.AddForce(otherForce, ForceMode.Force);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

}
