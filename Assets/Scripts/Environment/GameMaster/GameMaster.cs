using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.InputSystem;


public class GameMaster : MonoBehaviour
{
    //Example You must survive till the end of the waves
    //Example You must find the wrong block
    //Example be the last block standing on the platform
    //Example climb to the top

    //TODO ensure conservation of mass, energy
    //This also means kinetic energy cannot be converted to energy/mass
    //or maybe not

    GameRules gameRules;

    static public int MessageColliderLayer = 17;

    public GameObject playerRef;
    public GameObject clusterRef;
    public GameObject blockRef;
    public GameObject baseParticleRef;
    public Material particleLit;

    [ReadOnly]
    public float timeCounter = 0;

    private void OnEnable()
    {
        Start();
    }


    void Start()
    {
        //Add Cage Ref
        SetBasePhysicsRules();
    }

    void Update()
    {
    }

    void SetBasePhysicsRules()
    {
        foreach (KeyValuePair<string, ParticleType> entry in ParticleUtils.possibleTypes)
        {
            foreach (string ignoredCollision in entry.Value.ignoredCollisions)
            {
                Physics.IgnoreLayerCollision(layer1: ParticleUtils.possibleTypes[entry.Key].layer, layer2: ParticleUtils.possibleTypes[ignoredCollision].layer);
            }
        }
        Physics.IgnoreLayerCollision(layer1: MessageColliderLayer, layer2: MessageColliderLayer);
        Physics.IgnoreLayerCollision(layer1: ParticleUtils.blockLayer, layer2: ParticleUtils.noBlockCollisionLayer);
        Physics.IgnoreLayerCollision(layer1: ParticleUtils.noBlockCollisionLayer, layer2: ParticleUtils.noBlockCollisionLayer);
        Physics.IgnoreLayerCollision(layer1: ParticleUtils.noBlockCollisionLayer, layer2: ParticleUtils.noBlockCollisionLayer);
    }

    //Mass Rules Check
    public HashSet<ClusterBehavior> SystemClusters = new HashSet<ClusterBehavior>();

    public float TotalSystemMass()
    {
        float totalSystemMass = 0;
        foreach(ClusterBehavior cluster in SystemClusters)
        {
            totalSystemMass += cluster.totalMass;
        }
        return totalSystemMass;
    }

    public bool GravityCheck(ClusterBehavior cluster)
    {
        return true;
        return cluster.totalMass >= 0.25f * TotalSystemMass() && cluster.totalMass > 10; //TODO fix arbitrary number
    }


    //Spawn Utils
    void SpawnParticles(int seed, Vector3 bounds)
    {
        for (int i = 0; i < seed; i++)
        {
            BlockBehavior block = CreateBlock();
            block.transform.position = Vector3Utils.RandomBoundedVector3(bounds);
            block.transform.eulerAngles = Vector3Utils.RandomEulerAngles();
        }

        for (int i = 0; i < seed; i++)
        {
            ElectronBehavior electron = CreateElectron(1, 100, 1);
            electron.transform.position = Vector3Utils.RandomBoundedVector3(bounds);
        }
    }

    public BlockBehavior CreateBlock()
    //Created when collions occur between same types. Quarks, Leptons.
    //Created when mass is too high
    {
        GameObject block = Instantiate(blockRef);
        BlockBehavior blockBehavior = block.GetComponent<BlockBehavior>();
        blockBehavior.gameMaster = this;
        blockBehavior.Start();
        return blockBehavior;
    }

    public ClusterBehavior CreateCluster(HashSet<BlockBehavior> blocks)
    //Created when collions occur between same types. Quarks, Leptons.
    //Created when mass is too high
    {
        GameObject cluster = Instantiate(clusterRef);
        ClusterBehavior clusterBehavior = cluster.GetComponent<ClusterBehavior>();
        clusterBehavior.gameMaster = this;
        clusterBehavior.blocks = blocks;
        cluster.GetComponent<ClusterMessageBehavior>().Start();
        clusterBehavior.UpdateCenter();
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
        neutrinoBehavior.gameMaster = this;
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
        electronBehavior.gameMaster = this;
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
        posQuarkBehavior.gameMaster = this;
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
        negQuarkBehavior.gameMaster = this;
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
        photonBehavior.gameMaster = this;
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
        zBosonBehavior.gameMaster = this;
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
        wBosonBehavior.gameMaster = this;
        wBosonBehavior.Start();
        return wBosonBehavior;
    }
}
