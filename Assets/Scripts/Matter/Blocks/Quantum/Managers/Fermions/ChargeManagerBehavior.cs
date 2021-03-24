using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChargeManagerBehavior : QuantumBlockManagerBehavior
{
    protected Dictionary<int, HashSet<FermionBehavior>> fermionDictionary
    {
        get { return GetFermionDictionary(); }
    }

    public List<FermionBehavior> GetFermions()
    {
        HashSet<FermionBehavior> allFermionSet = new HashSet<FermionBehavior>();
        foreach(HashSet<FermionBehavior> fermionSet in fermionDictionary.Values)
        {
            allFermionSet.UnionWith(fermionSet);
        }
        return allFermionSet.ToList();
    }

    public Dictionary<int, HashSet<FermionBehavior>> GetFermionDictionary()
    {
        Dictionary<int, HashSet<FermionBehavior>> fermionDictionary = new Dictionary<int, HashSet<FermionBehavior>>();
        for(int i =0; i < transform.childCount; i++)
        {
            FermionBehavior fermion = transform.GetChild(i).GetComponent<FermionBehavior>();
            if (!fermionDictionary.ContainsKey(fermion.charge))
            {
                fermionDictionary[fermion.charge] = new HashSet<FermionBehavior>();
            }
            fermionDictionary[fermion.charge].Add(fermion);
        }
        return fermionDictionary;
    }

    public virtual int GetNetCharge()
    {
        int netCharge = 0;
        foreach (KeyValuePair<int, HashSet<FermionBehavior>> fermion in GetFermionDictionary())
        {
            netCharge += fermion.Key * fermion.Value.Count;
        }
        return netCharge;
    }

    public bool Annihilate(FermionBehavior fermion)
    {
        if(fermionDictionary.ContainsKey(fermion.charge) && fermionDictionary[fermion.charge].Count > 0)
        {
            FermionBehavior thisParticle = fermionDictionary[fermion.charge].First();
            fermion.TriggerPseudoCollision(thisParticle);
            fermion.Free();
            thisParticle.Free();
            return true;
        }
        return false;
    }

    protected FermionBehavior GetFirstFermion(HashSet<FermionBehavior> particleHashSet)
    {
        return particleHashSet.First();
    }

    public virtual bool DeathCheck()
    {
        Debug.LogError("Implement Death Check");
        return false;
    }

    public virtual void Death()
    {
        foreach(HashSet<FermionBehavior> fermions in fermionDictionary.Values)
        {
            foreach (FermionBehavior fermion in fermions)
            {
                fermion.Free();
            }
        }
        transform.DetachChildren();
        Destroy(gameObject);
    }
}
