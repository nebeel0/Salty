using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public abstract class SpecialSubController : SubController
    {
        public abstract string GetSpecialAbility1Description();
        public abstract string GetSpecialAbility2Description();
        public abstract string GetSpecialAbility3Description();

        public abstract void OnSpecialAbility1();


        public abstract void OnSpecialAbility2();


        public abstract void OnSpecialAbility3();

    }
}


