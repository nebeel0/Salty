using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMessagesBehavior : MatterBehavior
{
    BlockConnectionBehavior blockConnectionBehavior
    {
        get { return GetComponent<BlockConnectionBehavior>(); }
    }

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        SetUpColliders();
    }

    protected override void RefreshState()
    {
    }

    void SetUpColliders()
    {
        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = 2 * scalingFactor;
    }

    protected override void DeathCheck()
    {
    }

    void OnTriggerEnter(Collider other)
    {

    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Block"))
        {
            //Attract(other);
            //BlockConnectionBehavior otherBlockConnectionBehavior = other.GetComponent<BlockConnectionBehavior>();
            //if (blockConnectionBehavior.ParentCluster != otherBlockConnectionBehavior.ParentCluster)
            //{
            //}
        }
    }

    void OnTriggerExit(Collider other)
    {
    }

    void Attract(Collider other)
    {
        try
        {
            SphereCollider collider = (SphereCollider)other;
            //Equal and opposite reactions
            GameObject otherObject = collider.gameObject;
            Rigidbody otherRigidBody = otherObject.GetComponent<Rigidbody>();
            Vector3 otherForce = transform.position - otherObject.transform.position;

            float forceScalar = Vector3.Distance(transform.position, otherObject.transform.position);
            forceScalar = 10 * (1 / (forceScalar + 0.001f)) * (1 / (forceScalar + 0.001f));

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
