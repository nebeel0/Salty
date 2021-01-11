using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ElectronBehavior : LeptonBehavior
{
    public ElectronPosition electronPosition = null; //Total of 8 blocks 
    public Dictionary<BlockBehavior, FixedJoint> connectedBlocks = new Dictionary<BlockBehavior, FixedJoint>(); //up to 24 connected blocks

    public override void Start()
    {
        particleType = ParticleUtils.leptonNeg;
        base.Start();
    }

    public bool isLocked
    {
        get { return connectedBlocks.Count > 0; }
    }

    public void ConnectBlock(BlockBehavior block)
    {
        if(!connectedBlocks.ContainsKey(block))
        {
            electronPosition.Jump();
            connectedBlocks[block] = gameObject.AddComponent<FixedJoint>();
            connectedBlocks[block].enableCollision = false;
            connectedBlocks[block].connectedBody = block.GetComponent<Rigidbody>();
            block.slotManager.OccupantsUpdate();
        }
    }

    public void ReleaseBlock(BlockBehavior block)
    {
        if (connectedBlocks.ContainsKey(block))
        {
            FixedJoint removedFixedJoint = connectedBlocks[block];
            Destroy(removedFixedJoint);
            connectedBlocks.Remove(block);
            block.slotManager.OccupantsUpdate();
        }
    }

    public override void Free()
    {
        electronPosition = null;
        foreach (KeyValuePair<BlockBehavior, FixedJoint> connectedBlock in connectedBlocks)
        {
            ReleaseBlock(connectedBlock.Key);
        }
        base.Free();
    }

    protected override void Update()
    {
        base.Update();
        if(electronPosition != null)
        {
            Orbit();
        }
    }

    void Orbit()
    {
        if(electronPosition.PositionsNotFull())
        {
            if (Vector3Utils.V3Equal(transform.localPosition, electronPosition.position))
            {
                if(electronPosition.electron != this || electronPosition.electron == null)
                {
                    Debug.LogError("WTF");
                }

                if(!isLocked && !electronPosition.IsEntangled)
                {
                    electronPosition.AttemptJump();
                }
            }
            else
            {
                electronPosition.Jumping();
            }
        }
    }

}
