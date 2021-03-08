using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class ClassicalBlockBehavior : BlockBehavior
{
    public ClassicalSlotManagerBehavior slotManager;
    public override SlotManagerBehavior GetSlotManager()
    {
        return slotManager;
    }

    protected override void SetUpManagers()
    {
        if (slotManager == null)
        {
            slotManager = new GameObject().AddComponent<ClassicalSlotManagerBehavior>();
            Vector3Utils.NeutralParent(parent: transform, child: slotManager.transform);
        }
        slotManager.Start();
    }

    public override void Death()
    {
        slotManager.Death();
        base.Death();
    }

    public override bool DeathCheck()
    {
        return false;
    }
}
