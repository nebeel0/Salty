using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterMessageBehavior : MonoBehaviour
{
    //A sphere collider that collects messages
    //Base message is similar to gravity

    ClusterBehavior clusterBehavior;
    SphereCollider messageSphere;

    void Start()
    {
        clusterBehavior = GetComponent<ClusterBehavior>();
        SetUpColliders();
    }

    void Update()
    {
        messageSphere.radius = Mathf.Pow(2 * Vector3Utils.GetRadiusFromVolume(clusterBehavior.totalMass), 2);
    }

    void SetUpColliders()
    {
        messageSphere = gameObject.AddComponent<SphereCollider>();
        messageSphere.isTrigger = true;
        messageSphere.radius = 2; //TODO pull information from cluster behavior
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Block"))
        {
            if(other.transform.parent == transform)
            {
                Physics.IgnoreCollision(messageSphere, other);
            }
            else
            {
                SlotManagerBehavior otherBlockSlotManagerBehavior = other.GetComponent<BlockBehavior>().slotManager;
                if (!clusterBehavior.blocks.Contains(other.gameObject.GetComponent<BlockBehavior>()) && !otherBlockSlotManagerBehavior.IsOccupying())
                {
                    if (otherBlockSlotManagerBehavior.ParentCluster != null && !otherBlockSlotManagerBehavior.ParentCluster.IsOccupying() && !clusterBehavior.IsOccupying())
                    {
                        Attract(other);
                    }
                }
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
            Vector3 otherObjectDirection = otherObject.transform.forward;
        }
        catch
        {

        }
    }

}
