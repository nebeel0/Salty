using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

namespace Controller
{
    public class RotationSubController : SubController
    {
        public Transform RotationTarget
        {
            get 
            {
                return Player.gimbal.transform;
                //if (IsGhost() || GetComponent<PerspectiveSubController>().thirdPerson || IsCluster())
                //{
                //    return Player.gimbal.transform;
                //}
                //else if(IsBlock())
                //{
                //    return GetCluster().trackingBlock.transform;
                //}
                //else
                //{
                //    return null;
                //}
            }
        }
        protected float lookSensitivity = 10;
        protected float rotationLerpTime = 0.01f;
        protected float RotationLerpPct
        {
            get { return 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime); }
        }
        public bool invertY = false;
        protected Vector3 lookDelta = Vector3.zero;
        public bool targetLockFlag = false;

        protected virtual void Update()
        {
            Player.transform.eulerAngles = Vector3.zero;
            RotationUpdate();
        }
        void RotationUpdate()
        {
            if(targetLockFlag)
            {

            }
            if (RotationTarget != null)
            {
                if(targetLockFlag)
                {

                }
                else
                {
                    Vector3 updatedRotation = lookDelta + RotationTarget.eulerAngles;
                    if (updatedRotation.x > 90 && updatedRotation.x < 270)
                    {
                        if (updatedRotation.x - 90 > 270 - updatedRotation.x)
                        {
                            updatedRotation.x = 270;
                        }
                        else
                        {
                            updatedRotation.x = 90;
                        }
                    }
                    RotationTarget.rotation = Quaternion.Euler(updatedRotation);
                    lookDelta = Vector3.zero;
                }
            }
        }

        //Input Functions
        void OnLook(InputValue value) //TODO Will Probably be overridden for xr
        {
            Vector2 lookDelta = value.Get<Vector2>() * Time.deltaTime * lookSensitivity;
            OnLook(lookDelta);
        }

        void OnTargetLock(InputValue value)
        {
            bool availableTargets = false; //TODO 
            targetLockFlag = value.Get<float>() == 1 && availableTargets;
        }

        //Secondary/AI Functions
        public void OnLook(Vector2 lookDelta) //TODO see if we can set a value for InputValue so we can use onLook 
        {
            lookDelta.y *= (invertY ? 1 : -1);
            this.lookDelta.y = lookDelta.x;
            this.lookDelta.x = lookDelta.y;
            this.lookDelta.z = 0;
        }

        public void LookAt(Transform lookTarget)
        {
            if (RotationTarget != null)
            {
                lookDelta = Vector3.zero;
                RotationTarget.LookAt(lookTarget);
            }
        }

        public void LookAt(Vector3 position)
        {
            if (RotationTarget != null)
            {
                Vector3 relativePos = position - transform.position;
                lookDelta = Vector3.zero;
                Quaternion rotation = Quaternion.LookRotation(relativePos);
                transform.rotation = rotation;
            }
        }

    }
}
