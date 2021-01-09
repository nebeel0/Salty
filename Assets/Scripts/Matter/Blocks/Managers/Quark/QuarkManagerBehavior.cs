using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class QuarkManagerBehavior : ChargeManagerBehavior
{
    //net charge only valid if quark group is maxed out
    public List<QuarkGroup> quarkGroups = new List<QuarkGroup>(); //fifteen max
    Vector3[] quarkPositions;
    int quarkGroupMax;

    public override int GetNetCharge()
    {
        int netCharge = 0;
        for(int i=0; i < quarkGroups.Count; i++)
        {
            //Debug.Log(string.Format("Quark Group Charge [{0}] - {1}", i, quarkGroups[i].GetNetCharge()));
            //Debug.Log(string.Format("Quark Group Size [{0}] - {1}", i, quarkGroups[i].quarks.Count));
            //Debug.Log(string.Format("Quark Group - {0}", quarkGroups[i].ToString()));
            if(quarkGroups[i].isFull)
            {
                netCharge += quarkGroups[i].GetNetCharge();
            }
        }
        //Debug.Log("Quark Net Charge");
        //Debug.Log(netCharge);
        return netCharge;
    }

    ElectronManagerBehavior leptonManager
    {
        get { return block.electronManager; }
    }

    // Start is called before the first frame update
    public void Start()
    {
        if(quarkPositions == null)
        {
            SetUpQuarkPositions();
        }
    }

    void Update()
    {
        //TODO implement stack
        //TODO not all the time
        PlaceQuarkGroups();
    }

    public override bool DeathCheck() //Only call on subtract operations
    {
        if(particleDictionary[2].Count == 0)
        {
            return true;
        }
        return false;
    }

    void SetUpQuarkPositions()
    {
        //TODO Implement GameMaster Dictionary cache
        //Cache on quark and leptonsMax, if leptonsMax in cache load those values
        //Possibly save cache as json, and game master can load that on start.
        //Garbage cleanups on non-used cache entries
        //Setting up quarkGroupMax from leptonMax, ie bottlenecked on max neg charge
        quarkGroupMax = 1;
        int quarkGroupFibonnacciI = 2;
        while (quarkGroupMax < leptonManager.electronsMax)
        {
            quarkGroupMax += quarkGroupFibonnacciI++;
        }

        quarkPositions = new Vector3[quarkGroupMax*QuarkGroup.quarkGroupCapacity];
        Vector3 initPos = new Vector3(x: 0, y: 0, z: 0);
        int i = 0;
        int count = 0;
        float scalingFactor = transform.localScale.x / 10; // scale should be same for x,y,z
        while (true)
        {
            float dimY = initPos.y - (i * scalingFactor * 0.5f);
            float initZ = initPos.z + (i * scalingFactor * 0.25f);
            for (int ii = 0; ii <= i; ii++)
            {
                float initX = initPos.x + (ii * scalingFactor * 0.25f);
                for (int iii = 0; iii <= ii; iii++)
                {
                    if (count >= quarkPositions.Length)
                    {
                        for (int placeI = 0; placeI < quarkPositions.Length; placeI++)
                        {
                            float newY = quarkPositions[placeI].y + ((i + 1) * scalingFactor * 0.5f) / 2;
                            quarkPositions[placeI] = new Vector3(quarkPositions[placeI].x, newY, quarkPositions[placeI].z);
                        }
                        return;
                    }

                    float dimX = initX - (iii * scalingFactor * 0.5f);
                    float dimZ = initZ - (ii * scalingFactor * 0.5f);
                    Vector3 pos = new Vector3(x: dimX, y: dimY, z: dimZ);
                    quarkPositions[count++] = pos;
                }
            }
            i++;
        }
    }
    void PlaceQuarkGroups()
    {
        int quarkPositionI = 0;
        for (int i = 0; i < quarkGroups.Count; i++)
        {
            foreach (ParticleBehavior quark in quarkGroups[i].quarks)
            {
                quark.transform.localPosition = Vector3.Lerp(quark.transform.localPosition, quarkPositions[quarkPositionI], block.particleAnimationSpeed / 2 * Time.deltaTime);
                quarkPositionI++; // So that the particles will be displaced from each other, if they are not in the same groups
            }
        }
    }
    public void SetQuark(QuarkBehavior quark)  //Race Condition
    {
        for (int quarkGroupI = quarkGroups.Count - 1; quarkGroupI >= 0; quarkGroupI--)
        {
            QuarkGroup quarkGroup = quarkGroups[quarkGroupI];
            if(quarkGroup.Validate(quark))
            {
                quarkGroup.AddParticle(quark);
                return;
            }
        }

        // TODO figure out if we want particles to fill up sequentually or in parallel
        if (quarkGroups.Count < quarkGroupMax)
        {
            quarkGroups.Add(new QuarkGroup(quark, this));
            return;
        }
        else
        {
            quark.Free();
            return;
        }
    }

    public HashSet<FermionBehavior> ExtractExcessQuarks() // -1, -2, +1
    {
        //If netCharge != all quarks plus each other rescatter.
        bool optimalProtonState = particleDictionary[2].Count >= (particleDictionary[-1].Count * 2);

        if (!optimalProtonState)
        {
            quarkGroups = new List<QuarkGroup>(); //reset Quark Groups

            HashSet<FermionBehavior> negativeQuarks = particleDictionary[-1];
            HashSet<FermionBehavior> positiveQuarks = particleDictionary[2];

            particleDictionary[-1] = new HashSet<FermionBehavior>();
            particleDictionary[2] = new HashSet<FermionBehavior>();

            int numProtons = particleDictionary[2].Count / 2;

            //Creates as many protons
            for (int protonCounter = 0; protonCounter < numProtons; protonCounter++)
            {
                FermionBehavior negativeQuark = GetFirstParticle(negativeQuarks);
                negativeQuarks.Remove(negativeQuark);
                ReAddParticle(negativeQuark);

                QuarkGroup proton = new QuarkGroup((QuarkBehavior)negativeQuark, this);
                for (int posCounter = 1; posCounter <= 2; posCounter++)
                {
                    FermionBehavior positiveQuark = GetFirstParticle(positiveQuarks);
                    positiveQuarks.Remove(positiveQuark);
                    proton.AddParticle((QuarkBehavior)positiveQuark);
                }
                quarkGroups.Add(proton);
            }

            //Adds any remaining up particles, as there could be  remainder
            foreach (ParticleBehavior positiveQuark in positiveQuarks)
            {
                SetQuark((QuarkBehavior)positiveQuark);
            }

            //Frees all remaining negative particles
            foreach (ParticleBehavior negativeQuark in negativeQuarks)
            {
                QuarkBehavior negativeQuarkBehavior = (QuarkBehavior)negativeQuark;
                negativeQuarkBehavior.Free();
            }

            return negativeQuarks;
        }
        return null;
    }


}
