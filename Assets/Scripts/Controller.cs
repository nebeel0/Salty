using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class Controller : MonoBehaviour
{
    protected class CameraState
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

        public void LerpTowards(CameraState target, float rotationLerpPct)
        {
            yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
            pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
            roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);
        }
        public void LerpTowardsNeutral(float rotationLerpPct)
        {
            pitch = Mathf.Lerp(roll, 0, rotationLerpPct);
            roll = Mathf.Lerp(roll, 0, rotationLerpPct);
        }

        public void UpdateTransform(Transform t)
        {
            t.eulerAngles = new Vector3(pitch, yaw, roll);
        }
    }

    [Header("Rotation Settings")]
    [Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
    public float mouseSensitivity = 2f;

    [Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
    public float rotationLerpTime = 0.01f;
    protected float rotationLerpPct
    {
        get { return 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime); }
    }
    [Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
    public bool invertY = false;

    public Camera primaryCamera;
    public Vector3 primaryCameraRootPosition;
    public Vector3 primaryCameraSlingShotPosition;
    public float primaryCameraSlingShotSpeed = 0.25f;

    protected CameraState m_TargetCameraState = new CameraState();
    protected CameraState m_InterpolatingCameraState = new CameraState();
    protected bool visualFlag = true;
    protected bool lockPerspective = false;
    protected bool lockOn = false;
    protected bool fadeOutFlag = false;
    protected bool planHoldFlag = false;
    protected bool resetOrientationFlag = false;
    protected PlayerInput playerInput;
    protected new Rigidbody rigidbody;
    protected LineRenderer lineRenderer;
    protected float planScalar = 0;
    protected enum ActionQueueTypes
    {
        Default, //Launch Forward, or movement action
        Photon,
        ZBoson,
        WBoson,
    }
    protected ActionQueueTypes actionQueue = ActionQueueTypes.Default;
    protected enum ActionQueueModeTypes
    {
        Default, //Doesn't enqueue, and automatically routes the action
        Add,
    }
    protected ActionQueueModeTypes actionQueueMode = ActionQueueModeTypes.Default;
    protected class ActionParams
    {
        public Vector3 direction;
        public float scalar;
        public bool lockOnFlag;
        public float createdTime; //https://docs.unity3d.com/ScriptReference/Time-time.html

        public ActionParams(Vector3 direction, float scalar, bool lockOnFlag)
        {
            this.direction = direction;
            this.scalar = scalar;
            this.lockOnFlag = lockOnFlag;
            createdTime = Time.time; // TODO If we're simulating everything we'll need to do an advanced simulation that takes into account lost mass, since if we export a particle, and then travel, it'll be a different path, just copy the current state, and actually run everything?
        }
    }
    protected Dictionary<ActionQueueTypes, Queue<ActionParams>> actionQueues = new Dictionary<ActionQueueTypes, Queue<ActionParams>>();  //Maybe include time stamp so we can also execute them all in the order they were created

    public virtual void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rigidbody = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
        foreach (ActionQueueTypes aq in (ActionQueueTypes[])System.Enum.GetValues(typeof(ActionQueueTypes)))
        {
            actionQueues[aq] = new Queue<ActionParams>();
        }
        m_TargetCameraState.SetFromTransform(transform);
        m_InterpolatingCameraState.SetFromTransform(transform);
        primaryCameraRootPosition = primaryCamera.transform.localPosition;
        primaryCameraSlingShotPosition = primaryCameraRootPosition;
    }

    public virtual void Update()
    {
        PerspectiveUpdate();
        ActionQueueUpdate();
        VisualUpdate();
    }

    protected virtual void PerspectiveUpdate()
    {
        ResetOrientationUpdate();
        if (!lockPerspective && !resetOrientationFlag)
        {
            var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));

            m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivity;
            m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivity;

            // Framerate-independent interpolation
            // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
            m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, rotationLerpPct);
            m_InterpolatingCameraState.UpdateTransform(transform);
        }
        primaryCamera.transform.localPosition = Vector3.Lerp(primaryCamera.transform.localPosition, primaryCameraSlingShotPosition, Time.deltaTime);
    }

    protected void ActionQueueUpdate()
    {
        //Only when executing do we set up our actions
        PlanHoldUpdate();
    }

    protected void VisualUpdate()
    {
        FadeOutLineRendererUpdate();
    }

    protected void ActionRouter(ActionParams actionParams)
    {
        Vector3 direction = actionParams.direction;
        float scalar = actionParams.scalar;

        if (actionQueue == ActionQueueTypes.Default)
        {
            rigidbody.AddForce(direction * scalar, ForceMode.Impulse);
        }
    }

    public void OnLockPerspective() //Should be right click
    {
        lockPerspective = !lockPerspective;
    }

    public void OnPlan()
    {
        planHoldFlag = !planHoldFlag; //Sets to hold
        if (planHoldFlag)
        {
            SetUpLineRenderer();
        }
        else if (!planHoldFlag && planScalar > 0)
        {
            primaryCameraSlingShotPosition = primaryCameraRootPosition;
            Vector3 direction = transform.forward;
            float scalar = planScalar;
            planScalar = 0; //Reset

            ActionParams actionParams = new ActionParams(direction, scalar, lockOn);

            if(actionQueueMode == ActionQueueModeTypes.Add)
            {
                actionQueues[actionQueue].Enqueue(actionParams);
            }
            else
            {
                ActionRouter(actionParams);
            }
            fadeOutFlag = true;
        }
    }

    public void OnAddModeToggle()
    {
        if(actionQueueMode == ActionQueueModeTypes.Add)
        {
            actionQueueMode = ActionQueueModeTypes.Default;
        }
        else
        {
            actionQueueMode = ActionQueueModeTypes.Add;
        }
    }

    public void OnFire()
    {
        if(actionQueues[actionQueue].Count > 0)
        {
            ActionRouter(actionQueues[actionQueue].Dequeue());
        }
    }

    public void OnDrawPlan() // TODO slingshot effect, instead of drawing line, pull camera back
    {
        SetUpLineRenderer();
        if (actionQueues[actionQueue].Count != lineRenderer.positionCount - 1 && actionQueues[actionQueue].Count > 0)
        {
            Vector3 positionTracker = transform.position; //tracker for default action queue
            List<Vector3> points = new List<Vector3>();
            points.Add(positionTracker);
            IEnumerator<ActionParams> enumerator = GetActionQueueEnumerator();
            while (enumerator.MoveNext())
            {
                if (actionQueue == ActionQueueTypes.Default)
                {
                    ActionParams actionParams = enumerator.Current;
                    Vector3 point = EstimateLaunchDestination(actionParams, positionTracker);
                    points.Add(point);
                    positionTracker = point;
                }
            }

            if (points.Count > 1)
            {
                lineRenderer.positionCount = points.Count;
                lineRenderer.SetPositions(points.ToArray());
            }
        }
        fadeOutFlag = true;
    }
    public void OnVisualToggle()
    {
        visualFlag = !visualFlag;
    }

    public void OnResetOrientation()
    {
        resetOrientationFlag = true;
    }

    protected void ResetOrientationUpdate()
    {
        //TODO maybe just rotate camera and not body
        //TODO initial a lock on perspective and rotate quickly to 0,0,0
        //TODO fade Rotate
        if (resetOrientationFlag)
        {
            if(m_InterpolatingCameraState.pitch > 0.01 || m_InterpolatingCameraState.roll > 0.01 || m_InterpolatingCameraState.pitch < -0.01 || m_InterpolatingCameraState.roll < -0.01)
            {
                m_TargetCameraState.LerpTowardsNeutral(Time.deltaTime);
                m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, rotationLerpPct);
                m_InterpolatingCameraState.UpdateTransform(transform);
            }
            else
            {
                resetOrientationFlag = false;
            }
        }
    }

    protected void PlanHoldUpdate()
    {
        if (planHoldFlag)
        {
            planScalar += 1 + planScalar * (float).25 * Time.deltaTime;
            primaryCameraSlingShotPosition += planScalar * primaryCameraSlingShotSpeed * Vector3.back;
            DrawCurrentPlan();
        }
    }
    protected void DrawCurrentPlan()
    {
        Vector3[] points = new Vector3[2];
        points[0] = transform.position;
        if (actionQueue == ActionQueueTypes.Default)
        {
            ActionParams actionParams = new ActionParams(transform.forward, planScalar, false);
            points[1] = EstimateLaunchDestination(actionParams, transform.position);
        }

        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }
    protected IEnumerator<ActionParams> GetActionQueueEnumerator()
    {
        return actionQueues[actionQueue].GetEnumerator();
    }
    protected Vector3 EstimateLaunchDestination(ActionParams actionParams, Vector3 initPosition)
    {
        float initVelocity = actionParams.scalar/rigidbody.mass;
        while (initVelocity > 0.01)
        {
            initPosition += actionParams.direction* Time.fixedDeltaTime*initVelocity;
            initVelocity = initVelocity * (1 - Time.fixedDeltaTime * rigidbody.drag);
        }
        return initPosition;
    }

    protected void SetUpLineRenderer()
    {
        if (!visualFlag)
        {
            lineRenderer.enabled = false;
            return;
        }
        lineRenderer.enabled = true;
        lineRenderer.startColor = new Color(1,0,1,0); //faded magenta
        lineRenderer.endColor = Color.cyan;
        fadeOutFlag = false;
    }

    protected void FadeOutLineRendererUpdate()
    {
        if (fadeOutFlag)
        {
            if (lineRenderer.enabled && lineRenderer.endColor.a > 0.01)
            {
                lineRenderer.startColor = Color.Lerp(lineRenderer.startColor, Color.clear, Time.deltaTime/1);
                lineRenderer.endColor = Color.Lerp(lineRenderer.endColor, Color.clear, Time.deltaTime/2);
            }
            else
            {
                lineRenderer.SetPositions(new Vector3[0]);
                lineRenderer.positionCount = 0;
                lineRenderer.enabled = false;
                fadeOutFlag = false;
            }
        }

    }

}
