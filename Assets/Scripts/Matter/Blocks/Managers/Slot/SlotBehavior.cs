using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 RelativeLocalPosition;
    public ElectronPosition[] connectingElectronPositions;

    public FixedJoint fixedJoint;
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
        if(this.otherSlot != otherSlot)
        {
            OccupantBlock = otherSlot.slotManager.block;
            this.otherSlot = otherSlot;
            Entangle(otherSlot);
            otherSlot.StartOccupying(this);
        }
    }


    void StopOccupying()
    {
        if(IsOccupying())
        {
            SlotBehavior removedSlot = otherSlot;
            Untangle(otherSlot);
            OccupantBlock = null;
            fixedJoint = null;
            otherSlot = null;
            OccupantUpdate();
            removedSlot.StopOccupying();
        }
    }


    void OnTriggerStay(Collider other)
    {
        if (!IsOccupied() && other.gameObject.CompareTag("Block")) //Check if position doesn't equal the same and a connection hasn't been made
        {
            if (!HasOccupantBlock())
            {
                SlotBehavior otherSlot = GetOtherSlot(other.gameObject);
                bool validOtherBlock = !otherSlot.slotManager.cluster.IsOccupying() && !otherSlot.HasOccupantBlock();
                bool validThisBlock = !slotManager.cluster.IsOccupying() && !HasOccupantBlock();

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
                else if (!IsOccupied())
                {
                    if(OccupantAlignedCheck())
                    {
                        LockOccupant();
                    }
                    else
                    {
                        Vector3Utils.Align(rootObject: slotManager.block.gameObject, objectToAttract: other.gameObject, desiredTransform: transform, attractionFactor: slotManager.attractionFactor);
                    }
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

    bool OccupantAlignedCheck()
    {
        GameObject thisBlock = slotManager.block.gameObject;
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

    void LockOccupant()
    {
        GameObject thisBlock = slotManager.block.gameObject;
        GameObject otherBlock = OccupantBlock.gameObject;

        otherBlock.transform.eulerAngles = thisBlock.transform.eulerAngles;
        otherBlock.transform.position = thisBlock.transform.TransformPoint(RelativeLocalPosition);

        fixedJoint = thisBlock.AddComponent<FixedJoint>();
        fixedJoint.enableCollision = false;
        fixedJoint.connectedBody = otherBlock.GetComponent<Rigidbody>();
        otherSlot.fixedJoint = fixedJoint;

        otherSlot.OccupantUpdate();
        OccupantUpdate();

        AddJointToElectrons();
        slotManager.cluster.AddBlock(OccupantBlock);
    }


    public void ReleaseBlock()
    {
        if(IsOccupied())
        {
            if(fixedJoint != null)
            {
                for (int i = 0; i < connectingElectronPositions.Length; i++)
                {
                    connectingElectronPositions[i].ReleaseElectron();
                }
                Destroy(fixedJoint);
                StopOccupying();
            }
        }
    }

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
    public bool HasOccupantBlock()
    {
        return OccupantBlock != null;
    }

    //Electron Utils
    void Entangle(SlotBehavior otherSlot)
    {
        for (int i = 0; i < connectingElectronPositions.Length; i++)
        {
            Vector3 relativePosition = GetRelativeElectronPosition(connectingElectronPositions[i]);
            ElectronPosition otherElectronPosition = otherSlot.slotManager.block.electronManager.electronPositionDictionary[relativePosition.ToString()];
            connectingElectronPositions[i].Entangle(otherElectronPosition);
        }
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

    Vector3 GetRelativeElectronPosition(ElectronPosition electronPosition)
    {
        return electronPosition.position - RelativeLocalPosition;
    }

    public bool ValidElectrons(SlotBehavior otherSlot)
    {
        bool validElectrons; //electron check; check for interfering electrons, and check if there are any available electron for attracting
        if (GetOccupiedElectronPositions().Count == 0 && otherSlot.GetOccupiedElectronPositions().Count == 0)
        {
            validElectrons = false;
        }
        else
        {
            validElectrons = !HasInterferingElectrons(otherSlot);
        }
        return validElectrons;
    }

    public void AddJointToElectrons()
    {
        for (int i = 0; i < connectingElectronPositions.Length; i++)
        {
            connectingElectronPositions[i].AddJoint(fixedJoint, this);
        }
    }

    public void ReleaseElectrons()
    {
        for(int i=0; i < connectingElectronPositions.Length; i++)
        {
            connectingElectronPositions[i].ReleaseElectron();
        }    
    }

    public List<ElectronPosition> GetOccupiedElectronPositions()
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

    public bool HasInterferingElectrons(SlotBehavior otherSlot)
    {
        for (int i = 0; i < connectingElectronPositions.Length; i++)
        {
            Vector3 relativePosition = GetRelativeElectronPosition(connectingElectronPositions[i]);
            ElectronPosition otherElectronPosition = otherSlot.slotManager.block.electronManager.electronPositionDictionary[relativePosition.ToString()];
            if(otherElectronPosition.electron != null && connectingElectronPositions[i].electron != null && connectingElectronPositions[i].electron != otherElectronPosition.electron)
            {
                return true;
            }
        }
        return false;
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
            lineRenderer.enabled = true;
            //OccupiedElectronsLine();
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

    void OccupiedElectronsLine()
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < connectingElectronPositions.Length; i++)
        {
            if(connectingElectronPositions[i].electron != null)
            {
                points.Add(slotManager.block.transform.TransformPoint(connectingElectronPositions[i].position));
            }
        }
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }


}
