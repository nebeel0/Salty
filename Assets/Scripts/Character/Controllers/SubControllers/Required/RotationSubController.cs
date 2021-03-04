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
        protected Vector2 lookDelta = Vector2.zero;

        protected virtual void Update()
        {
            Player.transform.eulerAngles = Vector3.zero;
            RotationUpdate();
        }
        public void RotationUpdate()
        {
            if (RotationTarget != null && enabled)
            {
                RotationUtils.UpdateRotation(RotationTarget, lookDelta, RotationLerpPct);
                lookDelta = Vector2.zero;
            }
        }

        //Input Functions
        public void OnLook(InputValue value) //TODO Will Probably be overridden for xr
        {
            if (enabled)
            {
                lookDelta = value.Get<Vector2>() * Time.deltaTime * lookSensitivity;
                lookDelta.y *= (invertY ? 1 : -1);
            }
        }

        //Secondary/AI Functions
        public void SetLookDelta(Vector2 lookDelta) //TODO see if we can set a value for InputValue so we can use onLook 
        {
            this.lookDelta = lookDelta;
        }

        public void LookAt(Transform lookTarget)
        {
            if (RotationTarget != null)
            {
                lookDelta = Vector2.zero;
                RotationTarget.LookAt(lookTarget);
            }
        }
    }
}
