using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSlotBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 RelativeLocalPosition;

    public bool FullyConnected
    {
        get
        {
            bool hasOccupantBlock = OccupantBlock != null;
            bool hasFixedJoint = BlockFixedJoint != null;
            return hasOccupantBlock && hasFixedJoint; //Returns true if occupant block is true, but no fixed joint, false otherwise 
        }
    }

    public bool IsOccupying
    {
        get
        {
            bool hasOccupantBlock = OccupantBlock != null;
            bool hasFixedJoint = BlockFixedJoint != null;
            return hasOccupantBlock && !hasFixedJoint; //Returns true if occupant block is true, but no fixed joint, false otherwise 
        }
    }
    public GameObject OccupantBlock;

    BlockSlotManagerBehavior ParentBlockSlotManager
    {
        get
        {
            return transform.parent.gameObject.GetComponent<BlockSlotManagerBehavior>();
        }
    }
    FixedJoint BlockFixedJoint = null;
    void Start()
    {
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }
    void Update()
    {
        GarbageCleanUp();
    }
    void GarbageCleanUp()
    {
        if (BlockFixedJoint != null && OccupantBlock == null)
        {
            Destroy(BlockFixedJoint);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Block")) //Check if position doesn't equal the same and a connection hasn't been made
        {
            //TODO check if other block is only connecting to you.

        }
    }


    void OnTriggerStay(Collider other)
    {
        try
        {
            if (other.gameObject.CompareTag("Block")) //Check if position doesn't equal the same and a connection hasn't been made
            {

                BlockSlotManagerBehavior otherSlotManager = other.gameObject.GetComponent<BlockSlotManagerBehavior>();
                bool validOtherBlock = !otherSlotManager.ParentCluster.IsOccupying() || !otherSlotManager.IsOccupying() || (otherSlotManager.IsOccupying() && otherSlotManager.InSlot(ParentBlockSlotManager));

                if (!ParentBlockSlotManager.ParentCluster.IsOccupying() && !ParentBlockSlotManager.IsOccupying() && validOtherBlock && OccupantBlock == null)
                {
                    OccupantBlock = other.gameObject;
                }

                if (!validOtherBlock && !FullyConnected)
                {
                    OccupantBlock = null;
                }

                if (OccupantBlock == other.gameObject) //Check if position doesn't equal the same and a connection hasn't been made
                {
                    if (BlockFixedJoint == null && !Snap(other))
                    {
                        Attract(other);
                    }
                }
            }
        }
        catch { }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Block") && OccupantBlock == other.gameObject)
        {
            OccupantBlock = null;
        }
    }

    bool Snap(Collider other)
    {
        GameObject thisBlock = ParentBlockSlotManager.gameObject;
        GameObject otherBlock = other.transform.gameObject;

        bool samePosition = Vector3Utils.V3Equal(thisBlock.transform.InverseTransformPoint(otherBlock.transform.position), RelativeLocalPosition, 0.001f);
        bool sameVelocity = Vector3Utils.V3Equal(thisBlock.GetComponent<Rigidbody>().velocity, otherBlock.GetComponent<Rigidbody>().velocity, 0.001f);

        if (samePosition && sameVelocity)
        {
            other.transform.eulerAngles = thisBlock.transform.eulerAngles;
            other.transform.position = thisBlock.transform.TransformPoint(RelativeLocalPosition);

            BlockFixedJoint = thisBlock.AddComponent<FixedJoint>();
            BlockFixedJoint.enableCollision = false;
            BlockFixedJoint.connectedBody = other.GetComponent<Rigidbody>();

            Vector3 otherBlockRelativeLocalPosition = otherBlock.transform.InverseTransformPoint(thisBlock.transform.position);
            BlockSlotBehavior otherBlockSlot = otherBlock.GetComponent<BlockSlotManagerBehavior>().slots[otherBlockRelativeLocalPosition.ToString()];

            otherBlockSlot.BlockFixedJoint = BlockFixedJoint;
            return true;
        }
        return false;
    }
    void Attract(Collider other)
    {
        try
        {
            PositionAlignment(other);
            RotationalAlignment(other);
        }
        catch
        {

        }
    }
    void PositionAlignment(Collider other)
    {
        GameObject otherObject = other.gameObject;
        Rigidbody otherRigidBody = otherObject.GetComponent<Rigidbody>();
        Vector3 otherForce = transform.position - otherObject.transform.position;

        float forceScalar = Vector3.Distance(transform.position, otherObject.transform.position);
        otherForce = 10 * forceScalar * otherForce.normalized;
        otherRigidBody.AddForce(otherForce, ForceMode.Force);
    }
    void RotationalAlignment(Collider other)
    {
        Rigidbody ObjectToAttract = other.gameObject.GetComponent<Rigidbody>();
        Quaternion AngleDifference = Quaternion.FromToRotation(ObjectToAttract.transform.up, transform.up);

        float AngleToCorrect = Quaternion.Angle(transform.rotation, ObjectToAttract.transform.rotation);
        Vector3 Perpendicular = Vector3.Cross(transform.up, transform.forward);
        if (Vector3.Dot(ObjectToAttract.transform.forward, Perpendicular) < 0)
            AngleToCorrect *= -1;
        Quaternion Correction = Quaternion.AngleAxis(AngleToCorrect, transform.up);

        Vector3 MainRotation = RectifyAngleDifference((AngleDifference).eulerAngles);
        Vector3 CorrectiveRotation = RectifyAngleDifference((Correction).eulerAngles);
        ObjectToAttract.AddTorque((MainRotation - CorrectiveRotation / 2), ForceMode.Force);
    }
    private Vector3 RectifyAngleDifference(Vector3 angdiff)
    {
        if (angdiff.x > 180) angdiff.x -= 360;
        if (angdiff.y > 180) angdiff.y -= 360;
        if (angdiff.z > 180) angdiff.z -= 360;
        return angdiff;
    }
}
