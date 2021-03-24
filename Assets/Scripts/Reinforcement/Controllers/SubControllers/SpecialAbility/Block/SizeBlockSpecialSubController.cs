using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class SizeBlockSpecialSubController : BlockSpecialSubController
    {
        //Take a block, if its big enough splits it into 9.

        Vector3 desiredScale;

        private void Start()
        {
            desiredScale = GetCluster().trackingBlock.transform.localScale;
        }

        private void Update()
        {
            if(transform.localScale != desiredScale && GetCluster() != null && GetCluster().trackingBlock != null)
            {
                GetCluster().trackingBlock.transform.localScale = Vector3.Lerp(GetCluster().trackingBlock.transform.localScale, desiredScale, Time.deltaTime * 10);
            }
        }

        public override string GetSpecialAbility1Description()
        {
            return "Enlarge.";
        }

        public override void OnSpecialAbility1()
        {
            //TODO round
            desiredScale *= 2;
        }

        public override string GetSpecialAbility2Description()
        {
            return "Shrink";
        }
        public override void OnSpecialAbility2()
        {
            desiredScale /= 2;
        }

        public override string GetSpecialAbility3Description()
        {
            return "";
        }

        public override void OnSpecialAbility3()
        {

        }
    }
}


