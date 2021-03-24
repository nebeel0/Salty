using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Matter.Block.Property.Base;

namespace Matter.Block.Base
{
    public class BlockPropertyManager : MonoBehaviour
    {
        public List<Type> properties;

        public void Start()
        {
            properties = InstantiationUtils.SetUpSubComponents(gameObject, typeof(BlockProperty<>));
            if(BlockUtils.IsQuantumBlock(gameObject))
            {
                properties.AddRange(InstantiationUtils.SetUpSubComponents(gameObject, typeof(QuantumBlockProperty<>)));
            }
            if (BlockUtils.IsClassicalBlock(gameObject))
            {
                properties.AddRange(InstantiationUtils.SetUpSubComponents(gameObject, typeof(ClassicalBlockProperty<>)));
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