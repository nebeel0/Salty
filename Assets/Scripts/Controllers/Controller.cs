using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class Controller : MonoBehaviour
{
    protected Transform target;
    public class RotationState
    {
        public float yaw;
        public float pitch;
        public float roll;

        public void SetFromTransform(Transform t)
        {
            pitch = t.eulerAngles.x;
            yaw = t.eulerAngles.y;
            roll = t.eulerAngles.z;
        }

        public void SetFromEulerAngles(Vector3 eulerAngles)
        {
            pitch = eulerAngles.x;
            yaw = eulerAngles.y;
            roll = eulerAngles.z;
        }


        public void Copy(RotationState target)
        {
            yaw = target.yaw;
            pitch = target.pitch;
            roll = target.roll;
        }

        public void LerpTowards(RotationState target, float rotationLerpPct)
        {
            yaw = Mathf.LerpAngle(yaw, target.yaw, rotationLerpPct);
            pitch = Mathf.LerpAngle(pitch, target.pitch, rotationLerpPct);
            roll = Mathf.LerpAngle(roll, target.roll, rotationLerpPct);
        }
        public void LerpTowardsNeutral(float rotationLerpPct)
        {
            pitch = Mathf.LerpAngle(roll, 0, rotationLerpPct);
            roll = Mathf.LerpAngle(roll, 0, rotationLerpPct);
        }

        public void LerpTowardsZero(float rotationLerpPct)
        {
            pitch = Mathf.LerpAngle(roll, 0, rotationLerpPct);
            roll = Mathf.LerpAngle(roll, 0, rotationLerpPct);
            yaw = Mathf.LerpAngle(roll, 0, rotationLerpPct);
        }

        public void UpdateTransform(Transform t)
        {
            t.eulerAngles = new Vector3(pitch, yaw, roll);
        }

        public void UpdateLocalTransform(Transform t)
        {
            t.localEulerAngles = new Vector3(pitch, yaw, roll);
        }
    }
    protected float lookSensitivity = 10;
    protected float rotationLerpTime = 0.01f;
    protected float rotationLerpPct
    {
        get { return 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime); }
    }
    public bool invertY = false;

    public GameObject directionalLight;
    public GameObject primaryCameraGameObject;
    protected PlayerInput playerInput;

    protected Camera primaryCamera;
    public float primaryCameraDisplacement = 0;
    protected Vector3 primaryCameraRootPosition = new Vector3(0, 0.5f, 0); //TODO static element?

    protected RotationState m_TargetCameraState = new RotationState();
    protected RotationState m_InterpolatingCameraState = new RotationState();
    protected Vector2 lookDelta = Vector2.zero;
    protected float holdScalar = 0;
    protected bool holdFlag = false;
    protected bool resetOrientation = false;


    public virtual void Start()
    {
        // TODO when you die you can exist as a camera and find a new block to be come your host?
        // Clear Camera or Directional Light
        primaryCamera = Instantiate(primaryCameraGameObject, transform).GetComponent<Camera>();
        primaryCamera.transform.localPosition = primaryCameraRootPosition;
        Instantiate(directionalLight, primaryCamera.transform);
        playerInput = gameObject.GetComponent<PlayerInput>();
    }

    protected virtual void Update()
    {
        ResetOrientationUpdate();
        RotationUpdate();
    }


    protected virtual void RotationUpdate()
    {
        if(target != null)
        {
            m_TargetCameraState.SetFromTransform(target);
            m_InterpolatingCameraState.SetFromTransform(target);

            m_TargetCameraState.yaw += lookDelta.x * lookSensitivity; 
            m_TargetCameraState.pitch += lookDelta.y * lookSensitivity;

            m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, rotationLerpPct);
            m_InterpolatingCameraState.UpdateTransform(target);
        }
    }


    protected void OnLook(InputValue value) //TODO Will Probably be overridden for xr
    {
        lookDelta = value.Get<Vector2>() * Time.deltaTime;
        lookDelta.y *= (invertY ? 1 : -1);
    }

    public void OnResetOrientation()
    {
        resetOrientation = true;
    }

    protected void ResetOrientationUpdate()
    {
        //TODO maybe just rotate camera and not body
        //TODO initial a lock on perspective and rotate quickly to 0,0,0
        //TODO fade Rotate
        if (resetOrientation)
        {
            m_TargetCameraState.SetFromTransform(target);
            m_InterpolatingCameraState.SetFromTransform(target);
            if (m_InterpolatingCameraState.pitch > 0.1 || m_InterpolatingCameraState.roll > 0.1 || m_InterpolatingCameraState.pitch < -0.1 || m_InterpolatingCameraState.roll < -0.1)
            {
                m_TargetCameraState.LerpTowardsNeutral(Time.deltaTime);
                m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, rotationLerpPct);
                m_InterpolatingCameraState.UpdateTransform(target);
            }
            else if (m_InterpolatingCameraState.pitch >= 0.1 || m_InterpolatingCameraState.roll >= 0.1 || m_InterpolatingCameraState.pitch <= -0.1 || m_InterpolatingCameraState.roll <= -0.1)
            {
                m_TargetCameraState.LerpTowardsNeutral(1);
                m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, 1);
                m_InterpolatingCameraState.UpdateTransform(target);
            }
            else
            {
                resetOrientation = false;
            }
        }
    }


    protected virtual void OnHold()
    {
        holdFlag = !holdFlag; //Sets to hold
        if (!holdFlag && holdScalar > 0)
        {
            holdScalar = 0; //Reset
        }
    }

    protected virtual void HoldUpdate()
    {
        if (holdFlag)
        {
            holdScalar += 1 + holdScalar * (float).25 * Time.deltaTime;
        }
    }


}
