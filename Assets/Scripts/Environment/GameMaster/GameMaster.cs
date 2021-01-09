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

    ParticleBehavior particleEnv = new ParticleBehavior();
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
    //Spawn Utils
    void SpawnParticles(int seed, Vector3 bounds)
    {
        for (int i = 0; i < seed; i++)
        {
            GameObject block = Instantiate(blockRef);
            block.transform.position = Vector3Utils.RandomBoundedVector3(bounds);
            block.transform.eulerAngles = Vector3Utils.RandomEulerAngles();
        }

        for (int i = 0; i < seed; i++)
        {
            GameObject lepton = Instantiate(baseParticleRef);
            ParticleBehavior leptonBehavior = lepton.GetComponent<ParticleBehavior>();
            leptonBehavior.particleType = "leptonNeg";
            lepton.transform.position = Vector3Utils.RandomBoundedVector3(bounds);
        }
    }

    public BlockBehavior CreateBlock()
    //Created when collions occur between same types. Quarks, Leptons.
    //Created when mass is too high
    {
        GameObject block = Instantiate(blockRef);
        BlockBehavior blockBehavior = block.GetComponent<BlockBehavior>();
        return blockBehavior;
    }

    public NeutrinoBehavior CreateNeutrino(float energy, int weightClass)
    //Neutrinos can be created by wBosons decaying
    {
        GameObject neutrino = Instantiate(baseParticleRef);
        NeutrinoBehavior neutrinoBehavior = neutrino.AddComponent<NeutrinoBehavior>();
        neutrinoBehavior.energy = energy;
        neutrinoBehavior.weightClass = weightClass;
        return neutrinoBehavior;
    }

    public ElectronBehavior CreateElectron(int weightClass, float energy, int antiCharge)
    //Leptons can be created by wBosons decaying
    {
        GameObject electron = Instantiate(baseParticleRef);
        ElectronBehavior electronBehavior = electron.AddComponent<ElectronBehavior>();
        electronBehavior.particleType = "leptonNeg";
        electronBehavior.energy = energy;
        electronBehavior.weightClass = weightClass;
        electronBehavior.antiCharge = antiCharge;
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
        return posQuarkBehavior;
    }
    public DownBehavior CreateQuarkNeg(int weightClass, float energy, int antiCharge)
    {

        GameObject negQuark = Instantiate(baseParticleRef);
        DownBehavior negQuarkBehavior = negQuark.AddComponent<DownBehavior>();
        negQuarkBehavior.energy = energy;
        negQuarkBehavior.weightClass = weightClass;
        negQuarkBehavior.antiCharge = antiCharge;
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
        return photonBehavior;
    }
    public ZBosonBehavior CreateZBoson(int weightClass, float energy)
    //Created when collions occur between neutrinos
    {
        GameObject zBoson = Instantiate(baseParticleRef);
        ZBosonBehavior zBosonBehavior = zBoson.AddComponent<ZBosonBehavior>();
        zBosonBehavior.energy = energy;
        zBosonBehavior.weightClass = weightClass;
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
        return wBosonBehavior;
    }
}
