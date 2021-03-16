using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace Matter.Block.Base
{
    public abstract class BlockBehavior : GameBehavior
    {
        public ClusterBehavior Cluster;
        public BlockPropertyManager blockPropertyManager;
        public BoxCollider Collider
        {
            get { return GetComponent<BoxCollider>(); }
        }
        public new Rigidbody rigidbody;

        public override void Start()
        {
            if(blockPropertyManager == null)
            {
                blockPropertyManager = gameObject.AddComponent<BlockPropertyManager>();
            }
            blockPropertyManager.Start();
            rigidbody = GetComponent<Rigidbody>();
            SetUpManagers();
        }

        protected abstract void SetUpManagers();

        private void Update()
        {
            if (Cluster == null || !Cluster.blocks.Contains(this)) //TODO move into callback when blocks break links
            {
                Cluster = gameMaster.spawnManager.CreateCluster(new HashSet<BlockBehavior>() { this });
            }
        }

        public virtual void Death()
        {
            transform.DetachChildren();
            Cluster.RemoveBlocks(new HashSet<BlockBehavior>() { this });
        }

        public abstract bool DeathCheck();
        public abstract SlotManagerBehavior GetSlotManager();

    }
}