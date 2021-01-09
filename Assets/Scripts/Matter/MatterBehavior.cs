using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatterBehavior : GameBehavior
{
    public bool playerOverride = false;
    public float collisionCooldown = 0.0f; //tracker for time remaining till cooldown is over
    public bool timerEnabled = true;
    public bool colliderEnabled = false;

    protected float collisionCooldownTime = 0.5f; // needs three seconds before cooldown is over
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

    protected virtual void Start()
    {
    }
    public virtual void Update()
    {
        CollisionCooldownDrain();
        this.colliderEnabled = collider.enabled && timerEnabled;
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
}
