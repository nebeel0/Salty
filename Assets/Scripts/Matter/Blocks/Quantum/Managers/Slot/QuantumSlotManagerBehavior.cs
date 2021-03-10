using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Collections;
using Matter.Block.Base;

public class QuantumSlotManagerBehavior : SlotManagerBehavior
{
    public QuantumBlockBehavior Block
    {
        get
        {
            if (transform.parent != null && BlockUtils.IsBlock(transform.parent.gameObject))
            {
                return transform.parent.gameObject.GetComponent<QuantumBlockBehavior>();
            }
            return null;
        }
    }

    public Dictionary<string, QuantumSlotBehavior> slots = new Dictionary<string, QuantumSlotBehavior>();
    public override Dictionary<string, SlotBehavior> GetSlots()
    {
        Dictionary<string, SlotBehavior> baseSlots = CodingUtils.CastDict(slots).ToDictionary(entry => (string)entry.Key, entry => (SlotBehavior)entry.Value);
        return baseSlots;
    }

    public override bool IsOccupying()
    {
        foreach(QuantumSlotBehavior slot in slots.Values)
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
                    QuantumSlotBehavior newBlockQuantumSlotBehavior = newSlot.AddComponent<QuantumSlotBehavior>();
                    newBlockQuantumSlotBehavior.slotManager = this;
                    newBlockQuantumSlotBehavior.RelativeLocalPosition = newSlotPosition;
                    newBlockQuantumSlotBehavior.connectingElectronPositions = GetConnectingElectronPositions(newSlotPosition);
                    slots[newSlotPosition.ToString()] = newBlockQuantumSlotBehavior;
                }
            }
        }
    }

    public ElectronPosition[] GetConnectingElectronPositions(Vector3 position)
    {
        //TODO cache this as well.
        Block.electronManager.Start();
        ElectronPosition[] allElectronPositions = Block.electronManager.electronPositions;
        Dictionary<int, List<ElectronPosition>> electronPositionsDictionary = new Dictionary<int, List<ElectronPosition>>();

        int closestDistance = -1;

        for (int i=0; i< allElectronPositions.Length; i++)
        {
            int distance = (int) Vector3.Distance(allElectronPositions[i].position, position);
            if (closestDistance == -1 || distance < closestDistance)
            {
                closestDistance = distance;
            }
            if (!electronPositionsDictionary.ContainsKey(distance))
            {
                electronPositionsDictionary[distance] = new List<ElectronPosition>();
            }
            electronPositionsDictionary[distance].Add(allElectronPositions[i]);
        }

        if(electronPositionsDictionary[closestDistance].Count != numFaceVertices)
        {
            Debug.LogError("Scale is too small to get int based distances.");
        }

        return electronPositionsDictionary[closestDistance].ToArray();
    }

    public override void OccupantsUpdate()
    {
        foreach(QuantumSlotBehavior slot in slots.Values)
        {
            slot.OccupantUpdate();
        }
    }

    public override void ReleaseBlocks()
    {
        foreach (QuantumSlotBehavior slot in slots.Values)
        {
            slot.ReleaseBlock();
        }
    }
}
