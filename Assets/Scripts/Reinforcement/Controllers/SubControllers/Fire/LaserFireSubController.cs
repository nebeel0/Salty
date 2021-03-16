using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class LaserFireSubController : FireSubController
    {
        //Subcontroller for "Fire" actions, basically right trigger, and mouse left click. Is Press and Release
        public override string GetFireDescription()
        {
            return "Shoots a laser.";
        }

        public override void FireUpdate()
        {
            base.FireUpdate();
            if (IsBlock())
            {
                GetCluster().trackingBlock.GetComponent<Matter.Block.Property.Energy>().FireLaser();
            }
        }

    }
}


