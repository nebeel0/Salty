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

}

