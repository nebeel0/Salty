using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Collections;

namespace Matter.Block.Base
{
    public abstract class SlotManagerBehavior : MonoBehaviour
    {
        public bool slotLockEnabled = false;
        public float attractionFactor = 1f;
        public float displacementFactor = 1.1f;
        public int numFaceVertices = 4; //TODO store in Vector3Utils and determine faceVertices from type of shape.

        public virtual void Start()
        {
            gameObject.name = "Slot Manager";
        }
        public abstract bool IsOccupying();

        public BlockBehavior GetBlock()
        {
            if (transform.parent == null)
            {
                return null;
            }
            else
            {
                return transform.parent.GetComponent<BlockBehavior>();
            }
        }

        public ClusterBehavior Cluster
        {
            get
            {
                if (GetBlock() != null)
                {
                    return GetBlock().Cluster;
                }
                return null;
            }
        }
        public abstract Dictionary<string, SlotBehavior> GetSlots();

        protected abstract void SetUpSlots();

        public abstract void OccupantsUpdate();

        public abstract void ReleaseBlocks();

        public void Death()
        {
            ReleaseBlocks();
            Destroy(gameObject);
        }

    }
}