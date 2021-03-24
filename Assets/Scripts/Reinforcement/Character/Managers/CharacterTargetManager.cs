using Character.Managers.Base;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Character.Managers
{
    public class CharacterTargetManager : CharacterManagerBehavior
    {
        List<GameObject> targets = new List<GameObject>();

        public bool HasTargets()
        {
            return targets.Count > 0;
        }

        public void SetTarget(GameObject target)
        {
            targets = new List<GameObject>() { target };
        }

        public void SetTargets(List<GameObject> targets)
        {
            this.targets = targets;
        }

        public Vector3 GetTargetsCenter()
        {
            if(targets.Count == 0)
            {
                return transform.position;
            }
            else
            {
                Vector3 center = Vector3.zero;
                foreach (GameObject gameObject in targets)
                {
                    center += gameObject.transform.position;
                }
                return center;
            }
        }
    }
}