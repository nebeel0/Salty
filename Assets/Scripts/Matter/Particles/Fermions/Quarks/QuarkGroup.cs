using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuarkGroup
{
    QuarkManagerBehavior quarkManager;
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

    public QuarkGroup(QuarkBehavior quark, QuarkManagerBehavior quarkManager)
    {
        this.quarkManager = quarkManager;
        AddParticle(quark);
    }

    public int GetNetCharge()
    {
        int netCharge = 0;
        foreach (ParticleBehavior quark in quarks)
        {
            netCharge += quark.charge;
        }
        return netCharge;
    }

    public void AddParticle(QuarkBehavior quark)
    {
        quark.Occupy(quarkManager);
        quarks.Add(quark);
        quark.quarkGroup = this;
    }

    public void RemoveParticle(QuarkBehavior quark)
    {
        quarks.Remove(quark);
        quark.quarkGroup = null;
        quark.Free();
    }

    public bool Validate(QuarkBehavior quark)
    {
        int netCharge = GetNetCharge();
        bool capacityCheck = (quarks.Count + 1) <= quarkGroupCapacity;
        bool maxTwoSame = quark.charge * netCharge < 0;

        int totalCharge = quark.charge + netCharge;
        bool totalChargeCheck = totalCharge == 3 || totalCharge == 0;

        bool fullCheck = capacityCheck && maxTwoSame && totalChargeCheck;
        bool notFull = quarks.Count + 1 <= (quarkGroupCapacity - 1);
        return notFull || fullCheck;
    }
}