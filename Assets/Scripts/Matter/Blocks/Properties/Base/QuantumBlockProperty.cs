using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace Matter.Block.Property.Base
{
    public abstract class QuantumBlockProperty<T> : BlockProperty<T>
    {
        protected QuantumBlockBehavior thisQuantumBlock
        {
            get { return GetComponent<QuantumBlockBehavior>(); }
        }
    }
}