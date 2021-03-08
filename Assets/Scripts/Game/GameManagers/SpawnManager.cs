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
    public GameObject clusterGridRef;
    public GameObject clusterRef;
    public GameObject baseBlockRef;
    public GameObject baseParticleRef;
    public Material particleLit;

    public HashSet<GameObject> spawnedObjects = new HashSet<GameObject>();
    public HashSet<CageBehavior> SystemCage = new HashSet<CageBehavior>();
    public HashSet<BlockBehavior> SystemBlocks = new HashSet<BlockBehavior>();
    public HashSet<ParticleBehavior> SystemParticles = new HashSet<ParticleBehavior>();
    public HashSet<ClusterBehavior> SystemClusters = new HashSet<ClusterBehavior>();
    public HashSet<PlayerControlManager> players = new HashSet<PlayerControlManager>();
    public HashSet<PlayerControlManager> aiPlayers = new HashSet<PlayerControlManager>();

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
        players.Clear();
    }

    public void DestroyAI(PlayerControlManager aiPlayer)
    {
        aiPlayers.Remove(aiPlayer);
        spawnedObjects.Remove(aiPlayer.gameObject);
        Destroy(aiPlayer.gameObject);
    }

    public void DestroyEverything()
    {
        ClearPlayers();
        foreach (GameObject spawnedObject in spawnedObjects)
        {
            Destroy(spawnedObject);
        }
        GameObject[] remainingObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        for(int i =0; i < remainingObjects.Length; i++)
        {
            if(remainingObjects[i] != gameMaster.gameObject)
            {
                Destroy(remainingObjects[i]);
            }
        }
    }

    public void EquipDefaultPlayer(PlayerControlManager player)
    {
        QuantumBlockBehavior defaultBlock = CreateQuantumBlock();
        defaultBlock.BeginnerElementFlag = true;
        defaultBlock.Start();
        //player.AttachPlayer(defaultBlock.cluster);
    }

    //Player Join/Left Utils
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        PlayerControlManager player = playerInput.gameObject.GetComponent<PlayerControlManager>();
        player.gameMaster = gameMaster;
        players.Add(player);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        PlayerControlManager player = playerInput.gameObject.GetComponent<PlayerControlManager>();
        players.Remove(player);
    }

    //Spawn Utils
    public void SpawnQuantumBlocks(int seed, Vector3 bounds)
    {
        for (int i = 0; i < seed; i++)
        {
            QuantumBlockBehavior block = CreateQuantumBlock();
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

    public PlayerControlManager CreateAIPlayer()
    //Created when collions occur between same types. Quarks, Leptons.
    //Created when mass is too high
    {
        GameObject aiPlayer = Instantiate(aiRef);
        PlayerControlManager aiController = aiPlayer.GetComponent<PlayerControlManager>();
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

    public QuantumBlockBehavior CreateQuantumBlock(Transform transform=null)
    //Created when collions occur between same types. Quarks, Leptons.
    //Created when mass is too high
    {
        GameObject block = Instantiate(baseBlockRef, transform);
        QuantumBlockBehavior blockBehavior = block.AddComponent<QuantumBlockBehavior>();
        blockBehavior.gameMaster = gameMaster;
        blockBehavior.Start();
        spawnedObjects.Add(block);
        return blockBehavior;
    }

    public ClassicalBlockBehavior CreateClassicalBlock(Transform transform = null)
    //Created when collions occur between same types. Quarks, Leptons.
    //Created when mass is too high
    {
        GameObject block = Instantiate(baseBlockRef, transform);
        ClassicalBlockBehavior blockBehavior = block.AddComponent<ClassicalBlockBehavior>();
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
        CreateAIPlayer().AttachPlayer(clusterBehavior);
        SystemClusters.Add(clusterBehavior);
        spawnedObjects.Add(cluster);
        return clusterBehavior;
    }

    public void CreateClusterGrid(Transform transform)
    //Created when collions occur between same types. Quarks, Leptons.
    //Created when mass is too high
    {
        ClusterGridBehavior clusterGrid = Instantiate(clusterGridRef).GetComponent<ClusterGridBehavior>();
        clusterGrid.gameMaster = gameMaster;
        clusterGrid.transform.position = transform.position;
        clusterGrid.transform.eulerAngles = transform.eulerAngles;
        clusterGrid.transform.localScale = transform.localScale;
        clusterGrid.CreateGrid();
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
