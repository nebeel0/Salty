using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class BlockQuarkManagerBehavior : BlockManagerBehavior
{
    //public class QuarkGroup
    //{
    //    public static int maxLength = 3;
    //    public GameObject[] quarks = new GameObject[maxLength];
    //    public int netCharge
    //    {
    //        get
    //        {
    //            int netCharge = 0;
    //            for (int i = 0; i < maxLength; i++)
    //            {
    //                if (quarks[i] != null)
    //                {
    //                    netCharge += quarks[i].GetComponent<ParticleBehavior>().effectiveCharge;
    //                }
    //                else
    //                {
    //                    return 0; // zero if not full
    //                }
    //            }
    //            return netCharge;
    //        }
    //    }
    //    public QuarkGroup(GameObject quark)
    //    {
    //        quarks[0] = quark;
    //    }

    //    public bool Valid()
    //    {
    //        bool filledUp = true;
    //        int productCharge = 1;
    //        int totalCharge = 0;
    //        for (int i = 0; i < maxLength; i++)
    //        {
    //            GameObject quark = quarks[i];
    //            if (quark != null)
    //            {
    //                totalCharge += quark.GetComponent<ParticleBehavior>().effectiveCharge;
    //                productCharge *= quark.GetComponent<ParticleBehavior>().effectiveCharge;
    //            }
    //            else
    //            {
    //                filledUp = false;
    //            }
    //        }
    //        if (filledUp)
    //        {
    //            bool maxTwoSame = productCharge < 0;
    //            return (totalCharge == 3 || totalCharge == 0) && maxTwoSame;
    //        }
    //        return true;
    //    }
    //}
    //List<QuarkGroup> quarkGroups = new List<QuarkGroup>(); //fifteen max
    //public int numQuarkGroups
    //{
    //    get { return quarkGroups.Count; }
    //}
    //Vector3[] quarkPositions;
    //int quarkGroupMax;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    SetUpQuarkPositions();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    PlaceQuarkGroups();
    //}

    //void SetUpQuarkPositions()
    //{
    //    //Setting up quarkGroupMax from leptonMax, ie bottlenecked on max neg charge
    //    quarkGroupMax = 1;
    //    int quarkGroupFibonnacciI = 2;
    //    while (quarkGroupMax < leptonsMax)
    //    {
    //        quarkGroupMax += quarkGroupFibonnacciI++;
    //    }

    //    quarkPositions = new Vector3[quarkGroupMax*QuarkGroup.maxLength];
    //    Vector3 initPos = new Vector3(x: 0, y: 0, z: 0);
    //    int i = 0;
    //    int count = 0;
    //    float scalingFactor = transform.localScale.x / 10; // scale should be same for x,y,z
    //    while (true)
    //    {
    //        float dimY = initPos.y - (i * scalingFactor * 0.5f);
    //        float initZ = initPos.z + (i * scalingFactor * 0.25f);
    //        for (int ii = 0; ii <= i; ii++)
    //        {
    //            float initX = initPos.x + (ii * scalingFactor * 0.25f);
    //            for (int iii = 0; iii <= ii; iii++)
    //            {
    //                if (count >= quarkPositions.Length)
    //                {
    //                    for (int placeI = 0; placeI < quarkPositions.Length; placeI++)
    //                    {
    //                        float newY = quarkPositions[placeI].y + ((i + 1) * scalingFactor * 0.5f) / 2;
    //                        quarkPositions[placeI] = new Vector3(quarkPositions[placeI].x, newY, quarkPositions[placeI].z);
    //                    }
    //                    return;
    //                }

    //                float dimX = initX - (iii * scalingFactor * 0.5f);
    //                float dimZ = initZ - (ii * scalingFactor * 0.5f);
    //                Vector3 pos = new Vector3(x: dimX, y: dimY, z: dimZ);
    //                quarkPositions[count++] = pos;
    //            }
    //        }
    //        i++;
    //    }
    //}

    //public int GetNetQuarkCharge()
    //{
    //    int quarkNetCharge = 0;
    //    foreach (QuarkGroup quarkGroup in quarkGroups)
    //    {
    //        quarkNetCharge += quarkGroup.netCharge;
    //    }
    //    return quarkNetCharge;
    //}

    //void PlaceQuarkGroups()
    //{
    //    RefreshQuarkGroups();
    //    int quarkPositionI = 0;
    //    for (int i = 0; i < quarkGroups.Count; i++)
    //    {
    //        for (int ii = 0; ii < QuarkGroup.maxLength; ii++)
    //        {
    //            GameObject quark = quarkGroups[i].quarks[ii];
    //            if (quark != null)
    //            {
    //                quark.transform.localPosition = Vector3.Lerp(quark.transform.localPosition, quarkPositions[quarkPositionI], particleAnimationSpeed / 2 * Time.deltaTime);
    //                quarkPositionI++; // So that the particles will be displaced from each other, if they are not in the same groups
    //            }
    //        }
    //    }
    //}

    //public bool AddQuark(GameObject quark)  //Race Condition
    //{
    //    ParticleBehavior quarkBehavior = quark.GetComponent<ParticleBehavior>();
    //    if (!quarkBehavior.isQuark)
    //    {
    //        return false;
    //    }
    //    // TODO figure out if we want particles to fill up sequentually or in parallel
    //    for (int quarkGroupI = quarkGroups.Count - 1; quarkGroupI >= 0; quarkGroupI--)
    //    {
    //        QuarkGroup quarkGroup = quarkGroups[quarkGroupI];
    //        for (int i = 0; i < quarkGroup.quarks.Length; i++)
    //        {
    //            if (quarkGroup.quarks[i] == null)
    //            {
    //                quarkGroup.quarks[i] = quark;
    //                if (quarkGroup.Valid())
    //                {
    //                    netChargeCache += quarkBehavior.effectiveCharge;
    //                    quark.GetComponent<ParticleBehavior>().Occupy(gameObject);
    //                    return true;
    //                }
    //                else
    //                {
    //                    quarkGroup.quarks[i] = null;
    //                    break;
    //                }
    //            }
    //        }
    //    }

    //    if (quarkGroups.Count < quarkGroupMax)
    //    {
    //        netChargeCache += quarkBehavior.effectiveCharge;
    //        quarkGroups.Add(new QuarkGroup(quark));
    //        quark.GetComponent<ParticleBehavior>().Occupy(gameObject);
    //        return true;
    //    }

    //    return false;
    //}

    //void RefreshQuarkGroups() // TODO quarks cooldown, give players a chance to recollect particles, and computers a rendering break
    //{
    //    //If netCharge != all quarks plus each other rescatter.
    //    if (netChargeCache < 0) //Imbalanced, more electrons than protons
    //    {
    //        Dictionary<int, List<GameObject>> quarkDictionary = new Dictionary<int, List<GameObject>>();
    //        foreach (QuarkGroup quarkGroup in quarkGroups)
    //        {
    //            for (int i = 0; i < QuarkGroup.maxLength; i++)
    //            {
    //                GameObject quark = quarkGroup.quarks[i];
    //                if (quark != null)
    //                {
    //                    int quarkCharge = quark.GetComponent<ParticleBehavior>().effectiveCharge;
    //                    if (!quarkDictionary.ContainsKey(quarkCharge))
    //                    {
    //                        quarkDictionary[quarkCharge] = new List<GameObject>();
    //                    }
    //                    quarkDictionary[quarkCharge].Add(quark);
    //                }
    //            }
    //        }

    //        //Make as many protons as possible, till net Charge is back to zero
    //        quarkGroups = new List<QuarkGroup>(); //reset Quark Groups
    //        int netLeptonCharge = GetNetLeptonCharge();
    //        while (netLeptonCharge + GetNetQuarkCharge() < 0)
    //        {
    //            if (quarkDictionary.ContainsKey(2) && quarkDictionary.ContainsKey(-1) && quarkDictionary[2].Count >= 2 && quarkDictionary[-1].Count >= 1)
    //            {
    //                QuarkGroup proton = new QuarkGroup(quarkDictionary[-1][0]);
    //                quarkDictionary[-1].RemoveAt(0);
    //                for (int i = 1; i <= 2; i++)
    //                {
    //                    GameObject upParticle = quarkDictionary[2][0];
    //                    proton.quarks[i] = upParticle;
    //                    quarkDictionary[2].RemoveAt(0);
    //                }
    //                quarkGroups.Add(proton);
    //            }
    //            else
    //            {
    //                break;
    //            }
    //        }

    //        foreach (List<GameObject> quarkList in quarkDictionary.Values)
    //        {
    //            // For rest of particles just add it back normally
    //            foreach (GameObject quark in quarkList)
    //            {
    //                if (!AddQuark(quark))
    //                {
    //                    netChargeCache -= quark.GetComponent<ParticleBehavior>().effectiveCharge;
    //                    quark.GetComponent<ParticleBehavior>().Free();
    //                }
    //            }
    //        }
    //    }
    //    netChargeCache = GetNetLeptonCharge() + GetNetQuarkCharge();
    //}


}
