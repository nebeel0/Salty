using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlotBehavior : MonoBehaviour
{
    public Vector3 RelativeLocalPosition;
    public FixedJoint fixedJoint;
    public BoxCollider slotCollider;

    public abstract BlockBehavior GetOccupantBlock();

    protected abstract bool OccupantCheck(Collider other);

    public void OccupantUpdate()
    {
        if (IsOccupied() && slotCollider.enabled)
        {
            slotCollider.enabled = false;
        }
        else if (!slotCollider.enabled && !IsOccupied())
        {
            slotCollider.enabled = true;
            StopOccupying();
        }
    }

    protected abstract void StopOccupying();
    protected abstract void OnTriggerStay(Collider other);

    protected void OnTriggerExit(Collider other)
    {
        if (BlockUtils.IsBlock(other.gameObject) && OccupantCheck(other))
        {
            StopOccupying();
        }
    }

    protected abstract bool OccupantAlignedCheck();

    protected abstract void LockOccupant();

    public abstract void ReleaseBlock();

    //State Utils
    public bool IsFixed()
    {
        return fixedJoint != null;
    }
    public bool IsOccupied()
    {
        return HasOccupantBlock() && IsFixed();
    }
    public bool IsOccupying()
    {
        return HasOccupantBlock() && !IsFixed(); //Returns true if occupant block is true, but no fixed joint, false otherwise 
    }
    public abstract bool HasOccupantBlock();

}
