using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : GameBehavior
{
    public GhostController Ghost
    {
        get { return GetComponent<GhostController>(); }
    }
    public ClusterBehavior Cluster;

    public override void Start()
    {
        SetUpSubControllers();
    }

    public void SetUpSubControllers()
    {
        List<Type> subControllers = ReflectionUtils.GetSubClasses(typeof(Controller.SubController));
        foreach(Type subController in subControllers)
        {
            if(GetComponent(subController) == null)
            {
                List<Type> subControllersImpls = ReflectionUtils.GetSubClasses(subController);
                if(subControllersImpls.Count == 0)
                {
                    gameObject.AddComponent(subController);
                }
                else
                {
                    gameObject.AddComponent(subControllersImpls[0]);
                }
            }
            //MethodInfo[] methods = subController.GetMethods();
            //for(int i = 0; i < methods.Length; i++)
            //{
            //    string methodName = methods[i].Name;
            //    //if(methodName.Contains("On"))
            //    //{
            //    //    Debug.Log(methods[i].Name);
            //    //}
            //}
            //gameObject.AddComponent(subController);
        }
    }

    public void OnGhostMode()
    {

    }

    public void LoadJson()
    {
        //string movementFilePath = Application.dataPath + "/InputActionAssets/FlierController.inputactions";
        //string rotationFilePath = Application.dataPath + "/InputActionAssets/BaseRotationSubController.inputactions";
        //string movementJsonFile = File.ReadAllText(movementFilePath);
        //string rotationJsonFile = File.ReadAllText(rotationFilePath);

        //InputActionAsset movementAsset = InputActionAsset.FromJson(movementJsonFile);
        //InputActionAsset rotationAsset = InputActionAsset.FromJson(rotationJsonFile);

        //foreach(InputAction inputAction in rotationAsset.actionMaps[0].actions)
        //{
        //    movementAsset.actionMaps[0].AddAction(name: inputAction.name, type: inputAction.type, expectedControlLayout: inputAction.expectedControlType, processors: inputAction.processors);
        //}

        //foreach (InputBinding inputBinding in rotationAsset.actionMaps[0].bindings)
        //{
        //    movementAsset.actionMaps[0].AddBinding(path: inputBinding.path, interactions: inputBinding.interactions, groups: inputBinding.groups, action: inputBinding.action);
        //}

        //GetComponent<PlayerInput>().actions = movementAsset;
    }

}
