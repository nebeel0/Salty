using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace Matter
{
    namespace Block
    {
        namespace Property
        {
            public class BlockColor : BlockProperty<Color>
            {
                public override bool PlayerControllable()
                {
                    return true;
                }
                public override bool ReadOnly()
                {
                    return true;
                }
                public override Color Get()
                {
                    return GetComponent<Renderer>().material.color;
                }

                public override void Set(Color property)
                {
                    GetComponent<Renderer>().material.color = property;
                }

            }
        }
    }
}