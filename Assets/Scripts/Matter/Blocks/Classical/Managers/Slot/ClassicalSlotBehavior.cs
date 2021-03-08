using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicalSlotBehavior : SlotBehavior
{
    public ClassicalBlockBehavior OccupantBlock;
    public ClassicalSlotManagerBehavior slotManager;

    ClassicalSlotBehavior otherSlot;
    LineRenderer lineRenderer;

    public override BlockBehavior GetOccupantBlock()
    {
        return OccupantBlock;
    }

    protected override bool OccupantCheck(Collider other)
    {
        return OccupantBlock != null && OccupantBlock.gameObject == other.gameObject;
    }

    void Start()
    {
        slotCollider = gameObject.AddComponent<BoxCollider>();
        slotCollider.isTrigger = true;
        SetUpLineRenderer();
        OccupantUpdate();
    }

    void Update()
    {
        LineUpdate();
    }

    void StartOccupying(ClassicalSlotBehavior otherSlot)
    {
        if(this.otherSlot != otherSlot)
        {
            OccupantBlock = otherSlot.slotManager.Block;
            this.otherSlot = otherSlot;

            otherSlot.OccupantBlock = slotManager.Block;
            otherSlot.otherSlot = this;
        }
    }

    protected override void StopOccupying()
    {
        if(IsOccupying())
        {
            ClassicalSlotBehavior removedSlot = otherSlot;
            OccupantBlock = null;
            fixedJoint = null;
            otherSlot = null;
            OccupantUpdate();

            removedSlot.OccupantBlock = null;
            removedSlot.fixedJoint = null;
            removedSlot.otherSlot = null;
            removedSlot.OccupantUpdate();
        }
    }


    protected override void OnTriggerStay(Collider other)
    {
        if (!IsOccupied() && BlockUtils.IsBlock(other.gameObject) && slotManager.slotLockEnabled) //Check if position doesn't equal the same and a connection hasn't been made
        {
            if (!HasOccupantBlock())
            {
                ClassicalSlotBehavior otherSlot = GetOtherSlot(other.gameObject);
                bool validOtherBlock = !otherSlot.slotManager.Cluster.IsOccupying() && !otherSlot.HasOccupantBlock();
                bool validThisBlock = !slotManager.Cluster.IsOccupying() && !HasOccupantBlock();

                if (validThisBlock && validOtherBlock)
                {
                    StartOccupying(otherSlot);
                }
                else
                {
                    //TODO repulse if not attract
                }
            }
            else if (OccupantCheck(other)) //Check if position doesn't equal the same and a connection hasn't been made
            {
                if (!IsOccupied())
                {
                    if(OccupantAlignedCheck())
                    {
                        LockOccupant();
                    }
                    else
                    {
                        Vector3Utils.Align(rootObject: slotManager.Block.gameObject, objectToAttract: other.gameObject, desiredTransform: transform, attractionFactor: slotManager.attractionFactor);
                    }
                }
            }
        }
    }

    protected override bool OccupantAlignedCheck()
    {
        GameObject thisBlock = slotManager.Block.gameObject;
        GameObject otherBlock = OccupantBlock.gameObject; //Other collider is actually the rigidbody collider attached to the object

        float threshold = 0.05f;
        bool samePosition = Vector3Utils.V3Equal(thisBlock.transform.InverseTransformPoint(otherBlock.transform.position), RelativeLocalPosition, threshold);
        bool sameVelocity = Vector3Utils.V3Equal(thisBlock.GetComponent<Rigidbody>().velocity, otherBlock.GetComponent<Rigidbody>().velocity, threshold);
        bool sameDirection = Vector3Utils.V3Equal(thisBlock.GetComponent<Rigidbody>().transform.forward, otherBlock.GetComponent<Rigidbody>().transform.forward, threshold);

        if (samePosition && sameVelocity && sameDirection)
        {
            return true;
        }
        return false;
    }

    protected override void LockOccupant()
    {
        GameObject thisBlock = slotManager.Block.gameObject;
        GameObject otherBlock = OccupantBlock.gameObject;

        otherBlock.transform.eulerAngles = thisBlock.transform.eulerAngles;
        otherBlock.transform.position = thisBlock.transform.TransformPoint(RelativeLocalPosition);

        fixedJoint = thisBlock.AddComponent<FixedJoint>();
        fixedJoint.enableCollision = false;
        fixedJoint.connectedBody = otherBlock.GetComponent<Rigidbody>();
        otherSlot.fixedJoint = fixedJoint;

        otherSlot.OccupantUpdate();
        OccupantUpdate();

        slotManager.Cluster.AddBlock(OccupantBlock);
    }

    public override void ReleaseBlock()
    {
        if (IsOccupied())
        {
            if (fixedJoint != null)
            {
                Destroy(fixedJoint);
                StopOccupying();
            }
        }
    }

    //State Utils
    public override bool HasOccupantBlock()
    {
        return OccupantBlock != null;
    }

    public ClassicalSlotBehavior GetOtherSlot(GameObject other)
    {
        ClassicalBlockBehavior otherBlockBehavior = other.GetComponent<ClassicalBlockBehavior>();
        Vector3 otherBlockRelativeLocalPosition = -1 * RelativeLocalPosition;

        ClassicalSlotBehavior otherBlockSlot = otherBlockBehavior.slotManager.slots[otherBlockRelativeLocalPosition.ToString()];
        return otherBlockSlot;
    }

    //Line Utils
    void SetUpLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.endWidth = 0.03f;
        lineRenderer.startWidth = 0.03f;
        lineRenderer.startColor = Color.grey;
        lineRenderer.endColor = Color.grey;
        lineRenderer.material = slotManager.Block.gameMaster.spawnManager.particleLit;
        lineRenderer.enabled = false;
    }

    void LineUpdate()
    {
        if (HasOccupantBlock())
        {
            lineRenderer.enabled = true;
            AttractLineUpdate();
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    void AttractLineUpdate()
    {
        Vector3[] points = new Vector3[2];
        points[0] = slotManager.Block.transform.position;
        points[1] = OccupantBlock.transform.position;
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

}
