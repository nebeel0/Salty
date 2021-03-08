using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public abstract class BlockBehavior : GameBehavior
{
    // Overall Manager Class Instance

    // TODO on merge change camera position
    // TODO no mixing of anti and regular particles, when they clash, annihilation must happen, nvm I was wrong
    // TODO rotate on block place
    public static Color defaultColor = new Color(0.4f,0.4f,0.4f,0.06f);
    
    public ClusterBehavior cluster;
    public BoxCollider Collider
    {
        get { return GetComponent<BoxCollider>(); }
    }
    public new Rigidbody rigidbody;

    public void SetColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }

    public override void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        SetUpManagers();
    }

    protected abstract void SetUpManagers();

    private void Update()
    {
        if (cluster == null || !cluster.blocks.Contains(this)) //TODO move into callback when blocks break links
        {
            cluster = gameMaster.spawnManager.CreateCluster(new HashSet<BlockBehavior>() { this });
        }
    }

    public virtual void Death()
    {
        transform.DetachChildren();
        cluster.RemoveBlocks(new HashSet<BlockBehavior>() {this});
    }

    protected virtual void OnCollisionEnter(Collision col)
    {

    }

    public abstract bool DeathCheck();
    public abstract SlotManagerBehavior GetSlotManager();

}
