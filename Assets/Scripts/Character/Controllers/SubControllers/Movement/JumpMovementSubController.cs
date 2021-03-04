
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class JumpMovementSubController : MovementSubController
    {
        float distanceAggregator = 0;
        Vector3 lastDirection = Vector3.zero;

        protected override void GhostUpdate()
        {
            Debug.LogError("Can't be used for Ghosts");
        }

        protected override void BlockUpdate()
        {
            ClusterUpdate();
        }

        protected override void ClusterUpdate()
        {
            bool buttonRelease = direction.Equals(Vector3.zero);
            bool nonZeroDisplacement = distanceAggregator > 0;
            if (buttonRelease && nonZeroDisplacement)
            {
                ClusterDistributeForce(distanceAggregator * lastDirection, forceMode: ForceMode.Impulse);
                distanceAggregator = 0;
                lastDirection = Vector3.zero;
            }
            else
            {
                distanceAggregator += Distance;
                lastDirection = Displacement;
            }
        }
    }
}
