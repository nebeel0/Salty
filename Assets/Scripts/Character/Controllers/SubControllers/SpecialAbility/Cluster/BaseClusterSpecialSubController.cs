using Matter.Block.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class BaseClusterSpecialSubController : ClusterSpecialSubController
    {
        public override string GetSpecialAbility1Description()
        {
            return "Toggle Locking.";
        }

        public override void OnSpecialAbility1()
        {
            bool lockFlag = !GetCluster().trackingBlock.GetSlotManager().slotLockEnabled;
            foreach (BlockBehavior block in GetCluster().blocks)
            {
                block.GetSlotManager().slotLockEnabled = lockFlag;
            }
        }

        public override string GetSpecialAbility2Description()
        {
            return "Release All Blocks";
        }
        public override void OnSpecialAbility2()
        {
            GetCluster().RemoveBlocks(GetCluster().blocks);
        }

        public override string GetSpecialAbility3Description()
        {
            return "Combine";
        }

        public override void OnSpecialAbility3()
        {

        }
    }
}


