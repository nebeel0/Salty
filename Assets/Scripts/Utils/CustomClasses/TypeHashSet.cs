using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class TypeHashSet : HashSet<Type>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<string> keys = new List<string>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        foreach (Type key in this)
        {
            keys.Add(key.FullName);
            Debug.Log(keys.ToString());
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(Type.GetType(keys[i]));
        }
    }
}

