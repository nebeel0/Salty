﻿using UnityEngine;
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
                if(Player.Cluster == null || Player.Cluster.blocks.Count == 1)
                {
                    return gameObject.transform;
                }
                else
                {
                    return GetComponent<PerspectiveController>().PrimaryCamera.transform;
                }
            }
        }
        protected float lookSensitivity = 10;
        protected float rotationLerpTime = 0.01f;
        protected
            float RotationLerpPct
        {
            get { return 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime); }
        }
        public bool invertY = false;
        protected Vector2 lookDelta = Vector2.zero;

        protected virtual void Update()
        {
            RotationUpdate();
        }
        public void RotationUpdate()
        {
            //rotationTarget = Player.Cluster.blocks.Count > 1 ? transform : Cluster.trackingBlock.transform;
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