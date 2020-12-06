using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehavior : MatterBehavior
{
    public GameObject particleRef;
    List<GameObject> quarks = new List<GameObject>();
    List<GameObject> leptons = new List<GameObject>();
    LeptonPosition[] allLeptonPositions;
    List<int> openLeptonPositions;

    Dictionary<int, GameObject> connectedBlockDictionary; // max number of connected blocks is 6

    public class LeptonPosition
    {
        public GameObject lepton;
        public Vector3 position;
        public int[] neighborIdx;
        public int id;
    }


    void Start()
    {
        SetUpLeptonPositions();
        DisableCollision();
        BeginnerElement(); //TODO replace with RandomElement
    }

    void BeginnerElement()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject particle = Instantiate(particleRef);
            ParticleBehavior particleBehavior = getParticleData(particle);
            if (i <= 1)
            {
                particleBehavior.particleStateType = "quarkPos";
            }
            else if (i <= 2)
            {
                particleBehavior.particleStateType = "quarkNeg";
            }
            else 
            {
                particleBehavior.particleStateType = "leptonNeutral";
            }
            collideParticle(particle);
        }
    }
    void RandomElement()
    {
        int numberOfParticles = Random.Range(1,10);
        for (int i = 0; i < numberOfParticles; i++)
        {
            GameObject particle = Instantiate(particleRef);
            ParticleBehavior particleBehavior = getParticleData(particle);
            particleBehavior.RandomState();
            collideParticle(particle);
        }
    }


    protected override void VisualUpdate()
    {
        List<Vector3> quarkPositions = GetQuarkPlacements(quarks.Count);
        for (int i = 0; i < quarkPositions.Count; i++)
        {
            quarks[i].transform.localPosition = Vector3.Lerp(quarks[i].transform.localPosition, quarkPositions[i], 0.05f);
        }
        PlaceLeptons();
    }
    protected override void RefreshState()
    {
        quarks.RemoveAll(item => item == null);
        leptons.RemoveAll(item => item == null);
        int particleI = 0;
        while (!NetQuarkChargeValid() && (quarks.Count > 0) && (particleI < (quarks.Count-1)))
        {
            if(GetParticleGroupNetCharge(quarks) < 0 && getParticleData(quarks[particleI]).charge < 0)
            {
                removeParticleFrom(quarks, particleI);
            }
            else if (GetParticleGroupNetCharge(quarks) > 0 && getParticleData(quarks[particleI]).charge > 0)
            {
                removeParticleFrom(quarks, particleI);
            }
            else
            {
                particleI += 1;
            }
        }

        particleI = 0;
        while (!NetLeptonChargeValid() && (GetNetCharge() <= -1) && (leptons.Count > 0) && (particleI < leptons.Count-1))
        {
            if (getParticleData(leptons[particleI]).charge < 0)
            {
                removeParticleFrom(leptons, particleI);
            }
            else
            {
                particleI += 1;
            }
        }

    }
    protected override void DeathCheck()
    {
        if (leptons.Count == 0 && quarks.Count == 0)
        {
            Destroy(gameObject);
        } // TODO if the equilibrium is not correct, transform.DetachChildren();
    }
    // max number of particles is 10, acceptable indices are from 0-1, and a factor 3, min -2
    public void collideParticle(GameObject particle)
    {
        ParticleBehavior particleBehavior = getParticleData(particle);
        if(particleBehavior.isFermion)
        {
            if(NetQuarkChargeValid(particleBehavior))
            {
                addParticleTo(quarks, particle);
            }
            else if(NetLeptonChargeValid(particleBehavior))
            {
                addParticleTo(leptons, particle);
            }
        }
    }
    void addParticleTo(List<GameObject> particleGroup, GameObject particle)
    {
        particleGroup.Add(particle);
        ParticleBehavior particleBehavior = getParticleData(particle);
        particleBehavior.Occupy(gameObject);
    }

    void removeParticleFrom(List<GameObject> particleGroup, int particleIndex)
    {
        GameObject particle = particleGroup[particleIndex];
        particleGroup.RemoveAt(particleIndex);
        ParticleBehavior particleBehavior = getParticleData(particle);
        particleBehavior.Free();

    }
    void collideBlock(GameObject block)
    {
    }

    void OnCollisionEnter(Collision col){
        if (col.gameObject.CompareTag("Particle"))
        {
            DisableCollision();
            collideParticle(col.gameObject);
        }
        if (col.gameObject.CompareTag("Block"))
        {
            collideBlock(col.gameObject);
        }
    }
    // Visual Update Utils
    List<Vector3> GetQuarkPlacements(int particleCount)
    {
        List<Vector3> placements = new List<Vector3>();
        Vector3 initPos = new Vector3(x: 0, y: 0, z: 0);
        int i = 0;
        int count = 0;
        float scalingFactor = transform.localScale.x/10; // scale should be same for x,y,z
        while(true)
        {
            float dimY = initPos.y-(i*scalingFactor*0.5f);
            float initZ = initPos.z+(i*scalingFactor*0.25f);
            for(int ii = 0; ii <= i; ii++)
            {
                float initX = initPos.x+(ii*scalingFactor*0.25f);
                for(int iii = 0; iii <= ii; iii++)
                {
                    count++;
                    if(count > particleCount)
                    {
                        for(int placeI=0; placeI<placements.Count; placeI++)
                        {
                            float newY = placements[placeI].y + ((i+1)*scalingFactor*0.5f)/2;
                            placements[placeI] = new Vector3(placements[placeI].x, newY, placements[placeI].z);
                        }
                        return placements;
                    }

                    float dimX = initX - (iii*scalingFactor*0.5f);
                    float dimZ = initZ - (ii*scalingFactor*0.5f);
                    Vector3 pos = new Vector3(x: dimX, y: dimY, z: dimZ);
                    placements.Add(pos);
                }
            }
            i++;
        }
    }
    void PlaceLeptons()
    {
        foreach(GameObject lepton in leptons)
        {
            ParticleBehavior leptonBehavior = getParticleData(lepton);
            if(leptonBehavior.leptonPosition is null)
            {
                LeptonPosition openLeptonPosition = allLeptonPositions[openLeptonPositions[0]];
                leptonBehavior.leptonPosition = openLeptonPosition;
                openLeptonPosition.lepton = lepton;
                openLeptonPositions.Remove(0);
            }
            else
            {
                LeptonTransplace(leptonBehavior.leptonPosition);
            }

        }
    }

    void LeptonTransplace(LeptonPosition leptonPosition)
    {
        ParticleBehavior lepton = getParticleData(leptonPosition.lepton);
        if (V3Equal(leptonPosition.lepton.transform.localPosition, leptonPosition.position))
        {
            for (int i = 0; i < 3; i++)
            {
                int neighborId = leptonPosition.neighborIdx[i];
                LeptonPosition newLeptonPosition = allLeptonPositions[neighborId];
                if (newLeptonPosition.lepton is null)
                {
                    lepton.leptonPosition = newLeptonPosition;
                    newLeptonPosition.lepton = lepton.gameObject;
                    leptonPosition.lepton = null;
                    openLeptonPositions.Add(leptonPosition.id);
                    openLeptonPositions.Remove(neighborId);
                    return;
                }
            }
        }
        else
        {
            leptonPosition.lepton.transform.localPosition = Vector3.Lerp(leptonPosition.lepton.transform.localPosition, leptonPosition.position, 0.05f);
        }
    }

    void SetUpLeptonPositions()
    {
        allLeptonPositions = new LeptonPosition[8];
        openLeptonPositions = new List<int>();
        float[] distances = { -0.5f * scalingFactor, 0.5f * scalingFactor };
        int leptonI = 0;
        for (int i = 0; i < 2; i++)
        {
            for (int ii = 0; ii < 2; ii++)
            {
                for (int iii = 0; iii < 2; iii++)
                {
                    LeptonPosition leptonPosition = new LeptonPosition
                    {
                        position = new Vector3(distances[i], distances[ii], distances[iii]),
                        neighborIdx = new int[] { Vector3ToIdx(1 - i, ii, iii), Vector3ToIdx(i, 1 - ii, iii), Vector3ToIdx(i, ii, 1 - iii) },
                        id = leptonI
                    };
                    allLeptonPositions[leptonI] = leptonPosition;
                    openLeptonPositions.Add(leptonI);
                    leptonI++;
                }
            }
        }
    }

    int Vector3ToIdx(int i, int ii, int iii)
    {
        string binaryRep = string.Format("{0}{1}{2}", iii, ii, i);
        return System.Convert.ToInt32(binaryRep, 2);
    }


    // Refresh State Utils
    public int GetParticleGroupNetCharge(List<GameObject> particles)
    {
        int newCharge = 0;
        foreach (var quark in particles)
        {
            ParticleBehavior particleBehavior = getParticleData(quark);
            newCharge += particleBehavior.charge;
        }
        return newCharge;
    }
    public int GetNetCharge()
    {
        return GetParticleGroupNetCharge(leptons) + GetParticleGroupNetCharge(quarks);
    }

    bool NetQuarkChargeValid(ParticleBehavior particleBehavior=null)
    {
        int totalQuarkCharge = GetParticleGroupNetCharge(quarks);
        bool check;
        if(particleBehavior is null)
        {
            check = (totalQuarkCharge >= -2) && (totalQuarkCharge <= 4) && (quarks.Count <= 9);
        }
        else
        {
            check = particleBehavior.isQuark && (totalQuarkCharge + particleBehavior.charge >= -2) && (totalQuarkCharge + particleBehavior.charge <= 4) && (quarks.Count <= 9);
        }
        return check;
    }

    bool NetLeptonChargeValid(ParticleBehavior particleBehavior=null)
    {
        bool check;
        if (particleBehavior is null)
        {
            check = (GetNetCharge() == 0) && (leptons.Count <= 7);
        }
        else
        {
            check = particleBehavior.isLepton && (particleBehavior.charge + GetNetCharge() >= 0) && (leptons.Count <= 7);
        }
        return check;
    }


    ParticleBehavior getParticleData(GameObject particle)
    {
        return particle.GetComponent<ParticleBehavior>();
    }

    Rigidbody getRigidBody(GameObject particle)
    {
        return particle.GetComponent<Rigidbody>();
    }
}
