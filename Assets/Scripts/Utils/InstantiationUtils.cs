using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InstantiationUtils
{
    public static void SetUpManagers(GameObject gameObject, Type BaseType)
    {
        List<Type> managers = ReflectionUtils.GetSubClasses(BaseType);
        foreach(Type manager in managers)
        {
            if(gameObject.GetComponent(manager) == null)
            {
                gameObject.AddComponent(manager);
            }
        }
    }
}
