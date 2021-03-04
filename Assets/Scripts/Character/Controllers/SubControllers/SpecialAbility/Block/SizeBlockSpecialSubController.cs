using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class SizeBlockSpecialSubController : BlockSpecialSubController
    {
        //Take a block, if its big enough splits it into 9.

        public override string GetSpecialAbility1Description()
        {
            return "Enlarge.";
        }

        public override void OnSpecialAbility1()
        {
            GetCluster().trackingBlock.transform.localScale *= 2;
        }

        public override string GetSpecialAbility2Description()
        {
            return "Shrink";
        }
        public override void OnSpecialAbility2()
        {
            GetCluster().trackingBlock.transform.localScale /= 2;
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


