using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Matter.Block.Property.Base;

namespace Matter
{
    namespace Block
    {
        namespace Property
        {
            public class Integrity : BlockProperty<float>
            {
                public override bool ReadOnly()
                {
                    return false;
                }

                private void OnCollisionEnter(Collision collision)
                {
                    float collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;
                    Debug.Log("Force: " + collisionForce.ToString());
                    if (collisionForce < 100.0F)
                    {
                        Debug.Log("This collision has not damaged anyone...");
                    }
                    else if (collisionForce < 200.0F)
                    {
                        Debug.Log("Auch! This will take some damage.");
                        
                    }
                    else
                    {
                        Debug.Log("This collision killed me!");
                    }
                }
            }
        }
    }
}