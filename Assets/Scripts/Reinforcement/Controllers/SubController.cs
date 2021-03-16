using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Controller
{
    public class SubController : MonoBehaviour
    {
        public PlayerControlManager Player
        {
            get { return GetComponent<PlayerControlManager>(); }
        }
        public ClusterBehavior GetCluster()
        {
            return Player.GetCluster();
        }
        public bool IsGhost()
        {
            return Player.IsGhost();
        }
        public bool IsBlock()
        {
            return Player.IsBlock();
        }
        public bool IsCluster()
        {
            return Player.IsCluster();
        }

    }
}
