using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuarkGroup
{
    QuarkManagerBehavior manager;

    public static int quarkGroupCapacity = 3;
    public HashSet<QuarkBehavior> quarks = new HashSet<QuarkBehavior>();
    public bool isFull
    {
        get { return quarks.Count == quarkGroupCapacity; }
    }

    public override string ToString()
    {
        string quarkString = "";
        foreach (QuarkBehavior quark in quarks)
        {
            quarkString = quarkString + ", " + quark.particleType;
        }
        return quarkString;
    }

    public QuarkGroup(QuarkBehavior quark, QuarkManagerBehavior manager)
    {
        this.manager = manager;
        AddParticle(quark);
    }

    public int GetNetCharge()
    {
        int netCharge = 0;
        foreach (ParticleBehavior quark in quarks)
        {
            netCharge += quark.effectiveCharge;
        }
        return netCharge;
    }

    public void OnChargeChange(QuarkBehavior quark, int previousCharge)
    {
        quarks.Remove(quark);
        manager.OnParticleChargeChange(quark, previousCharge);
    }

    public void AddParticle(QuarkBehavior quark)
    {
        quarks.Add(quark);
        manager.AddParticle(quark);
        quark.quarkGroup = this;
    }

    public void RemoveParticle(QuarkBehavior quark)
    {
        quarks.Remove(quark);
        manager.RemoveParticle(quark);
        quark.quarkGroup = null;
    }

    public bool Validate(QuarkBehavior quark)
    {
        int netCharge = GetNetCharge();
        bool capacityCheck = (quarks.Count + 1) <= quarkGroupCapacity;
        bool maxTwoSame = quark.effectiveCharge * netCharge < 0;

        int totalCharge = quark.effectiveCharge + netCharge;
        bool totalChargeCheck = totalCharge == 3 || totalCharge == 0;

        bool fullCheck = capacityCheck && maxTwoSame && totalChargeCheck;
        bool notFull = quarks.Count + 1 <= (quarkGroupCapacity - 1);
        return notFull || fullCheck;
    }
}