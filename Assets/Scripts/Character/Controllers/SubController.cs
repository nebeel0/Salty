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
        public PlayerController Player
        {
            get { return GetComponent<PlayerController>(); }
        }
        public ClusterBehavior Cluster
        {
            get { return Player.Cluster; }
        }
        //String ProcessorBaseType
        //{
        //    get { return GetType().Name.Replace("SubController", "Processor"); }
        //}

        //void Start()
        //{
        //    if(GetProcessor() == null)
        //    {
        //        List<Type> processors = GetAvailableProcessors();
        //        SetProcessor(processors[0]);
        //    }
        //}

        //public void SetProcessor(Type processor)
        //{
        //    if(GetProcessor() != null)
        //    {
        //        //TODO delete the current processor
        //    }
        //    gameObject.AddComponent(processor);
        //}

        //public Processor GetProcessor()
        //{
        //    return (Processor) GetComponent(ProcessorBaseType);
        //}

        //public List<Type> GetAvailableProcessors()
        //{
        //    List<Type> availableProccessors = Assembly.GetExecutingAssembly().GetTypes().Where(t => String.Equals(t.BaseType.Name, ProcessorBaseType, StringComparison.Ordinal)).ToList();
        //    return availableProccessors;
        //}
    }
}
