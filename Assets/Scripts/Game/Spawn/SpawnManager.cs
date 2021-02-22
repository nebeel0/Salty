using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnManager : MonoBehaviour
{
    public PlayMenuManager playMenuManager;

    public GameMaster gameMaster;

    public GameObject cageRef;
    public GameObject playerRef;
    public GameObject aiRef;
    public GameObject clusterRef;
    public GameObject blockRef;
    public GameObject baseParticleRef;
    public Material particleLit;

    public HashSet<GameObject> spawnedObjects = new HashSet<GameObject>();
    public HashSet<CageBehavior> SystemCage = new HashSet<CageBehavior>();
    public HashSet<BlockBehavior> SystemBlocks = new HashSet<BlockBehavior>();
    public HashSet<ParticleBehavior> SystemParticles = new HashSet<ParticleBehavior>();
    public HashSet<ClusterBehavior> SystemClusters = new HashSet<ClusterBehavior>();
    public HashSet<PlayerController> players = new HashSet<PlayerController>();
    public HashSet<PlayerController> aiPlayers = new HashSet<PlayerController>();

    public PlayerInputManager playerInputManager
    {
        get { return GetComponent<PlayerInputManager>(); }
    }

    void Update()
    {
        players.Remove(null);
    }

    public void ClearPlayers()
    {
        foreach(PlayerController player in players)
        {
            if(player != null)
            {
                Destroy(player.gameObject);
            }
        }
        players.Clear();
    }

    public void DestroyAI(PlayerController aiPlayer)
    {
        aiPlayers.Remove(aiPlayer);
        spawnedObjects.Remove(aiPlayer.gameObject);
        Destroy(aiPlayer.gameObject);
    }

    public void DestroyEverything()
    {
        foreach(PlayerController player in players)
        {
            player.transform.parent = null;
            player.OnGhostMode();
        }
        foreach (GameObject spawnedObject in spawnedObjects)
        {
            Destroy(spawnedObject);
        }
    }

    public void EquipDefaultPlayer(PlayerController player)
    {
        player.OnGhostMode();
        BlockBehavior defaultBlock = CreateBlock();
        defaultBlock.BeginnerElementFlag = true;
        defaultBlock.Start();
        defaultBlock.cluster.DetachDriver(player);
    }

    //Player Join/Left Utils
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        PlayerController player = playerInput.gameObject.GetComponent<PlayerController>();
        //BaseController[] controllers = player.GetComponents<BaseController>();
        //for(int i = 0; i < controllers.Length; i++)
        //{
        //    controllers[i].gameMaster = gameMaster; //Sets GameMaster
        //}
        players.Add(player);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        PlayerController player = playerInput.gameObject.GetComponent<PlayerController>();
        players.Remove(player);
    }

    //Spawn Utils
    public void SpawnBlocks(int seed, Vector3 bounds)
    {
        for (int i = 0; i < seed; i++)
        {
            BlockBehavior block = CreateBlock();
            block.BeginnerElementFlag = true;
            block.transform.position = Vector3Utils.RandomBoundedVector3(bounds);
            block.transform.eulerAngles = Vector3Utils.RandomEulerAngles();
        }
    }


    public void SpawnParticles(int seed, Vector3 bounds)
    {
        for (int i = 0; i < seed; i++)
        {
            ElectronBehavior electron = CreateElectron();
            electron.transform.position = Vector3Utils.RandomBoundedVector3(bounds);
        }
    }

    public PlayerController CreateAIPlayer()
    //Created when collions occur between same types. Quarks, Leptons.
    //Created when mass is too high
    {
        GameObject aiPlayer = Instantiate(aiRef);
        PlayerController aiController = aiPlayer.GetComponent<PlayerController>();
        aiController.gameMaster = gameMaster;

        aiPlayers.Add(aiController);
        spawnedObjects.Add(aiPlayer);
        return aiController;
    }

    public CageBehavior CreateCage(Vector3 bounds)
    //Created when collions occur between same types. Quarks, Leptons.
    //Created when mass is too high
    {
        GameObject cageObject = Instantiate(cageRef);
        CageBehavior cageBehavior = cageObject.GetComponent<CageBehavior>();
        cageBehavior.gameMaster = gameMaster;
        cageBehavior.dimensions = bounds;
        spawnedObjects.Add(cageObject);
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
        spawnedObjects.Add(block);
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
        clusterBehavior.BFSRefresh();
        clusterBehavior.DetachDriver(CreateAIPlayer());
        SystemClusters.Add(clusterBehavior);
        spawnedObjects.Add(cluster);
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
        spawnedObjects.Add(neutrino);
        return neutrinoBehavior;
    }

    public ElectronBehavior CreateElectron(int weightClass=1, float energy=100, int antiCharge=1)
    //Leptons can be created by wBosons decaying
    {
        GameObject electron = Instantiate(baseParticleRef);
        ElectronBehavior electronBehavior = electron.AddComponent<ElectronBehavior>();
        electronBehavior.energy = energy;
        electronBehavior.weightClass = weightClass;
        electronBehavior.antiCharge = antiCharge;
        electronBehavior.gameMaster = gameMaster;
        electronBehavior.Start();
        spawnedObjects.Add(electron);
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
        spawnedObjects.Add(posQuark);
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
        spawnedObjects.Add(negQuark);
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
        spawnedObjects.Add(photon);
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
        spawnedObjects.Add(zBoson);
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
        spawnedObjects.Add(wBoson);
        return wBosonBehavior;
    }
}
