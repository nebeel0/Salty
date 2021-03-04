using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class ShrinkBlockSpecialSubController : BlockSpecialSubController
    {
        //Take a block, if its big enough splits it into 9.

        public override string GetSpecialAbility1Description()
        {
            return "Shrink by splitting yourself into 9 equal parts, and taking control of the center block. Only works if remaining blocks meet the min size requirements.";
        }

        public override void OnSpecialAbility1()
        {
            Debug.Log("Ima shrink.");
        }

        public override string GetSpecialAbility2Description()
        {
            return "N/A";
        }
        public override void OnSpecialAbility2()
        {

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


