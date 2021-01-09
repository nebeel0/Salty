using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ElectronBehavior : LeptonBehavior
{
    public HashSet<ElectronPosition> electronPositions = new HashSet<ElectronPosition>(); //Total of 8 blocks 
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

    public ElectronPosition GetRootElectronPosition()
    {
        return electronPositions.First();
    }

    BlockBehavior GetRootBlock()
    {
        return GetRootElectronPosition().electronManager.block;
    }

    public void ConnectBlock(BlockBehavior block)
    {
        if(!connectedBlocks.ContainsKey(block))
        {
            connectedBlocks[block] = GetRootBlock().gameObject.AddComponent<FixedJoint>();
            connectedBlocks[block].enableCollision = false;
            connectedBlocks[block].connectedBody = block.GetComponent<Rigidbody>();
        }
        block.slotManager.OccupantsUpdate();
    }

    public void ReleaseBlock(BlockBehavior block)
    {
        if (connectedBlocks.ContainsKey(block))
        {
            FixedJoint removedFixedJoint = connectedBlocks[block];
            Destroy(removedFixedJoint);
            block.slotManager.OccupantsUpdate();
        }
    }

    public override void Free()
    {
        foreach(ElectronPosition electronPosition in electronPositions)
        {
            electronPosition.ReleaseElectron();
        }
        electronPositions.Clear();
        foreach (KeyValuePair<BlockBehavior, FixedJoint> connectedBlock in connectedBlocks)
        {
            ReleaseBlock(connectedBlock.Key);
        }
        base.Free();
    }

    protected override void Update()
    {
        base.Update();
        if(electronPositions.Count >= 1)
        {
            Orbit();
        }
    }

    void Orbit()
    {
        ElectronPosition electronPosition = GetRootElectronPosition();
        if(electronPosition.CanJump())
        {
            if (Vector3Utils.V3Equal(transform.localPosition, electronPosition.position))
            {
                if(!isLocked)
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
