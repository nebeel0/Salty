using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatterBehavior : MonoBehaviour
{
    public bool playerOverride = false;
    public float collisionCooldown = 0.0f; //tracker for time remaining till cooldown is over
    protected float collisionCooldownTime = 100f; // needs three seconds before cooldown is over
    protected float scalingFactor
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
    public bool timerEnabled=true;
    public bool colliderEnabled=false;
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

    public int noBlockCollisionLayer = 16;
    public int blockLayer = 8;

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

    void Update()
    {
        CollisionCooldownDrain();
        VisualUpdate();
        RefreshState();
        DeathCheck();
        this.colliderEnabled = collider.enabled && timerEnabled;
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

    //Vector Utils
    public bool V3Equal(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.0001;
    }
}
