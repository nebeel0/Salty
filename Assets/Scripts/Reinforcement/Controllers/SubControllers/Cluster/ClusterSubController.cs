using Matter.Block.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class ClusterSubController : SubController
    {
        //Possible Actions
        //TODO fire particle
        //TODO fire energy
        ClusterBehavior Cluster
        {
            get { return Player.GetCluster(); }
        }

        public void OnSpecialAbility1()
        {

        }

        public void OnSpecialAbility2()
        {

        }

        public void OnSpecialAbility3()
        {

        }


        //public void SetSize(float scaleFactor)
        //{
        //    foreach (BlockBehavior block in Cluster.blocks)
        //    {
        //        block.transform.localScale = transform.localScale * scaleFactor;
        //    }
        //}

        //public void SetColor(Color color)
        //{
        //    foreach (BlockBehavior block in Cluster.blocks)
        //    {
        //        block.SetColor(color);
        //    }
        //}

        public void Brake()
        {
            foreach (BlockBehavior block in Cluster.blocks)
            {
                block.GetComponent<Rigidbody>().velocity = Vector3.zero;
                block.GetComponent<Rigidbody>().freezeRotation = true;
            }
        }
        public void UnBrake()
        {
            foreach (BlockBehavior block in Cluster.blocks)
            {
                block.GetComponent<Rigidbody>().freezeRotation = false;
            }
        }

    }
}


