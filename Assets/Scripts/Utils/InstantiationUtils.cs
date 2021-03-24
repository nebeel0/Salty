using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InstantiationUtils
{
    public static List<Type> SetUpSubComponents(GameObject gameObject, Type BaseType)
    {
        List<Type> subComponents = BaseType.IsGenericType ? ReflectionUtils.GetSubClassesFromGeneric(BaseType) : ReflectionUtils.GetSubClasses(BaseType);
        foreach (Type subComponent in subComponents)
        {
            if(gameObject.GetComponent(subComponent) == null)
            {
                gameObject.AddComponent(subComponent);
            }
        }
        return subComponents;
    }
}
