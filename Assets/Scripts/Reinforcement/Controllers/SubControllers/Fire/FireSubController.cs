using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public abstract class FireSubController : SubController
    {
        //Subcontroller for "Fire" actions, basically right trigger, and mouse left click. Is Press and Release
        public abstract string GetFireDescription();
        bool firing;
        void OnFire(InputValue inputValue)
        {
            OnFire(inputValue.Get<float>() == 1);
        }

        public virtual void OnFire(bool firing)
        {
            this.firing = firing;
        }

        private void Update()
        {
            if(firing)
            {
                FireUpdate();
            }
        }

        public virtual void FireUpdate()
        {
            if (IsBlock())
            {
                //Vector3Utils.RotationalAlign(GetCluster().trackingBlock.gameObject, Player.gimbal.transform);
                Vector3Utils.LerpEulerAngles(Player.gimbal.transform.eulerAngles, GetCluster().trackingBlock.gameObject.transform, 10);
                Player.transform.eulerAngles = Vector3.zero;
            }
        }
    }
}


