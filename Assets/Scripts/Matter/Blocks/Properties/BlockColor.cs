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
            public class BlockColor : BlockProperty<Color>
            {
                static Color neutralColor = new Color(0.5f, 0.5f, 0.5f, 0.1f);
                 
                public override bool ReadOnly()
                {
                    return false;
                }

                private void Start()
                {
                    Set(neutralColor);
                }

                private void Update()
                {
                    Color currentBlockColor = GetComponent<Renderer>().material.color;
                    if (currentBlockColor != Get())
                    {
                        GetComponent<Renderer>().material.color = Color.Lerp(currentBlockColor, Get(), Time.deltaTime * 10);
                    }
                }
            }
        }
    }
}