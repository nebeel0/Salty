using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChargeManagerBehavior : QuantumBlockManagerBehavior
{

    public Dictionary<int, HashSet<FermionBehavior>> particleDictionary = new Dictionary<int, HashSet<FermionBehavior>>();

    public virtual int GetNetCharge()
    {
        int netCharge = 0;
        foreach (KeyValuePair<int, HashSet<FermionBehavior>> particles in particleDictionary)
        {
            netCharge += particles.Key * particles.Value.Count;
        }
        return netCharge;
    }

    public void OnParticleChargeChange(FermionBehavior particle, int previousCharge)
    {
        RemoveParticleWithCharge(particle, previousCharge);
    }

    public void AddParticle(FermionBehavior particle)
    {
        ReAddParticle(particle);
        particle.Occupy(gameObject);
    }

    public void ReAddParticle(FermionBehavior particle)
    {
        if (!particleDictionary.ContainsKey(particle.effectiveCharge))
        {
            particleDictionary[particle.effectiveCharge] = new HashSet<FermionBehavior>();
        }
        particleDictionary[particle.effectiveCharge].Add(particle);
    }


    public void RemoveParticle(FermionBehavior particle)
    {
        RemoveParticleWithCharge(particle, particle.effectiveCharge);
    }

    public void RemoveParticleWithCharge(FermionBehavior particle, int charge)
    {
        particleDictionary[charge].Remove(particle);
        particle.Free();
        if (DeathCheck())
        {
            Block.Death();
        }
    }

    protected FermionBehavior GetFirstParticle(HashSet<FermionBehavior> particleHashSet)
    {
        return particleHashSet.First();
    }

    public virtual bool DeathCheck()
    {
        Debug.LogError("Implement Death Check");
        return false;
    }

    public void Death()
    {
        foreach(HashSet<FermionBehavior> particles in particleDictionary.Values)
        {
            foreach (FermionBehavior particle in particles)
            {
                particle.Free();
            }
        }
        transform.DetachChildren();
        Destroy(gameObject);
    }
}
