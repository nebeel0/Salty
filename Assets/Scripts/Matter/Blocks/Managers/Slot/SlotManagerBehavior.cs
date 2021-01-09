using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SlotManagerBehavior : BlockManagerBehavior
{
    public float attractionFactor = 10;
    public float displacementFactor = 1.1f;
    public Dictionary<string, SlotBehavior> slots = new Dictionary<string, SlotBehavior>();

    int numFaceVertices = 4; //TODO store in Vector3Utils and determine faceVertices from type of shape.

    public bool IsOccupying()
    {
        foreach(SlotBehavior slot in slots.Values)
        {
            if(slot.IsOccupying())
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
            if (block != null)
            {
                return block.ParentCluster;
            }
            return null;
        }
    }

    public void Start()
    {
        if(slots.Count == 0)
        {
            SetUpSlots();
        }
    }

    void SetUpSlots()
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
                    newSlot.layer = GameMaster.MessageColliderLayer; //message colliders
                    newSlot.name = string.Format("Box Slot - {0}", newSlotPosition.ToString());
                    newSlot.transform.parent = gameObject.transform;
                    newSlot.transform.localPosition = newSlotPosition;
                    newSlot.transform.localEulerAngles = Vector3.zero;
                    SlotBehavior newBlockSlotBehavior = newSlot.AddComponent<SlotBehavior>();
                    newBlockSlotBehavior.slotManager = this;
                    newBlockSlotBehavior.RelativeLocalPosition = newSlotPosition;
                    newBlockSlotBehavior.connectingElectronPositions = GetConnectingElectronPositions(newSlotPosition);
                    slots[newSlotPosition.ToString()] = newBlockSlotBehavior;
                }
            }
        }
    }

    public ElectronPosition[] GetConnectingElectronPositions(Vector3 position)
    {
        //TODO cache this as well.
        block.electronManager.Start();
        ElectronPosition[] allElectronPositions = block.electronManager.electronPositions;
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

    public void OccupantsUpdate()
    {
        foreach(SlotBehavior slot in slots.Values)
        {
            slot.OccupantUpdate();
        }
    }

    public void Death()
    {
        foreach (SlotBehavior slot in slots.Values)
        {
            slot.ReleaseAllElectrons();
        }
        Destroy(this);
    }

}
