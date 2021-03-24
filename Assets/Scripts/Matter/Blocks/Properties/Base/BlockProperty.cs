using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace Matter.Block.Property.Base
{
    public abstract class BlockProperty<T> : MonoBehaviour
    {
        protected Block.Base.BlockBehavior Block
        {
            get { return GetComponent<Block.Base.BlockBehavior>(); }
        }

        public string PropertyName
        {
            get
            {
                return GetType().Name;
            }
        }
        private T property;

        public virtual T Get()
        {
            return property;
        }
        public abstract bool ReadOnly();

        public virtual void Set(T property)
        {
            if (!ReadOnly())
            {
                this.property = property;
            }
        }

        public override string ToString()
        {
            return GetType().Name + " : " + property.ToString();
        }
    }
}