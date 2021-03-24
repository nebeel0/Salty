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
            public class Mass : BlockProperty<float>
            {
                public override bool ReadOnly()
                {
                    return false;
                }
                public override float Get()
                {
                    return GetComponent<Rigidbody>().mass;
                }
            }
        }
    }
}