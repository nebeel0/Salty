using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 RelativeLocalPosition;
    public ElectronPosition[] connectingElectronPositions;

    public BlockBehavior OccupantBlock;
    public BoxCollider slotCollider;
    public SlotManagerBehavior slotManager;

    SlotBehavior otherSlot;
    LineRenderer lineRenderer;

    bool OccupantCheck(Collider other)
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

    void StartOccupying(SlotBehavior otherSlot)
    {
        OccupantBlock = otherSlot.slotManager.block;
        this.otherSlot = otherSlot;

        otherSlot.otherSlot = this;
        otherSlot.OccupantBlock = slotManager.block;
        Entangle(otherSlot);
    }
    void Entangle(SlotBehavior otherSlot)
    {
        for(int i = 0; i < connectingElectronPositions.Length; i++)
        {
            Vector3 relativePosition = GetRelativeElectronPosition(connectingElectronPositions[i]);
            ElectronPosition otherElectronPosition = otherSlot.slotManager.block.electronManager.electronPositionDictionary[relativePosition.ToString()];
            connectingElectronPositions[i].Entangle(otherElectronPosition);
        }
    }

    void StopOccupying()
    {
        OccupantBlock = null;
        otherSlot.OccupantBlock = null;

        Untangle(otherSlot);

        otherSlot.otherSlot = null;
        this.otherSlot = null;
    }
    void Untangle(SlotBehavior otherSlot)
    {
        for (int i = 0; i < connectingElectronPositions.Length; i++)
        {
            Vector3 relativePosition = GetRelativeElectronPosition(connectingElectronPositions[i]);
            ElectronPosition otherElectronPosition = otherSlot.slotManager.block.electronManager.electronPositionDictionary[relativePosition.ToString()];
            connectingElectronPositions[i].Untangle(otherElectronPosition);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!IsOccupied() && other.gameObject.CompareTag("Block")) //Check if position doesn't equal the same and a connection hasn't been made
        {
            if (!HasOccupantBlock())
            {
                SlotBehavior otherSlot = GetOtherSlot(other.gameObject);
                bool validOtherBlock = !otherSlot.slotManager.ParentCluster.IsOccupying() && !HasOccupantBlock();
                bool validThisBlock = !slotManager.ParentCluster.IsOccupying() && !HasOccupantBlock();

                if (validThisBlock && validOtherBlock && ValidElectrons(otherSlot))
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
                if (!ValidElectrons(otherSlot))
                {
                    StopOccupying();
                }
                else if (!IsFixed() && !Snap(other))
                {
                    Vector3Utils.Attract(rootObject: slotManager.block.gameObject, objectToAttract: other.gameObject, desiredTransform: transform, attractionFactor: slotManager.attractionFactor);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Block") && OccupantCheck(other))
        {
            StopOccupying();
        }
    }

    bool Snap(Collider other)
    {
        GameObject thisBlock = slotManager.block.gameObject;
        GameObject otherBlock = other.transform.gameObject; //Other collider is actually the rigidbody collider attached to the object

        bool samePosition = Vector3Utils.V3Equal(thisBlock.transform.InverseTransformPoint(otherBlock.transform.position), RelativeLocalPosition, 0.001f);
        bool sameVelocity = Vector3Utils.V3Equal(thisBlock.GetComponent<Rigidbody>().velocity, otherBlock.GetComponent<Rigidbody>().velocity, 0.001f);
        bool sameDirection = Vector3Utils.V3Equal(thisBlock.GetComponent<Rigidbody>().transform.forward, otherBlock.GetComponent<Rigidbody>().transform.forward, 0.001f);

        if (samePosition && sameVelocity && sameDirection)
        {
            otherBlock.transform.eulerAngles = thisBlock.transform.eulerAngles;
            otherBlock.transform.position = thisBlock.transform.TransformPoint(RelativeLocalPosition);
            if (GetConnectingElectrons().Count == 0 && otherSlot.GetConnectingElectrons().Count == 0)
            {
                ElectronBehavior unconnectedElectron = slotManager.block.electronManager.GetUnconnectedElectron();
                unconnectedElectron = unconnectedElectron == null ? otherSlot.slotManager.block.electronManager.GetUnconnectedElectron() : unconnectedElectron;
                if(unconnectedElectron != null)
                {
                    connectingElectronPositions[0].SetElectron(unconnectedElectron);
                }
            }

            List<ElectronPosition> existingElectronPositions = GetConnectingElectrons();
            for(int i =0; i < existingElectronPositions.Count; i++)
            {
                existingElectronPositions[i].ConnectBlock(slotManager.block);
                existingElectronPositions[i].ConnectBlock(OccupantBlock);
            }
            otherSlot.OccupantUpdate();
            OccupantUpdate();
            slotManager.ParentCluster.AddBlock(OccupantBlock);
            return true;
        }
        return false;
    }

    //State Utils

    public bool IsFixed()
    {
        for (int i = 0; i < connectingElectronPositions.Length; i++)
        {
            if(connectingElectronPositions[i].electron != null)
            {
                bool containsOtherBlockJoint = connectingElectronPositions[i].electron.connectedBlocks.ContainsKey(OccupantBlock);
                bool containsRootBlockJoint = connectingElectronPositions[i].electron.connectedBlocks.ContainsKey(slotManager.block);
                if (containsOtherBlockJoint && containsRootBlockJoint)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool IsOccupied()
    {
        return HasOccupantBlock() && IsFixed();
    }
    public bool IsOccupying()
    {
        return HasOccupantBlock() && !IsFixed(); //Returns true if occupant block is true, but no fixed joint, false otherwise 
    }
    public bool HasOccupantBlock()
    {
        return OccupantBlock != null;
    }

    //Electron Utils

    Vector3 GetRelativeElectronPosition(ElectronPosition electronPosition)
    {
        return electronPosition.position - RelativeLocalPosition;
    }

    public bool ValidElectrons(SlotBehavior otherSlot)
    {
        bool validElectrons; //electron check; check for interfering electrons, and check if there are any available electron for attracting
        if (GetConnectingElectrons().Count == 0 && otherSlot.GetConnectingElectrons().Count == 0)
        {
            validElectrons = otherSlot.slotManager.block.electronManager.GetUnconnectedElectron() != null || slotManager.block.electronManager.GetUnconnectedElectron() != null;
        }
        else
        {
            validElectrons = !HasInterferingElectrons(otherSlot);
        }
        return validElectrons;
    }

    public void ReleaseAllElectrons()
    {
        for(int i=0; i < connectingElectronPositions.Length; i++)
        {
            connectingElectronPositions[i].ReleaseElectron();
        }    
    }

    public List<ElectronPosition> GetConnectingElectrons()
    {
        List<ElectronPosition> electronPositions = new List<ElectronPosition>();
        for (int i = 0; i < connectingElectronPositions.Length; i++)
        {
            if (connectingElectronPositions[i].electron != null)
            {
                electronPositions.Add(connectingElectronPositions[i]);
            }
        }
        return electronPositions;
    }

    public bool HasInterferingElectrons(SlotBehavior otherBlockSlot)
    {
        bool hasInterferingElectrons = false;
        List<ElectronPosition> existingElectronPositions = GetConnectingElectrons();
        for (int i = 0; i < existingElectronPositions.Count; i++)
        {
            Vector3 relativePosition = GetRelativeElectronPosition(existingElectronPositions[i]);
            ElectronPosition otherElectronPosition = otherBlockSlot.slotManager.block.electronManager.electronPositionDictionary[relativePosition.ToString()];
            if(otherElectronPosition.electron != null && existingElectronPositions[i].electron != null && existingElectronPositions[i].electron != otherElectronPosition.electron)
            {
                hasInterferingElectrons = true;
            }
        }
        return hasInterferingElectrons;
    }

    public SlotBehavior GetOtherSlot(GameObject other)
    {
        BlockBehavior otherBlockBehavior = other.GetComponent<BlockBehavior>();
        Vector3 otherBlockRelativeLocalPosition = -1 * RelativeLocalPosition;

        SlotBehavior otherBlockSlot = otherBlockBehavior.slotManager.slots[otherBlockRelativeLocalPosition.ToString()];
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
        lineRenderer.material = slotManager.gameMaster.particleLit;
        lineRenderer.enabled = false;
    }

    void LineUpdate()
    {
        if (HasOccupantBlock())
        {
            //lineRenderer.enabled = true;
            //ElectronLineUpdate();
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    void AttractLineUpdate()
    {
        Vector3[] points = new Vector3[2];
        points[0] = slotManager.block.transform.position;
        points[1] = OccupantBlock.transform.position;
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

    void ElectronLineUpdate()
    {
        Vector3[] points = new Vector3[4];
        for(int i=0; i < points.Length; i++)
        {
            points[i] = slotManager.block.transform.TransformPoint(connectingElectronPositions[i].position);
        }
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }


}
