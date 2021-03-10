using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Collections;
using Matter.Block.Base;

public class ClassicalSlotManagerBehavior : SlotManagerBehavior
{
    public Dictionary<string, ClassicalSlotBehavior> slots = new Dictionary<string, ClassicalSlotBehavior>();

    public ClassicalBlockBehavior Block
    {
        get
        {
            if (transform.parent != null && BlockUtils.IsBlock(transform.parent.gameObject))
            {
                return transform.parent.gameObject.GetComponent<ClassicalBlockBehavior>();
            }
            return null;
        }
    }

    public override Dictionary<string, SlotBehavior> GetSlots()
    {
        Dictionary<string, SlotBehavior> baseSlots = CodingUtils.CastDict(slots).ToDictionary(entry => (string) entry.Key, entry => (SlotBehavior) entry.Value);
        return baseSlots;
    }

    public override bool IsOccupying()
    {
        foreach(ClassicalSlotBehavior slot in slots.Values)
        {
            if(slot.IsOccupying())
            {
                return true;
            }
        }
        return false;
    }

    public override void Start()
    {
        base.Start();
        if(slots.Count == 0)
        {
            SetUpSlots();
        }
    }

    protected override void SetUpSlots()
    {
        //TODO cache all setups
        //Note this might not be able to be cached without accidently sharing items
        if(slots.Count == 0)
        {
            for (int i = 1; i <= 2; i++)
            {
                for (int ii = 0; ii < 3; ii++)
                {
                    Vector3 newSlotPosition = Vector3.zero;
                    newSlotPosition[ii] = Mathf.Pow(-1, i) * transform.localScale[ii] * displacementFactor;
                    GameObject newSlot = new GameObject();
                    newSlot.layer = PhysicsManager.messageColliderLayer; //message colliders
                    newSlot.name = string.Format("Box Slot - {0}", newSlotPosition.ToString());
                    newSlot.transform.parent = gameObject.transform;
                    newSlot.transform.localPosition = newSlotPosition;
                    newSlot.transform.localEulerAngles = Vector3.zero;
                    ClassicalSlotBehavior newClassicalSlotBehavior = newSlot.AddComponent<ClassicalSlotBehavior>();
                    newClassicalSlotBehavior.slotManager = this;
                    newClassicalSlotBehavior.RelativeLocalPosition = newSlotPosition;
                    slots[newSlotPosition.ToString()] = newClassicalSlotBehavior;
                }
            }
        }
    }

    public override void OccupantsUpdate()
    {
        foreach(ClassicalSlotBehavior slot in slots.Values)
        {
            slot.OccupantUpdate();
        }
    }

    public override void ReleaseBlocks()
    {
        foreach (ClassicalSlotBehavior slot in slots.Values)
        {
            slot.ReleaseBlock();
        }
    }

}
