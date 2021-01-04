using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSlotManagerBehavior : BlockManagerBehavior
{
    public float displacementFactor = 1.1f;
    public Dictionary<string, BlockSlotBehavior> slots;
    public bool InSlot(BlockSlotManagerBehavior other) //One way Connection
    {
        foreach(BlockSlotBehavior slot in slots.Values)
        {
            if (slot.OccupantBlock == other.BlockBehavior.gameObject)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsOccupying()
    {
        foreach(BlockSlotBehavior slot in slots.Values)
        {
            if(slot.IsOccupying)
            {
                return true;
            }
        }
        return false;
    }
    public ClusterBehavior ParentCluster
    {
        get
        {
            if (BlockBehavior != null)
            {
                return BlockBehavior.ParentCluster;
            }
            return null;
        }
    }

    void Start()
    {
        SetUpSlots();
    }
    void SetUpSlots()
    {
        slots = new Dictionary<string, BlockSlotBehavior>();
        for (int i = 1; i <= 2; i++)
        {
            for (int ii = 0; ii < 3; ii++)
            {
                Vector3 newSlotPosition = Vector3.zero;
                newSlotPosition[ii] = Mathf.Pow(-1, i) * transform.localScale[ii] * displacementFactor;
                GameObject newSlot = new GameObject();
                newSlot.layer = GameMaster.MessageColliderLayer; //message colliders
                newSlot.name = string.Format("Box Slot - {0}", newSlotPosition.ToString());
                newSlot.transform.parent = gameObject.transform;
                newSlot.transform.localPosition = newSlotPosition;
                newSlot.transform.localEulerAngles = Vector3.zero;
                BlockSlotBehavior newBlockSlotBehavior = newSlot.AddComponent<BlockSlotBehavior>();
                newBlockSlotBehavior.ParentBlockSlotManager = this;
                newBlockSlotBehavior.RelativeLocalPosition = newSlotPosition;
                slots[newSlotPosition.ToString()] = newBlockSlotBehavior;
            }
        }
    }
    // Update is called once per frame

}
