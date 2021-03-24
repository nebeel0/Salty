using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class SelectFireSubController : FireSubController
    {
        //Subcontroller for "Fire" actions, basically right trigger, and mouse left click. Is Press and Release
        public override string GetFireDescription()
        {
            return "Select blocks.";
        }

        protected override void Update()
        {
            
        }

        void OnSelect()
        {
            RaycastHit hit;
            Ray ray = Player.primaryCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject objectHit = hit.transform.gameObject;
                if (BlockUtils.IsBlock(objectHit) && objectHit != GetCluster().trackingBlock.gameObject)
                {
                    objectHit.GetComponent<Outline>().enabled = !objectHit.GetComponent<Outline>().enabled;
                    Matter.Block.Base.BlockBehavior hitBlock = objectHit.GetComponent<Matter.Block.Base.BlockBehavior>();
                    PlayerControlManager hitPlayer = BlockUtils.GetPlayer(hitBlock);
                    if (hitPlayer != null)
                    {
                        hitPlayer.Character.ParentPlayer = Player;
                    }
                }
                // Do something with the object that was hit by the raycast.
            }
        }

    }
}


