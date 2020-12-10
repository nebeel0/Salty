using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatterBehavior : MonoBehaviour
{
    public bool playerOverride = false;
    public float collisionCooldown = 0.0f; //tracker for time remaining till cooldown is over
    public int noBlockCollisionLayer = 16;
    public int blockLayer = 8;
    public bool timerEnabled = true;
    public bool colliderEnabled = false;

    protected GameObject gameMaster;
    protected Vector3 gameBounds;
    protected float collisionCooldownTime = 3; // needs three seconds before cooldown is over
    protected float scalingFactor //TODO int
    {
        get { return transform.localScale.x; }  // x,y,z should all be the same
    }
    protected Rigidbody rigidbody
    {
        get { return GetComponent<Rigidbody>(); }
    }
    protected Collider collider
    {
        get { return GetComponent<Collider>(); }
    }


    protected void CollisionCooldownDrain()
    {
        if (timerEnabled && !collider.enabled) //collider not enabled
        {
            //Debug.Log(string.Format("{0} Time Remaining", collisionCooldown));
            if (collisionCooldown > 0)
            {
                collisionCooldown -= 1 * Time.deltaTime;
            }
            else
            {
                collider.enabled = true;
            }
        }
    }

    protected void DisableCollision()
    {
        if (timerEnabled && collider.enabled)
        {
            collider.enabled = false;
            collisionCooldown = collisionCooldownTime; //Start a timer
        }
    }

    protected void DisableCollisionCooldownTimer()
    {
        timerEnabled = false;
    }

    protected void EnableCollisionCooldownTimer()
    {
        timerEnabled = true;
    }

    public virtual void Start()
    {
        gameMaster = GameObject.FindGameObjectWithTag("GameMaster");
        gameBounds = gameMaster.GetComponent<CageBehavior>().dimensions;
        Debug.Log(gameBounds);
    }

    public virtual void Update()
    {
        CollisionCooldownDrain();
        VisualUpdate();
        RefreshState();
        DeathCheck();
        this.colliderEnabled = collider.enabled && timerEnabled;
        BoundCheck();
    }

    protected virtual void VisualUpdate()
    {
        //Debug.Log("No Visual Updates");
    }
    protected virtual void RefreshState()
    {
        Debug.LogError("Override RefreshState");
    }
    protected virtual void DeathCheck()
    {
        Debug.LogError("Override Death Check");
    }
    protected void BoundCheck()  //Going to use the collisions bounds instead way better.
    {
        this.transform.position = new Vector3(
                Mathf.Min(gameBounds.x / 2, transform.position.x),
                Mathf.Min(gameBounds.y / 2, transform.position.y),
                Mathf.Min(gameBounds.z / 2, transform.position.z));
        this.transform.position = new Vector3(
        Mathf.Max(-gameBounds.x / 2, transform.position.x),
        Mathf.Max(-gameBounds.y / 2, transform.position.y),
        Mathf.Max(-gameBounds.z / 2, transform.position.z));
    }

    //Vector Utils
    public static bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }
}
