using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Matter.Block.Property.Base;

namespace Matter.Block.Property.Quantum
{
    public class Charge : QuantumBlockProperty<float>
    {
        public override bool ReadOnly()
        {
            return true;
        }
        public override float Get()
        {
            return thisQuantumBlock.GetActualNetCharge();
        }

        private void Update()
        {
            BlockColor blockColor = GetComponent<BlockColor>();
            if(blockColor != null && thisQuantumBlock != null)
            {
                blockColor.Set(GetBlockColor());
            }
        }

        private Color GetBlockColor()
        {
            Color blockColor = Color.black;
            if (Get() > 0)
            {
                blockColor.r = 0.5f; // Dependent on the mass range
            }
            else if (Get() < 0)
            {
                blockColor.b = 0.5f;
            }
            else
            {
                blockColor.g = 0.5f;
            }
            blockColor.a = 0.05f;
            return blockColor;
        }

    }
}