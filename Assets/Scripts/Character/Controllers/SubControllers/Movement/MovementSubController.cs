using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public abstract class MovementSubController : SubController
    {
        public Vector3 direction = Vector3.zero;
        public float movementFactor = 10;

        protected Vector3 Displacement
        {
            get
            {
                return Player.gimbal.transform.TransformDirection(direction) * Distance;
            }
        }

        protected float Distance
        {
            get
            {
                return movementFactor * Time.deltaTime;
            }
        }


        void OnMove(InputValue inputValue)  //For InputController
        {
            OnMove(inputValue.Get<Vector2>());
        }

        public void OnMove(Vector2 direction) //For AI
        {
            this.direction.x = direction.x;
            this.direction.z = direction.y;
        }

        protected void Update()
        {
            if(IsGhost())
            {
                GhostUpdate();
            }
            else if(IsBlock())
            {
                BlockUpdate();
            }
            else if(IsCluster())
            {
                ClusterUpdate();
            }
            else
            {
                Debug.LogError("Player is in unknown state.");
            }
        }

        protected abstract void GhostUpdate();
        protected abstract void BlockUpdate();
        protected abstract void ClusterUpdate();

        protected void ClusterDistributeForce(Vector3 forceVector, ForceMode forceMode)
        {
            float accelerationFactor = 50;
            Vector3 distributedForce = forceVector * accelerationFactor;
            foreach (BlockBehavior block in GetCluster().blocks)
            {
                block.GetComponent<Rigidbody>().AddForce(distributedForce, forceMode);
            }
        }

    }
}


