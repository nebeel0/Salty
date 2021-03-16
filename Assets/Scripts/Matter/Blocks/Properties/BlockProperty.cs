using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace Matter.Block.Property
{
    public abstract class BlockProperty<T> : MonoBehaviour
    {
        protected Base.BlockBehavior Block
        {
            get { return GetComponent<Base.BlockBehavior>(); }
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
        public abstract bool PlayerControllable();

        public virtual void Set(T property)
        {
            if(!ReadOnly())
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