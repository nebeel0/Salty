
using Matter.Block.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class TeleportMovementSubController : MovementSubController
    {
        float distanceAggregator = 0;
        Vector3 lastDirection = Vector3.zero;
        //TODO make sure nothing is in path.

        protected override void GhostUpdate()
        {
            Debug.LogError("Can't be used for Ghosts");
        }

        protected override void BlockUpdate()
        {
            bool buttonRelease = direction.Equals(Vector3.zero);
            bool nonZeroDisplacement = distanceAggregator > 0;
            if (buttonRelease && nonZeroDisplacement)
            {
                GetCluster().trackingBlock.transform.position += distanceAggregator * lastDirection;
                distanceAggregator = 0;
            }
            else
            {
                distanceAggregator += Distance;
            }
        }

        protected override void ClusterUpdate()
        {
            bool buttonRelease = direction.Equals(Vector3.zero);
            bool nonZeroDisplacement = distanceAggregator > 0;
            if (buttonRelease && nonZeroDisplacement)
            {
                foreach (BlockBehavior block in GetCluster().blocks)
                {
                    block.transform.position += distanceAggregator * lastDirection;
                }
                distanceAggregator = 0;
            }
            else
            {
                distanceAggregator += Distance;
            }
        }
    }
}
