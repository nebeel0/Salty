using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBehavior : GameBehavior
{
    //Only Particle object can destroy itself
    //Codify instability for higher masses

    //Max 6:3 Quarks or 7 Leptons
    //Eight possible charge spin combinations
    public string particleType;
    public int antiCharge = 1;
    public int weightClass = 1;
    public float energy;

    protected Color particleColor;

    public int charge
    {
        get {return ParticleUtils.possibleTypes[particleType].charge;}
    }
    public int effectiveCharge
    {
        get { return antiCharge * charge; }
    }
    public int spinFactor //Determines number of messages needed to make a decision
    {
        get {return ParticleUtils.possibleTypes[particleType].spinFactor;}
    }
    public int layer
    {
        get {return ParticleUtils.possibleTypes[particleType].layer;}
    }
    public float mass
    {
        get {return ParticleUtils.possibleTypes[particleType].mass * weightClass; }
    }

    public float maxEnergy
    {
        get {return mass*ParticleUtils.maxEnergyFactor;}
    }

    public bool isOverExcited
    {
        get { return energy > maxEnergy; }
    }

    protected SphereCollider particleCollider;
    protected Rigidbody rigidbody
    {
        get { return GetComponent<Rigidbody>(); }
    }

    public virtual void Start()
    {
        particleCollider = GetComponent<SphereCollider>();
        SetParticleColor();
    }

    protected virtual void Update()
    {
    }

    protected virtual void Death()
    {
        //TODO in child classes are callbacks
        // Also base.Death() is always last.
        Destroy(gameObject);
    }

    public void Annihilate(ParticleBehavior otherParticle)
    {
        if(otherParticle.particleType != particleType)
        {
            Debug.LogError("Can't annihilate non-different particles.");
        }
        if(otherParticle.weightClass - weightClass != 0)
        {
            ParticleBehavior remainingParticle = otherParticle.weightClass > weightClass ? otherParticle : this;
            ParticleBehavior annihilatedParticle = otherParticle.weightClass < weightClass ? otherParticle : this;
            remainingParticle.weightClass = remainingParticle.weightClass - annihilatedParticle.weightClass;
            annihilatedParticle.Death();
        }
        else
        {
            otherParticle.Death();
            Death();
        }
    }

    protected Color GetParticleColor()
    {
        Color particleColor = new Color();
        particleColor.r = 1 - (mass / (10 * weightClass)); // Dependent on the mass range
        particleColor.g = ((float)spinFactor / 2);
        particleColor.b = ((float)charge + 3) / 6;
        if (antiCharge < 0)
        {
            particleColor.r = 1 - particleColor.r;
            particleColor.g = 1 - particleColor.g;
            particleColor.b = 1 - particleColor.b;
        }
        return particleColor;
    }

    protected void SetParticleColor()
    {
        GetComponent<Renderer>().material.color = GetParticleColor();
        GetComponent<TrailRenderer>().startColor = GetParticleColor();
    }

    public void SpawnNewParticles(List<ParticleBehavior> particleBehaviors)
    //Set initial positions to be a sum of scale and colliding offset, should be automatically done
    //Set initial velocities, automatically done
    //Set colling to be Enabled
    //Set explosion
    {
        float radius = particleCollider.radius;
        float explosionForce = energy;
        float scalingFactor = transform.localScale.x;

        foreach (ParticleBehavior particleBehavior in particleBehaviors)
        {
            particleBehavior.gameObject.transform.position = Vector3Utils.RandomCircle(transform.position, (radius + 0.1f) * scalingFactor);
        }
        foreach (ParticleBehavior particleBehavior in particleBehaviors)
        {
            particleBehavior.rigidbody.AddExplosionForce(explosionForce: explosionForce, explosionPosition: transform.position, explosionRadius: (radius + 0.2f) * scalingFactor);
        }
    }
}
