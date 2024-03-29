﻿using Matter.Block.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class FractureBlockSpecialSubController : BlockSpecialSubController
    {
        //Take a block, if its big enough splits it into 9, into a new cluster.

        public override string GetSpecialAbility1Description()
        {
            return "Fractures the block into 9 equal parts. Only works if remaining blocks meet the min size requirements.";
        }

        public override void OnSpecialAbility1()
        {
            ClusterBehavior cluster = GetCluster();
            BlockBehavior block = cluster.trackingBlock;
            block.GetComponent<BoxCollider>().enabled = false;
            Player.gameMaster.spawnManager.CreateClusterGrid(block.transform, BlockUtils.IsQuantumBlock(block.gameObject));
            Player.OnDetachCluster();
            Destroy(block.gameObject);
        }

        public override string GetSpecialAbility2Description()
        {
            return "N/A";
        }
        public override void OnSpecialAbility2()
        {

        }

        public override string GetSpecialAbility3Description()
        {
            return "";
        }

        public override void OnSpecialAbility3()
        {

        }
    }
}


