using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReflectionUtils
{
    public static List<Type> GetSubClasses(Type BaseType)
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.BaseType.Equals(BaseType)).ToList();
    }
    public static List<Type> GetSubClassesFromGeneric(Type GenericBaseType)
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition().Equals(GenericBaseType)).ToList();
    }

    public static HashSet<Type> GetAllGenericArguments(Type GenericBaseType)
    {
        HashSet<Type> genericArguments = new HashSet<Type>();
        List<Type> subclasses = GetSubClassesFromGeneric(GenericBaseType);
        foreach(Type subclass in subclasses)
        {
            foreach (Type genericArgument in subclass.BaseType.GetGenericArguments())
            {
                genericArguments.Add(genericArgument);
            }
        }
        return genericArguments;
    }

}

