using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : Controller
{
    public ClusterBehavior cluster;
    public CharacterBehavior character;

    protected bool visualToggle = true;
    protected bool lockOn = false;
    protected bool thirdPerson = false;
    protected bool firstPerson = true;
    protected bool godMode = false;
    protected bool perspectiveAlign = false;

    protected bool fadeOutFlag = false;
    protected new Rigidbody rigidbody;
    protected LineRenderer lineRenderer;

    protected Vector3 cameraToBlockDirection
    {
        get
        {
            return (cluster.transform.position - primaryCamera.transform.position).normalized;
        }
    }

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
    protected Dictionary<ActionQueueTypes, Queue<ActionParams>> actionQueues = new Dictionary<ActionQueueTypes, Queue<ActionParams>>();  //Maybe include time stamp so we can also execute them all in the order they were created


    void OnEnable()
    {
        Start();
    }

    public void Reset()
    {
        if(cluster != null)
        {
            cluster.players.Remove(this);
        }
        cluster = null;
    }

    public override void Start()
    {
        gameObject.tag = "Player";
        if(rigidbody != null)
        {
            Destroy(GetComponent<Rigidbody>());
        }
        GetComponent<SphereCollider>().enabled = false;
        base.Start();
        OnFirstPerson();
        ResetParenting();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;
        foreach (ActionQueueTypes aq in (ActionQueueTypes[])System.Enum.GetValues(typeof(ActionQueueTypes)))
        {
            actionQueues[aq] = new Queue<ActionParams>();
        }
    }

    protected override void Update()
    {
        DeathCheck();
        base.Update();
        if(!godMode)
        {
            ActionQueueUpdate();
        }
        VisualUpdate();
    }

    protected void DeathCheck()
    {
        if(cluster == null)
        {
            Destroy(gameObject);
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
            cluster.DistributeForce(direction * scalar, ForceMode.Impulse);
        }
    }

    protected override void OnHold() // Press and Release
    {
        holdFlag = !holdFlag; //Sets to hold
        if (!enabled || godMode)
        {
            return;
        }

        if (holdFlag)
        {
            SetUpLineRenderer();
            HoldParenting();
        }
        else if (!holdFlag && holdScalar > 0)
        {
            ResetParenting();
            Vector3 direction = cluster.transform.forward; //direction is where the block is facing
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
            Vector3 newAngles = primaryCamera.transform.eulerAngles;
            Vector3Utils.LerpEulerAngles(newAngles: newAngles, objectToLerp: cluster.transform, percentage: 10);

            ResetParenting();
            HoldParenting();
        }
    }

    void OnGhostMode()
    {
        if (!enabled)
        {
            return;
        }
        GetComponent<GhostPlayerController>().enabled = true;
        enabled = false;
    }

    void OnAddMode()
    {
        if (enabled)
        {
            if (actionQueueMode == ActionQueueModeTypes.Add)
            {
                actionQueueMode = ActionQueueModeTypes.Default;
            }
            else
            {
                actionQueueMode = ActionQueueModeTypes.Add;
            }
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
        if (!enabled || godMode)
        {
            return;
        }
        firstPerson = true;
        thirdPerson = false;
        ResetParenting();
    }

    void OnThirdPerson()
    {
        if (!enabled || godMode)
        {
            return;
        }
        thirdPerson = true;
        firstPerson = false;
        ResetParenting();
    }

    public void UpdateCameraOffset() //TODO subscribe to Add block, and recalcute camera offset
    {
        transform.position = cluster.transform.position;
        if(firstPerson)
        {
            primaryCamera.transform.localPosition = primaryCameraRootPosition;
        }
        else
        {
            float displacement = Mathf.Max(cluster.diagonal * 4, 4);
            primaryCamera.transform.localPosition = primaryCameraRootPosition + Vector3.back * displacement;
        }
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
        godMode = !godMode;
        if (!enabled || holdFlag)
        {
            return;
        }
        if (thirdPerson)
        {
            if (godMode)
            {
                cluster.Brake();
                GodParenting();
            }
            else
            {
                cluster.UnBrake();
                ResetParenting();
            }
        }
    }

    void HoldParenting()
    {
        transform.parent = null;
        cluster.transform.parent = transform;
        foreach (BlockBehavior block in cluster.blocks)
        {
            block.transform.SetParent(cluster.transform);
        }
        target = transform;
    }


    //blocks -> cluster -> transform -> camera
    void GodParenting()
    {
        HoldParenting();
        primaryCamera.transform.parent = null;
        transform.parent = primaryCamera.transform;
        target = primaryCamera.transform;
    }

    //camera -> transform -> tracking block
    public void ResetParenting()
    {
        foreach (BlockBehavior block in cluster.blocks)
        {
            block.transform.SetParent(null);
        }
        primaryCamera.transform.parent = null;
        transform.parent = null;
        cluster.transform.parent = null;


        cluster.UpdatePosition();
        primaryCamera.transform.parent = transform;
        transform.parent = cluster.transform;
        target = transform;
        UpdateCameraOffset();
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
            Vector3 positionTracker = cluster.trackingBlock.transform.position; //tracker for default action queue
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
        points[0] = cluster.trackingBlock.transform.position;
        if (actionQueue == ActionQueueTypes.Default)
        {
            ActionParams actionParams = new ActionParams(cluster.transform.forward, holdScalar, false);
            points[1] = EstimateLaunchDestination(actionParams, points[0]);
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
        float initVelocity = actionParams.scalar / cluster.totalMass;
        while (initVelocity > 0.01) // TODO stop when block slows down, if successive positions
        {
            initPosition += actionParams.direction * Time.fixedDeltaTime * initVelocity;
            initVelocity = initVelocity * (1 - Time.fixedDeltaTime * cluster.averageDrag);
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
