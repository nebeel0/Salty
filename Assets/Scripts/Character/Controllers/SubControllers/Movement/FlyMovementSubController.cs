
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class FlyMovementSubController : MovementSubController
    {
        protected override void GhostUpdate()
        {
            transform.position += Displacement;
        }

        protected override void BlockUpdate()
        {
            ClusterDistributeForce(Displacement, forceMode: ForceMode.Force);
        }

        protected override void ClusterUpdate()
        {
            ClusterDistributeForce(Displacement, forceMode: ForceMode.Force);
        }

    }
}
