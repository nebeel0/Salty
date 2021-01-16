using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameMaster gameMaster;

    public GameObject cageRef;
    public GameObject playerRef;
    public GameObject clusterRef;
    public GameObject blockRef;
    public GameObject baseParticleRef;
    public Material particleLit;

    public Vector3 bounds;

    public HashSet<ClusterBehavior> SystemClusters = new HashSet<ClusterBehavior>();

    //Spawn Utils
    void SpawnParticles(int seed, Vector3 bounds)
    {
        for (int i = 0; i < seed; i++)
        {
            BlockBehavior block = CreateBlock();
            block.BeginnerElementFlag = true;
            block.transform.position = Vector3Utils.RandomBoundedVector3(bounds);
            block.transform.eulerAngles = Vector3Utils.RandomEulerAngles();
        }

        for (int i = 0; i < seed; i++)
        {
            ElectronBehavior electron = CreateElectron(1, 100, 1);
            electron.transform.position = Vector3Utils.RandomBoundedVector3(bounds);
        }
    }

    public CageBehavior CreateCage()
    //Created when collions occur between same types. Quarks, Leptons.
    //Created when mass is too high
    {
        GameObject cageObject = Instantiate(cageRef);
        CageBehavior cageBehavior = cageObject.GetComponent<CageBehavior>();
        cageBehavior.gameMaster = gameMaster;
        cageBehavior.dimensions = bounds;
        return cageBehavior;
    }

    public BlockBehavior CreateBlock()
    //Created when collions occur between same types. Quarks, Leptons.
    //Created when mass is too high
    {
        GameObject block = Instantiate(blockRef);
        BlockBehavior blockBehavior = block.GetComponent<BlockBehavior>();
        blockBehavior.gameMaster = gameMaster;
        blockBehavior.Start();
        return blockBehavior;
    }

    public ClusterBehavior CreateCluster(HashSet<BlockBehavior> blocks)
    //Created when collions occur between same types. Quarks, Leptons.
    //Created when mass is too high
    {
        GameObject cluster = Instantiate(clusterRef);
        ClusterBehavior clusterBehavior = cluster.GetComponent<ClusterBehavior>();
        clusterBehavior.gameMaster = gameMaster;
        clusterBehavior.blocks = blocks;
        cluster.GetComponent<ClusterMessageBehavior>().Start();
        clusterBehavior.BFSRefresh();
        SystemClusters.Add(clusterBehavior);
        return clusterBehavior;
    }

    public NeutrinoBehavior CreateNeutrino(float energy, int weightClass)
    //Neutrinos can be created by wBosons decaying
    {
        GameObject neutrino = Instantiate(baseParticleRef);
        NeutrinoBehavior neutrinoBehavior = neutrino.AddComponent<NeutrinoBehavior>();
        neutrinoBehavior.energy = energy;
        neutrinoBehavior.weightClass = weightClass;
        neutrinoBehavior.gameMaster = gameMaster;
        neutrinoBehavior.Start();
        return neutrinoBehavior;
    }

    public ElectronBehavior CreateElectron(int weightClass, float energy, int antiCharge)
    //Leptons can be created by wBosons decaying
    {
        GameObject electron = Instantiate(baseParticleRef);
        ElectronBehavior electronBehavior = electron.AddComponent<ElectronBehavior>();
        electronBehavior.energy = energy;
        electronBehavior.weightClass = weightClass;
        electronBehavior.antiCharge = antiCharge;
        electronBehavior.gameMaster = gameMaster;
        electronBehavior.Start();
        return electronBehavior;
    }
    //Quarks can be created by wBosons decaying
    public UpBehavior CreateQuarkPos(int weightClass, float energy, int antiCharge)
    {
        GameObject posQuark = Instantiate(baseParticleRef);
        UpBehavior posQuarkBehavior = posQuark.AddComponent<UpBehavior>();
        posQuarkBehavior.energy = energy;
        posQuarkBehavior.weightClass = weightClass;
        posQuarkBehavior.antiCharge = antiCharge;
        posQuarkBehavior.gameMaster = gameMaster;
        posQuarkBehavior.Start();
        return posQuarkBehavior;
    }
    public DownBehavior CreateQuarkNeg(int weightClass, float energy, int antiCharge)
    {

        GameObject negQuark = Instantiate(baseParticleRef);
        DownBehavior negQuarkBehavior = negQuark.AddComponent<DownBehavior>();
        negQuarkBehavior.energy = energy;
        negQuarkBehavior.weightClass = weightClass;
        negQuarkBehavior.antiCharge = antiCharge;
        negQuarkBehavior.gameMaster = gameMaster;
        negQuarkBehavior.Start();
        return negQuarkBehavior;
    }
    public PhotonBehavior CreatePhoton(float energy, Vector3 direction)
    //Created by annihilation
    //Created by decay processes 
    //Created by collisions
    //All processes can cause photon creation, as it takes extra mass and converts to energy.
    {
        GameObject photon = Instantiate(baseParticleRef);
        PhotonBehavior photonBehavior = photon.AddComponent<PhotonBehavior>();
        photonBehavior.energy = energy;
        photonBehavior.transform.forward = direction;
        photonBehavior.gameMaster = gameMaster;
        photonBehavior.Start();
        return photonBehavior;
    }
    public ZBosonBehavior CreateZBoson(int weightClass, float energy)
    //Created when collions occur between neutrinos
    {
        GameObject zBoson = Instantiate(baseParticleRef);
        ZBosonBehavior zBosonBehavior = zBoson.AddComponent<ZBosonBehavior>();
        zBosonBehavior.energy = energy;
        zBosonBehavior.weightClass = weightClass;
        zBosonBehavior.gameMaster = gameMaster;
        zBosonBehavior.Start();
        return zBosonBehavior;
    }
    public WBosonBehavior CreateWBoson(int weightClass, float energy, int antiCharge)
    //Created when collions occur between same types. Quarks, Leptons.
    //Created when mass is too high
    {
        GameObject wBoson = Instantiate(baseParticleRef);
        WBosonBehavior wBosonBehavior = wBoson.AddComponent<WBosonBehavior>();
        wBosonBehavior.energy = energy;
        wBosonBehavior.weightClass = weightClass;
        wBosonBehavior.antiCharge = antiCharge;
        wBosonBehavior.gameMaster = gameMaster;
        wBosonBehavior.Start();
        return wBosonBehavior;
    }
}
