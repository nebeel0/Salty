using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace Matter.Block.Property.Base
{
    public abstract class ClassicalBlockProperty<T> : BlockProperty<T>
    {
        protected ClassicalBlockBehavior ClassicalBlock
        {
            get { return GetComponent<ClassicalBlockBehavior>(); }
        }
    }
}