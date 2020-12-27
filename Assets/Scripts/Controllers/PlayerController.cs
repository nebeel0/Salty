using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : Controller
{
    GameObject superBlock;
    protected SuperBlockBehavior superBlockBehavior
    {
        get { return superBlock.GetComponent<SuperBlockBehavior>(); }
    }

    protected bool visualToggle = true;
    protected bool lockOn = false;
    protected bool thirdPerson = false;
    protected bool firstPerson = true;
    protected bool godMode = false;
    protected bool blockMode = false;
    protected bool perspectiveAlign = false;

    protected bool fadeOutFlag = false;
    protected new Rigidbody rigidbody;
    protected LineRenderer lineRenderer;

    protected Vector3 cameraToBlockDirection
    {
        get
        {
            return (superBlock.transform.position - primaryCamera.transform.position).normalized;
        }
    }

    //TODO collision frequency
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

    void OnEnable()
    {
        Start();
    }

    public override void Start()
    {
        gameObject.tag = "Player";
        Destroy(GetComponent<Rigidbody>());
        GetComponent<SphereCollider>().enabled = false;
        if (transform.parent.gameObject.CompareTag("SuperBlock"))
        {
            superBlock = transform.parent.gameObject;
        }
        if (superBlock != null)
        {
            base.Start();
            OnFirstPerson();
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;

            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.enabled = false;
            lineRenderer.positionCount = 0;
            foreach (ActionQueueTypes aq in (ActionQueueTypes[])System.Enum.GetValues(typeof(ActionQueueTypes)))
            {
                actionQueues[aq] = new Queue<ActionParams>();
            }
        }
    }

    protected override void Update()
    {
        DeathCheck();
        if(superBlock != null)
        {
            base.Update();
            ResetOrientationUpdate();
            CameraUpdate();
            if(!godMode)
            {
                ActionQueueUpdate();
            }
            VisualUpdate();
        }
    }


    protected void DeathCheck()
    {
        if(superBlock == null)
        {
            Destroy(gameObject);
        }
    }

    protected void CameraUpdate()
    {
        if (firstPerson) //TODO some sort of animation effect to show the distinction
        {
            primaryCamera.transform.localPosition = primaryCameraRootPosition; //Vector3.zero should be Camera root position

            m_TargetCameraState.SetFromEulerAngles(transform.localEulerAngles);
            m_TargetCameraState.LerpTowardsZero(1);
            m_TargetCameraState.UpdateLocalTransform(transform);
        }
        else
        {
            if(!godMode)
            {
                float displacement = Mathf.Max(4, superBlockBehavior.prevBlockCount*1.5f);
                Vector3 finalCameraPosition = primaryCameraRootPosition + Vector3.back * displacement;
                primaryCamera.transform.localPosition = Vector3.Lerp(primaryCamera.transform.localPosition, finalCameraPosition, 1);
                transform.position = superBlock.transform.position;
            }
        }
    }

    protected void ActionQueueUpdate()
    {
        //Only when executing do we set up our actions
        HoldUpdate();
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
            superBlockBehavior.DistributeForce(direction * scalar, ForceMode.Impulse);
        }
    }

    protected override void OnHold() // Press and Release
    {
        holdFlag = !holdFlag; //Sets to hold
        if (!enabled)
        {
            return;
        }
        if (godMode)
        {
            return;
        }

        if (holdFlag)
        {
            SetUpLineRenderer();
        }
        else if (!holdFlag && holdScalar > 0)
        {
            Vector3 direction = transform.forward; //direction is just where the player is looking
            float scalar = holdScalar;
            holdScalar = 0; //Reset

            ActionParams actionParams = new ActionParams(direction, scalar, lockOn);

            if (actionQueueMode == ActionQueueModeTypes.Add)
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

    protected override void HoldUpdate()
    {
        if (holdFlag)
        {
            holdScalar += 1 + holdScalar * (float).25 * Time.deltaTime;
            DrawCurrentPath();
        }
    }

    void OnAddMode()
    {
        if (!enabled)
        {
            return;
        }
        if (actionQueueMode == ActionQueueModeTypes.Add)
        {
            actionQueueMode = ActionQueueModeTypes.Default;
        }
        else
        {
            actionQueueMode = ActionQueueModeTypes.Add;
        }
    }

    void OnFire()
    {
        if (!enabled)
        {
            return;
        }
        if (actionQueues[actionQueue].Count > 0)
        {
            ActionRouter(actionQueues[actionQueue].Dequeue());
        }
    }
    void OnVisualToggle()
    {
        if (!enabled)
        {
            return;
        }
        visualToggle = !visualToggle;
    }

    void OnFirstPerson()
    {
        if (!enabled)
        {
            return;
        }
        firstPerson = true;
        thirdPerson = false;
        target = superBlock.transform;
        ResetParenting();
    }

    void OnThirdPerson()
    {
        if (!enabled)
        {
            return;
        }
        thirdPerson = true;
        firstPerson = false;
        target = transform;
    }

    void OnBlockMode()  //Rotates the block
    {
        if (!enabled)
        {
            return;
        }
        //if(thirdPerson && !godMode)
        //{
        //    blockMode = !blockMode;
        //    if(blockMode)
        //    {
        //        target = mainBlock.transform;
        //        transform.parent = null;
        //    }
        //    else
        //    {
        //        target = transform;
        //        transform.SetParent(mainBlock.transform);
        //    }
        //}
    }

    void OnPerspectiveAlign()  //Rotates the block
    {
        if (!enabled)
        {
            return;
        }
        if (thirdPerson)
        {
            perspectiveAlign = true;
        }
    }

    void OnGodMode() //Moves the block
    {
        if (!enabled)
        {
            return;
        }
        if (thirdPerson && !blockMode)
        {
            godMode = !godMode;
            if (godMode)
            {
                superBlockBehavior.Brake();
                target = transform;
                transform.parent = null;
                primaryCamera.transform.parent = null;
                transform.position = primaryCamera.transform.position;
                primaryCamera.transform.parent = transform;
                superBlock.transform.SetParent(transform);
            }
            else
            {
                superBlockBehavior.UnBrake();
                target = transform;
                ResetParenting();
            }
        }
    }

    void ResetParenting()
    {
        superBlock.transform.parent = null;
        transform.parent = null;
        primaryCamera.transform.parent = null;

        transform.position = superBlock.transform.position;
        transform.SetParent(superBlock.transform);
        primaryCamera.transform.SetParent(transform);
    }

    void OnDrawPath() // TODO slingshot effect, instead of drawing line, pull camera back
    {
        if (!enabled)
        {
            return;
        }
        SetUpLineRenderer();
        if (actionQueues[actionQueue].Count != lineRenderer.positionCount - 1 && actionQueues[actionQueue].Count > 0)
        {
            Vector3 positionTracker = superBlock.transform.position; //tracker for default action queue
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

    protected void DrawCurrentPath()
    {
        Vector3[] points = new Vector3[2];
        points[0] = superBlock.transform.position;
        if (actionQueue == ActionQueueTypes.Default)
        {
            ActionParams actionParams = new ActionParams(transform.forward, holdScalar, false);
            points[1] = EstimateLaunchDestination(actionParams, superBlock.transform.position);
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
        float initVelocity = actionParams.scalar / superBlockBehavior.totalMass;
        while (initVelocity > 0.01) // TODO stop when block slows down, if successive positions
        {
            initPosition += actionParams.direction * Time.fixedDeltaTime * initVelocity;
            initVelocity = initVelocity * (1 - Time.fixedDeltaTime * superBlockBehavior.averageDrag);
        }
        return initPosition;
    }

    protected void SetUpLineRenderer()
    {
        if (!visualToggle)
        {
            lineRenderer.enabled = false;
            return;
        }
        lineRenderer.enabled = true;
        lineRenderer.startColor = new Color(1, 0, 1, 0); //faded magenta
        lineRenderer.endColor = Color.cyan;
        fadeOutFlag = false;
    }

    protected void FadeOutLineRendererUpdate()
    {
        if (fadeOutFlag)
        {
            if (lineRenderer.enabled && lineRenderer.endColor.a > 0.01)
            {
                lineRenderer.startColor = Color.Lerp(lineRenderer.startColor, Color.clear, Time.deltaTime);
                lineRenderer.endColor = Color.Lerp(lineRenderer.endColor, Color.clear, Time.deltaTime);
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
