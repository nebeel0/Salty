using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace Matter.Block.Property
{
    public abstract class BlockProperty<T> : MonoBehaviour
    {
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
        public abstract bool CanSet();

        public virtual void Set(T property)
        {
            if(CanSet())
            {
                this.property = property;
            }
        }
    }
}