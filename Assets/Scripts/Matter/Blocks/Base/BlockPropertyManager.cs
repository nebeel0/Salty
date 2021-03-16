using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace Matter.Block.Base
{
    public class BlockPropertyManager : MonoBehaviour
    {
        public List<Type> properties;

        public void Start()
        {
            properties = ReflectionUtils.GetSubClassesFromGeneric(typeof(Property.BlockProperty<>));
            foreach(Type type in properties)
            {
                if (gameObject.GetComponent(type) == null)
                {
                    gameObject.AddComponent(type);
                }
            }
        }

        public List<string> GetReadableProperties()
        {
            List<string> readableProperties = new List<string>();
            foreach (Type property in properties)
            {
                Debug.Log(GetComponent(property).ToString());
                readableProperties.Add(GetComponent(property).ToString());
            }
            return readableProperties;
        }
    }
}