using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BosonBehavior : ParticleBehavior
{
    protected override void Start()
    {
        base.Start();
        particleCollider.isTrigger = true;
    }

    protected override void Update()
    {
        base.Update();
        //Apply a force in transform.direction with remaining energy, and subtract based off amount of mass
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
    }

    protected virtual void OnTriggerStay(Collider other)
    {
    }
    protected virtual void OnTriggerExit(Collider other)
    {
    }

    //public void BosonDecay() // Initial pos outside of collision range. Create the new particles, offset, give initial velocities the same as curr, and create explosion force
    //{
    //    if (ParticleUtils.isPhoton(gameObject) || ParticleUtils.isFermion(gameObject))
    //    {
    //        Debug.LogError(string.Format("Only Z and W Bosons can split. Not {0}", particleType));
    //    }
    //    List<ParticleBehavior> particleBehaviors = new List<ParticleBehavior>();
    //    if (charge != 0) //wBoson
    //    {
    //        if (MassDecayChance(this))
    //        {
    //            particleBehaviors.Add(gameMaster.CreateElectron(mass: mass, energy: energy / 2, antiCharge * -1));
    //            if (mass > ((float)mass / 2) && mass > 100)
    //            {
    //                particleBehaviors.Add(gameMaster.CreateQuarkNeg(mass: mass / 2, energy: energy / 2, antiCharge));
    //                particleBehaviors.Add(gameMaster.CreateQuarkPos(mass: mass / 2, energy: energy / 2, antiCharge));
    //            }
    //            else
    //            {
    //                particleBehaviors.Add(gameMaster.CreateNeutrino(mass: 1, energy: energy / 2));
    //            }
    //        }
    //        else
    //        {
    //            particleBehaviors.Add(gameMaster.CreateElectron(mass: mass, energy: energy, antiCharge * -1));
    //        }
    //    }
    //    else if (charge == 0) //zBoson
    //    {
    //        if (MassDecayChance(this))
    //        {
    //            for (int i = 0; i < 2; i++)
    //            {
    //                particleBehaviors.Add(gameMaster.CreateNeutrino(mass: 1, energy: energy / 2));
    //            }
    //            gameMaster.CreatePhoton(energy: mass * 100); //Photons have their own movement system.
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError(string.Format("Okay only W and Z bosons should be decaying. Not {0}", particleType));
    //    }
    //    SpawnNewParticles(particleBehaviors);
    //    Debug.Log("Destroying the gameobject via bosonDecay");
    //    Death();
    //}

    //void BosonRadiation(BosonBehavior boson)
    //// If particles are anti annihilate
    //{
    //    Debug.Log("AbsorbMass");
    //    float minMass = maxMass * 0.1f;
    //    float remainingMass = maxMass - mass;
    //    float massCap = boson.mass;
    //    if (remainingMass > massCap)
    //    {
    //        massCap = remainingMass;
    //    }
    //    int absorbedMass = (int)Random.Range(massCap / 4, massCap);
    //    boson.mass -= absorbedMass;
    //    mass += absorbedMass;
    //    if (boson.charge != 0)
    //    {
    //        int newCharge = (boson.charge * boson.antiCharge) + (charge * antiCharge);
    //        if (newCharge < 0)
    //        {
    //            antiCharge *= -1;
    //            boson.antiCharge *= -1;
    //        }
    //    }
    //}

    protected void TransferEnergy(ParticleBehavior particle)
    // All particles absorb energy. Energy is a scalar.
    {
        float remainingEnergy = maxEnergy - energy;
        float energyCap = particle.energy;
        if (remainingEnergy > energyCap)
        {
            energyCap = remainingEnergy;
        }
        float absorbedEnergy = Random.Range(energyCap / 4, energyCap);
        particle.energy -= absorbedEnergy;
        energy += absorbedEnergy;
    }

    protected void TransferMass(ParticleBehavior particle)
    // All particles absorb energy. Energy is a scalar.
    {
        float remainingEnergy = maxEnergy - energy;
        float energyCap = particle.energy;
        if (remainingEnergy > energyCap)
        {
            energyCap = remainingEnergy;
        }
        float absorbedEnergy = Random.Range(energyCap / 4, energyCap);
        particle.energy -= absorbedEnergy;
        energy += absorbedEnergy;
    }

    protected virtual bool DeathCheck()
    {
        return energy <= 0;
    }

}
