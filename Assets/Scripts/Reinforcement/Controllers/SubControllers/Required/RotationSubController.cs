using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

namespace Controller
{
    public class RotationSubController : SubController
    {
        Transform RotationTarget
        {
            get 
            {
                return Player.gimbal.transform;
            }
        }
        float lookSensitivity = 10;
        bool invertY = false;
        Vector3 lookDelta = Vector3.zero;
        bool targetLockFlag = false;
        bool lockRotationFlag = false;

        public bool cameraZoomMode;

        protected virtual void Update()
        {
            Player.transform.eulerAngles = Vector3.zero;
            RotationUpdate();
        }


        void RotationUpdate()
        {
            if (RotationTarget != null)
            {
                if(targetLockFlag)
                {
                    LookAtTargets();
                }
                else if(cameraZoomMode)
                {
                    float newThirdPersonCameraDisplacement = lookDelta.x + GetComponent<PerspectiveSubController>().thirdPersonCameraDisplacement;
                    GetComponent<PerspectiveSubController>().thirdPersonCameraDisplacement = Mathf.Clamp(newThirdPersonCameraDisplacement, 4, 100);
                }
                else if(!lockRotationFlag)
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
        void OnLockRotation()
        {
            lockRotationFlag = !lockRotationFlag;
        }


        void OnCameraZoom(InputValue inputValue)
        {
            cameraZoomMode = inputValue.Get<float>() == 1;
        }

        void OnLook(InputValue value) //TODO Will Probably be overridden for xr
        {
            Vector2 lookDelta = value.Get<Vector2>() * Time.deltaTime * lookSensitivity;
            OnLook(lookDelta);
        }

        void OnTargetLock(InputValue value)
        {
            OnTargetLock(value.Get<float>());
        }

        public void OnTargetLock(float value)
        {
            bool availableTargets = GetComponent<Character.Managers.CharacterTargetManager>().HasTargets();
            targetLockFlag = value == 1 && availableTargets;
        }

        //Secondary/AI Functions
        public void OnLook(Vector2 lookDelta) //TODO see if we can set a value for InputValue so we can use onLook 
        {
            if(!targetLockFlag)
            {
                lookDelta.y *= (invertY ? 1 : -1);
                this.lookDelta.y = lookDelta.x;
                this.lookDelta.x = lookDelta.y;
                this.lookDelta.z = 0;
            }
        }

        void LookAtTargets()
        {
            Vector3 position = GetComponent<Character.Managers.CharacterTargetManager>().GetTargetsCenter();
            Vector3 relativePos = position - transform.position;
            lookDelta = Vector3.zero;
            Quaternion targetedRotation = Quaternion.LookRotation(relativePos);
            Quaternion lerpedRotation = Quaternion.Lerp(RotationTarget.rotation, targetedRotation, Time.deltaTime * 10);
            RotationTarget.rotation = lerpedRotation;
        }

    }
}
