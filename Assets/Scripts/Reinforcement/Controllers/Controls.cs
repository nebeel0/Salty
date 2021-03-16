using Unity;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Controls
{
    public string Name;
    public bool Customizable;
    public TypeHashSet RequiredTypes; //Not Editable by Player
    public int CustomSlotIdx;
    public TypeDictionary LockedMapping;
    [SerializeField]
    public List<TypeDictionary> CustomSlots; //Slots holding different subcontroller mappings

    public Controls(string name, bool customizable, TypeHashSet requiredTypes, int customSlotIdx, TypeDictionary lockedMapping, List<TypeDictionary> customSlots)
    {
        Name = name;
        Customizable = customizable;
        RequiredTypes = requiredTypes;
        CustomSlotIdx = customSlotIdx;
        LockedMapping = lockedMapping;
        CustomSlots = customSlots;
    }

    public Controls()
    {
        Name = null;
        Customizable = true;
        RequiredTypes = new TypeHashSet();
        LockedMapping = new TypeDictionary();
        CustomSlots = new List<TypeDictionary>();
    }
    
    public Dictionary<Type, Type> GetCustomMapping()
    {
        if (CustomSlotIdx < CustomSlots.Count)
        {
            return CustomSlots[CustomSlotIdx];
        }
        return new Dictionary<Type, Type>();
    }
}

