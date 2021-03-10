using Matter.Block.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantumSlotBehavior : SlotBehavior
{
    // Start is called before the first frame update
    public ElectronPosition[] connectingElectronPositions;

    public QuantumBlockBehavior OccupantBlock;
    public QuantumSlotManagerBehavior slotManager;

    QuantumSlotBehavior otherSlot;
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

    void StartOccupying(QuantumSlotBehavior otherSlot)
    {
        if(this.otherSlot != otherSlot)
        {
            OccupantBlock = otherSlot.slotManager.Block;
            this.otherSlot = otherSlot;
            Entangle(otherSlot);

            otherSlot.OccupantBlock = slotManager.Block;
            otherSlot.otherSlot = this;
        }
    }

    protected override void StopOccupying()
    {
        if(IsOccupying())
        {
            QuantumSlotBehavior removedSlot = otherSlot;
            Untangle(otherSlot);
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
                QuantumSlotBehavior otherSlot = GetOtherSlot(other.gameObject);
                if(otherSlot.slotManager.Cluster != null)
                {
                    bool validOtherBlock = !otherSlot.slotManager.Cluster.IsOccupying() && !otherSlot.HasOccupantBlock();
                    bool validThisBlock = !slotManager.Cluster.IsOccupying() && !HasOccupantBlock();

                    if (validThisBlock && validOtherBlock && ValidElectrons(otherSlot))
                    {
                        StartOccupying(otherSlot);
                    }
                    else
                    {
                        //TODO repulse if not attract
                    }
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

        AddJointToElectrons();
        slotManager.Cluster.AddBlock(OccupantBlock);
    }

    public override void ReleaseBlock()
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
    public override bool HasOccupantBlock()
    {
        return OccupantBlock != null;
    }

    //Electron Utils
    void Entangle(QuantumSlotBehavior otherSlot)
    {
        for (int i = 0; i < connectingElectronPositions.Length; i++)
        {
            Vector3 relativePosition = GetRelativeElectronPosition(connectingElectronPositions[i]);
            ElectronPosition otherElectronPosition = otherSlot.slotManager.Block.electronManager.electronPositionDictionary[relativePosition.ToString()];
            connectingElectronPositions[i].Entangle(otherElectronPosition);
        }
    }

    void Untangle(QuantumSlotBehavior otherSlot)
    {
        for (int i = 0; i < connectingElectronPositions.Length; i++)
        {
            Vector3 relativePosition = GetRelativeElectronPosition(connectingElectronPositions[i]);
            ElectronPosition otherElectronPosition = otherSlot.slotManager.Block.electronManager.electronPositionDictionary[relativePosition.ToString()];
            connectingElectronPositions[i].Untangle(otherElectronPosition);
        }
    }

    Vector3 GetRelativeElectronPosition(ElectronPosition electronPosition)
    {
        return electronPosition.position - RelativeLocalPosition;
    }

    public bool ValidElectrons(QuantumSlotBehavior otherSlot)
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

    public bool HasInterferingElectrons(QuantumSlotBehavior otherSlot)
    {
        for (int i = 0; i < connectingElectronPositions.Length; i++)
        {
            Vector3 relativePosition = GetRelativeElectronPosition(connectingElectronPositions[i]);
            ElectronPosition otherElectronPosition = otherSlot.slotManager.Block.electronManager.electronPositionDictionary[relativePosition.ToString()];
            if(otherElectronPosition.electron != null && connectingElectronPositions[i].electron != null && connectingElectronPositions[i].electron != otherElectronPosition.electron)
            {
                return true;
            }
        }
        return false;
    }

    public QuantumSlotBehavior GetOtherSlot(GameObject other)
    {
        QuantumBlockBehavior otherBlockBehavior = other.GetComponent<QuantumBlockBehavior>();
        Vector3 otherBlockRelativeLocalPosition = -1 * RelativeLocalPosition;

        QuantumSlotBehavior otherBlockSlot = otherBlockBehavior.slotManager.slots[otherBlockRelativeLocalPosition.ToString()];
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
        points[0] = slotManager.Block.transform.position;
        points[1] = OccupantBlock.transform.position;
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }

    void ElectronLineUpdate()
    {
        Vector3[] points = new Vector3[4];
        for(int i=0; i < points.Length; i++)
        {
            points[i] = slotManager.Block.transform.TransformPoint(connectingElectronPositions[i].position);
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
                points.Add(slotManager.Block.transform.TransformPoint(connectingElectronPositions[i].position));
            }
        }
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }


}
