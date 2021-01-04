using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockLeptonManagerBehavior : BlockManagerBehavior
{
    //public class LeptonPosition
    //{
    //    public GameObject lepton;
    //    public Vector3 position;
    //    public int[] neighborIdx;
    //    public int id;
    //}
    //public int particleAnimationSpeed = 5;
    //List<GameObject> leptons = new List<GameObject>();
    //LeptonPosition[] allLeptonPositions;
    //List<int> openLeptonPositions;
    //int leptonsMax = 8;  //TODO programmatically figure out max number of leptons, which we can figure out from number of vertexes in shape
    //// Start is called before the first frame update
    //void Start()
    //{
    //    SetUpLeptonPositions();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    PlaceLeptons();
    //}

    //void SetUpLeptonPositions()
    //{
    //    allLeptonPositions = new LeptonPosition[leptonsMax];
    //    openLeptonPositions = new List<int>();
    //    float[] distances = { -0.5f * scalingFactor, 0.5f * scalingFactor };

    //    for (int i = 0; i < 3; i++)
    //    {
    //        float neededSpace = blockDimension[i] * displacementFactor;
    //        numBlocks[i] = (int)(dimensions[i] / neededSpace);
    //    }

    //    int leptonI = 0;
    //    for (int i = 0; i < 2; i++)
    //    {
    //        for (int ii = 0; ii < 2; ii++)
    //        {
    //            for (int iii = 0; iii < 2; iii++)
    //            {
    //                LeptonPosition leptonPosition = new LeptonPosition
    //                {
    //                    position = new Vector3(distances[i], distances[ii], distances[iii]),
    //                    neighborIdx = new int[] { Vector3Utils.Vector3ToIdx(1 - i, ii, iii), Vector3Utils.Vector3ToIdx(i, 1 - ii, iii), Vector3Utils.Vector3ToIdx(i, ii, 1 - iii) },
    //                    id = leptonI
    //                };
    //                allLeptonPositions[leptonI] = leptonPosition;
    //                openLeptonPositions.Add(leptonI);
    //                leptonI++;
    //            }
    //        }
    //    }
    //}

    //bool NetLeptonChargeValid(GameObject particle = null)
    //{
    //    bool check;
    //    if (particle is null)
    //    {
    //        check = (netChargeCache >= 0) && (leptons.Count <= leptonsMax - 1);
    //    }
    //    else
    //    {
    //        ParticleBehavior particleBehavior = particle.GetComponent<ParticleBehavior>();
    //        check = particleBehavior.isLepton && (particleBehavior.effectiveCharge + netChargeCache >= 0) && (leptons.Count <= leptonsMax - 1);
    //    }
    //    return check;
    //}

    //public int GetNetLeptonCharge() //TODO get outside lepton charge too
    //{
    //    int leptonNetCharge = 0;
    //    for (int i = 0; i < leptons.Count; i++)
    //    {
    //        ParticleBehavior particleBehavior = leptons[i].GetComponent<ParticleBehavior>();
    //        leptonNetCharge += particleBehavior.effectiveCharge;
    //    }

    //    //foreach(BlockConnection blockConnection in connectedBlocks.Values)
    //    //{
    //    //    if(blockConnection.hasOtherBlock)
    //    //    {
    //    //        leptonNetCharge += blockConnection.netOtherCharge;
    //    //    }
    //    //}

    //    return leptonNetCharge;
    //}

    //void LeptonTransplace(LeptonPosition leptonPosition)
    //{
    //    ParticleBehavior lepton = leptonPosition.lepton.GetComponent<ParticleBehavior>();
    //    if (V3Equal(leptonPosition.lepton.transform.localPosition, leptonPosition.position))
    //    {
    //        for (int i = 0; i < 3; i++)
    //        {
    //            int neighborId = leptonPosition.neighborIdx[i];
    //            LeptonPosition newLeptonPosition = allLeptonPositions[neighborId];
    //            if (newLeptonPosition.lepton is null)
    //            {
    //                lepton.leptonPosition = newLeptonPosition;
    //                newLeptonPosition.lepton = lepton.gameObject;
    //                leptonPosition.lepton = null;
    //                openLeptonPositions.Add(leptonPosition.id);
    //                openLeptonPositions.Remove(neighborId);
    //                return;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        leptonPosition.lepton.transform.localPosition = Vector3.Lerp(leptonPosition.lepton.transform.localPosition, leptonPosition.position, particleAnimationSpeed * Time.deltaTime);
    //    }
    //}

    //void PlaceLeptons()
    //{
    //    RefreshLeptons();
    //    // TODO lock leptons if they are used to connect blocks
    //    foreach (GameObject lepton in leptons)
    //    {
    //        ParticleBehavior leptonBehavior = lepton.GetComponent<ParticleBehavior>();
    //        if (leptonBehavior.leptonPosition is null)
    //        {
    //            LeptonPosition openLeptonPosition = allLeptonPositions[openLeptonPositions[0]];
    //            leptonBehavior.leptonPosition = openLeptonPosition;
    //            openLeptonPosition.lepton = lepton;
    //            openLeptonPositions.Remove(0);
    //        }
    //        else
    //        {
    //            if (leptonBehavior.leptonPosition.lepton is null)
    //            {
    //                leptonBehavior.leptonPosition.lepton = lepton; //TODO find out how this condition occurs
    //            }
    //            LeptonTransplace(leptonBehavior.leptonPosition);
    //        }
    //    }
    //}

    //bool LastLeptonPairing()
    //{
    //    return GetAvailableLeptons().Count + GetAvailableLeptonSpace() <= 4;
    //}

    //int GetAvailableLeptonSpace()
    //{
    //    int availableSpace = leptonsMax - leptons.Count;
    //    //foreach(BlockConnection blockConnection in connectedBlocks.Values)
    //    //{
    //    //    availableSpace -= blockConnection.totalLeptonCount;
    //    //}
    //    return System.Math.Max(availableSpace, 0);
    //}

    //List<GameObject> GetAvailableLeptons()
    //{
    //    List<GameObject> availableLeptons = new List<GameObject>();
    //    foreach (GameObject lepton in leptons)
    //    {
    //        if (lepton.GetComponent<ParticleBehavior>().available)
    //        {
    //            availableLeptons.Add(lepton);
    //        }
    //    }
    //    return availableLeptons;
    //}

    //void RemoveLepton(int particleIndex)
    //{
    //    GameObject particle = leptons[particleIndex];
    //    leptons.RemoveAt(particleIndex);
    //    ParticleBehavior particleBehavior = particle.GetComponent<ParticleBehavior>();
    //    netChargeCache -= particleBehavior.effectiveCharge;
    //    particleBehavior.Free();
    //}

    //public void AddLepton(GameObject particle)
    //{
    //    if (NetLeptonChargeValid(particle))
    //    {
    //        ParticleBehavior particleBehavior = particle.GetComponent<ParticleBehavior>();
    //        if (!particleBehavior.isLepton)
    //        {
    //            Debug.LogError("Particle needs to be lepton");
    //        }
    //        leptons.Add(particle);
    //        netChargeCache += particleBehavior.effectiveCharge;
    //        particleBehavior.Occupy(gameObject);
    //    }
    //}

    //protected void RefreshLeptons()
    //{
    //    leptons.RemoveAll(lepton => lepton == null);
    //    for (int i = 0; i < leptons.Count; i++)
    //    {
    //        leptons[i].GetComponent<ParticleBehavior>().available = true;
    //    }

    //    int leptonI = 0;
    //    while (leptonI < leptons.Count - 1) //NO NEUTRINOS
    //    {
    //        bool neutrinoCheck = leptons[leptonI].GetComponent<ParticleBehavior>().effectiveCharge == 0;
    //        if (neutrinoCheck)
    //        {
    //            RemoveLepton(leptonI);
    //        }
    //        else
    //        {
    //            leptonI += 1;
    //        }
    //    }

    //    leptonI = 0;
    //    while (!NetLeptonChargeValid() && (leptons.Count > 0) && (leptonI < leptons.Count - 1))
    //    {
    //        if (leptons[leptonI].GetComponent<ParticleBehavior>().effectiveCharge < 0)
    //        {
    //            RemoveLepton(leptonI);
    //        }
    //        else
    //        {
    //            leptonI += 1;
    //        }
    //    }
    //}

}
