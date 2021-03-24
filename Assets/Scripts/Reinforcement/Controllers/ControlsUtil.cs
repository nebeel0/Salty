using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ControlsUtil
{
    //Default Controls

    public static Controls UniversalControls()
    {
        Controls controls = new Controls();
        controls.Customizable = true;
        controls.Name = "ReplaceMe";
        controls.LockedMapping[typeof(Controller.PerspectiveSubController)] = typeof(Controller.PerspectiveSubController);
        controls.LockedMapping[typeof(Controller.RotationSubController)] = typeof(Controller.RotationSubController);
        controls.LockedMapping[typeof(Controller.UISubController)] = typeof(Controller.UISubController);
        return controls;
    }

    public static Controls GhostControls()
    {
        Controls controls = UniversalControls();
        controls.Customizable = false;
        controls.Name = "Ghost";
        controls.LockedMapping[typeof(Controller.MovementSubController)] = typeof(Controller.FlyMovementSubController);
        controls.LockedMapping[typeof(Controller.GhostSubController)] = typeof(Controller.GhostSubController);
        return controls;
    }

    public static Controls DefaultBlockControls()
    {
        Controls controls = UniversalControls();
        controls.Customizable = false;
        controls.Name = "DefaultBlock";
        controls.LockedMapping[typeof(Controller.ClusterSubController)] = typeof(Controller.ClusterSubController);
        controls.CustomSlots.Add(DefaultCustomSlot());
        controls.CustomSlots.Add(FlyCustomSlot());
        controls.CustomSlots.Add(BlockCustomSlot_Fracture());
        controls.CustomSlots.Add(BlockCustomSlot_Size());
        controls.CustomSlots.Add(BlockCustomSlot_Laser());
        return controls;
    }

    public static Controls DefaultClusterControls()
    {
        Controls controls = UniversalControls();
        controls.Customizable = false;
        controls.Name = "DefaultCluster";
        controls.LockedMapping[typeof(Controller.ClusterSubController)] = typeof(Controller.ClusterSubController);
        controls.CustomSlots.Add(DefaultCustomSlot());
        controls.CustomSlots.Add(ClusterCustomSlot_Base());
        controls.CustomSlots.Add(FlyCustomSlot());
        return controls;
    }

    public static TypeDictionary DefaultCustomSlot()
    {
        TypeDictionary customDictionary = new TypeDictionary();
        customDictionary[typeof(Controller.MovementSubController)] = typeof(Controller.FlyMovementSubController);
        customDictionary[typeof(Controller.FireSubController)] = typeof(Controller.SelectFireSubController);
        return customDictionary;
    }

    public static TypeDictionary FlyCustomSlot()
    {
        TypeDictionary customDictionary = new TypeDictionary();
        customDictionary[typeof(Controller.MovementSubController)] = typeof(Controller.FlyMovementSubController);
        return customDictionary;
    }

    public static TypeDictionary BlockCustomSlot_Fracture()
    {
        TypeDictionary customDictionary = new TypeDictionary();
        customDictionary[typeof(Controller.MovementSubController)] = typeof(Controller.FlyMovementSubController);
        customDictionary[typeof(Controller.BlockSpecialSubController)] = typeof(Controller.FractureBlockSpecialSubController);
        return customDictionary;
    }

    public static TypeDictionary BlockCustomSlot_Size()
    {
        TypeDictionary customDictionary = new TypeDictionary();
        customDictionary[typeof(Controller.MovementSubController)] = typeof(Controller.FlyMovementSubController);
        customDictionary[typeof(Controller.BlockSpecialSubController)] = typeof(Controller.SizeBlockSpecialSubController);
        return customDictionary;
    }

    public static TypeDictionary BlockCustomSlot_Laser()
    {
        TypeDictionary customDictionary = new TypeDictionary();
        customDictionary[typeof(Controller.MovementSubController)] = typeof(Controller.FlyMovementSubController);
        customDictionary[typeof(Controller.FireSubController)] = typeof(Controller.LaserFireSubController);
        return customDictionary;
    }


    public static TypeDictionary ClusterCustomSlot_Base()
    {
        TypeDictionary customDictionary = new TypeDictionary();
        customDictionary[typeof(Controller.MovementSubController)] = typeof(Controller.FlyMovementSubController);
        customDictionary[typeof(Controller.ClusterSpecialSubController)] = typeof(Controller.BaseClusterSpecialSubController);
        return customDictionary;
    }
    //String Utils
    public static string StripSubType(Type type)
    {
        bool sameAsBaseType = type.BaseType.Name == type.Name;
        bool containBaseType = type.Name.Contains(type.BaseType.Name);
        if (sameAsBaseType)
        {
            Debug.LogError("Custom controller types should not be the same as their base type.");
        }
        else if(!containBaseType)
        {
            Debug.LogError("Custom controller types need to have part of their name come from base type.");
        }
        return type.Name.Replace(type.BaseType.Name, "");
    }

    //Validation Utils
    public static bool ValidateControls(Controls controls)
    {
        return ValidateCustomSlots(controls) && ValidateRequiredTypes(controls) && ValidateTypeMappings(controls);
    }

    public static bool ValidateRequiredTypes(Controls controls)
    {
        if(controls.CustomSlots.Count == 0)
        {
            foreach (Type subControllerType in controls.RequiredTypes)
            {
                bool containsRequiredSubController = controls.LockedMapping.ContainsKey(subControllerType);
                if(!containsRequiredSubController)
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            foreach (Dictionary<Type, Type> customSlot in controls.CustomSlots)
            {
                foreach (Type subControllerType in controls.RequiredTypes)
                {
                    bool customContains = customSlot.ContainsKey(subControllerType);
                    bool lockedContains = controls.LockedMapping.ContainsKey(subControllerType);
                    bool containsRequiredSubController = customContains || lockedContains;
                    if (!containsRequiredSubController)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public static bool ValidateCustomSlots(Controls controls)
    {
        foreach (Dictionary<Type, Type> customSlot in controls.CustomSlots)
        {
            foreach (KeyValuePair<Type, Type> subControllerPair in customSlot)
            {
                bool isLocked = controls.LockedMapping.ContainsKey(subControllerPair.Key);
                if (isLocked)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static bool ValidateTypeMappings(Controls controls)
    {
        foreach (Dictionary<Type, Type> customSlot in controls.CustomSlots)
        {
            foreach (KeyValuePair<Type, Type> subControllerPair in customSlot)
            {
                bool sameType = subControllerPair.Key == subControllerPair.Value;
                bool inheritedType = subControllerPair.Key == subControllerPair.Value.BaseType;
                if (!(sameType || inheritedType))
                {
                    return false;
                }
            }
        }

        foreach (KeyValuePair<Type, Type> subControllerPair in controls.LockedMapping)
        {
            bool sameType = subControllerPair.Key == subControllerPair.Value;
            bool inheritedType = subControllerPair.Key == subControllerPair.Value.BaseType;
            if (!(sameType || inheritedType))
            {
                return false;
            }
        }
        return true;
    }

    //Saving and Loading Utils
    public static void SaveControls(Controls controls)
    {
        string jsonData = JsonUtility.ToJson(controls);
        string filePath = Application.persistentDataPath + "/" + controls.Name + "Controls.json";
        File.WriteAllText(path: filePath, contents: jsonData);
        Debug.Log("Saved to " + filePath);
    }

    public static void LoadControls(string file)
    {
        //List<Type> subControllers = ReflectionUtils.GetSubClasses(typeof(Controller.SubController));
        //foreach (Type subController in subControllers)
        //{
        //    if (GetComponent(subController) == null)
        //    {
        //        List<Type> subControllersImpls = ReflectionUtils.GetSubClasses(subController);
        //        if (subControllersImpls.Count == 0)
        //        {
        //            gameObject.AddComponent(subController);
        //        }
        //        else
        //        {
        //            gameObject.AddComponent(subControllersImpls[0]);
        //        }
        //    }
        //}
    }


}

