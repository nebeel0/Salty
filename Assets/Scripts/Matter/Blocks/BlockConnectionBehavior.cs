using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockConnectionBehavior : MatterBehavior
{
    public class BlockConnection
    {
        public Vector3 otherBlockLocalPosition;
        public GameObject thisBlock;
        public GameObject otherBlock;
        public FixedJoint fixedJoint;

        public bool hasOtherBlock
        {
            get { return otherBlock != null; }
        }

        public bool hasFixedJoint
        {
            get { return fixedJoint != null && fixedJoint.connectedBody != null; }
        }

        public BlockConnection(GameObject thisBlock, Vector3 otherBlockLocalPosition)
        {
            this.otherBlockLocalPosition = otherBlockLocalPosition;
            this.thisBlock = thisBlock;
            otherBlock = null;
            fixedJoint = null;
        }

        public void Refresh()
        {
            DeathCheck();
        }

        public void DeathCheck()
        {
            if (false) //TODO implement dynamic death checks
            {
                Death();
            }
        }
        public bool Valid()
        {
            //TODO arbitrary condition provided by user and states of blocks
            return true;
        }
        public void Death()
        {
            if (hasOtherBlock)
            {
                Physics.IgnoreCollision(otherBlock.GetComponent<Collider>(), thisBlock.GetComponent<Collider>(), false);
            }
            thisBlock.GetComponent<BlockConnectionBehavior>().DisableCollision();
            Destroy(fixedJoint);
            otherBlock = null;
            fixedJoint = null;

            if (thisBlock != null && otherBlock != null)
            {
                if (thisBlock.transform.parent == otherBlock.transform)
                {
                    thisBlock.transform.parent = null;
                }
                else if (otherBlock.transform.parent == thisBlock.transform)
                {
                    otherBlock.transform.parent = null;
                }
            }
        }

        public void Reflect(BlockConnection otherBlockConnection)
        {
            fixedJoint = otherBlockConnection.fixedJoint;
            otherBlock = otherBlockConnection.thisBlock;
        }
        public void Place()
        {
            //Snapping
            //if (thisBlock.transform.InverseTransformPoint(otherBlock.transform.position) != otherBlockLocalPosition)
            //{
                Transform otherBlockParent = otherBlock.transform.parent;
                otherBlock.transform.parent = thisBlock.transform;

                Vector3 otherBlockEulerAngles = otherBlock.transform.localEulerAngles;
                otherBlockEulerAngles.x = Mathf.LerpAngle(otherBlockEulerAngles.x, Mathf.Round(otherBlockEulerAngles.x / 90) * 90, 1);
                otherBlockEulerAngles.y = Mathf.LerpAngle(otherBlockEulerAngles.y, Mathf.Round(otherBlockEulerAngles.y / 90) * 90, 1);
                otherBlockEulerAngles.z = Mathf.LerpAngle(otherBlockEulerAngles.z, Mathf.Round(otherBlockEulerAngles.z / 90) * 90, 1);
                otherBlock.transform.localEulerAngles = otherBlockEulerAngles;

                otherBlock.transform.localPosition = otherBlockLocalPosition;
                otherBlock.transform.parent = otherBlockParent;

                //Setting fixed joint AKA glue, also terminates the Place Loop
                fixedJoint = thisBlock.AddComponent<FixedJoint>();
                fixedJoint.enableCollision = false;
                fixedJoint.connectedBody = otherBlock.GetComponent<Rigidbody>();

                BlockConnectionBehavior otherBlockConnectionBehavior = otherBlock.GetComponent<BlockConnectionBehavior>();
                Vector3 positionFromOtherBlock = otherBlock.transform.InverseTransformPoint(thisBlock.transform.position);
                BlockConnection otherBlockConnection = otherBlockConnectionBehavior.connectedBlocks[positionFromOtherBlock.ToString()];
                otherBlockConnection.Reflect(this);
            //}
        }
    }

    public Dictionary<string, BlockConnection> connectedBlocks;

    public GameObject ParentCluster
    {
        get
        {
            if (transform.parent != null && transform.parent.tag == "Cluster")
            {
                return transform.parent.gameObject;
            }
            return null;
        }
    }

    public GameObject clusterRef;


    public bool isConnected(BlockConnectionBehavior other)
    {
        foreach(BlockConnection blockConnection in connectedBlocks.Values)
        {
            if(blockConnection.otherBlock == other.gameObject)
            {
                return true;
            }
        }
        return false;
    }


    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        SetUpConnectedBlocks();
    }

    protected override void RefreshState()
    {
        if (ParentCluster == null)
        {
            GameObject cluster = Instantiate(clusterRef);
            transform.SetParent(cluster.transform);
        }
        RefreshConnections(); //Place Neighbors, Neighbor Refresh
    }

    void SetUpConnectedBlocks()
    {
        connectedBlocks = new Dictionary<string, BlockConnection>();
        for (int i = 1; i <= 2; i++)
        {
            for (int ii = 0; ii < 3; ii++)
            {
                Vector3 newFixedJointPosition = Vector3.zero;
                newFixedJointPosition[ii] = Mathf.Pow(-1, i) * (int)scalingFactor * 1.1f;
                connectedBlocks.Add(newFixedJointPosition.ToString(), new BlockConnection(gameObject, newFixedJointPosition));
            }
        }
    }

    void RefreshConnections() //Once it has been put in place add the fixed joint
    {
        foreach (BlockConnection blockConnection in connectedBlocks.Values)
        {
            blockConnection.Refresh();
        }
    }

    void CollideBlock(GameObject otherBlock) //TODO animation for connecting
    {
        BlockConnectionBehavior otherBlockConnectionBehavior = otherBlock.GetComponent<BlockConnectionBehavior>();
        bool sameParentCluster = otherBlockConnectionBehavior.ParentCluster == ParentCluster;
        if (sameParentCluster || otherBlock.transform.parent == transform || transform.parent == otherBlock.transform)
        {
            return;
        }
        Vector3 otherBlockLocalPosition = transform.InverseTransformPoint(otherBlock.transform.position);
        string closestBlockConnectionI = "";
        float closestDistance = 0;
        foreach (KeyValuePair<string, BlockConnection> connectedBlock in connectedBlocks)
        {
            float distance = Vector3.Distance(connectedBlock.Value.otherBlockLocalPosition, otherBlockLocalPosition);
            if (closestBlockConnectionI == "" || distance < closestDistance)
            {
                closestBlockConnectionI = connectedBlock.Key;
                closestDistance = distance;
            }
        }
        if (!connectedBlocks[closestBlockConnectionI].hasOtherBlock)
        {
            AddBlock(blockConnection: connectedBlocks[closestBlockConnectionI], otherBlock: otherBlock);
        }
    }

    void AddBlock(BlockConnection blockConnection, GameObject otherBlock)
    {
        Physics.IgnoreCollision(otherBlock.GetComponent<Collider>(), GetComponent<Collider>(), true);
        blockConnection.otherBlock = otherBlock;
        blockConnection.Place();
        if(ParentCluster != null)
        {
            ParentCluster.GetComponent<ClusterBehavior>().Merge(otherBlock.GetComponent<ClusterBehavior>());
        }
        blockConnection.Refresh();
    }

    void OnCollisionEnter(Collision col)  // TODO Use C# Job System to avoid extra subatomic particles or leptons than possible
    {
        if (col.gameObject.CompareTag("Block"))
        {
            DisableCollision();
            CollideBlock(col.gameObject);
        }
    }
}
