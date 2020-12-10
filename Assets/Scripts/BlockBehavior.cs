using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehavior : MatterBehavior
{
    //TODO on merge change camera position
    // TODO no mixing of anti and regular particles, when they clash, annihilation must happen, nvm I was wrong
    // TODO rotate on block place
    public class QuarkGroup
    {
        public static int maxLength = 3;
        public GameObject[] quarks = new GameObject[maxLength];
        public int netCharge
        {
            get
            {
                int netCharge = 0;
                for(int i = 0; i < maxLength; i++)
                {
                    if(quarks[i] != null)
                    {
                        netCharge += quarks[i].GetComponent<ParticleBehavior>().effectiveCharge;
                    }
                    else
                    {
                        return 0; // zero if not full
                    }
                }
                return netCharge;
            }
        }
        public QuarkGroup(GameObject quark)
        {
            quarks[0] = quark;
        }

        public bool Valid()
        {
            bool filledUp = true;
            int productCharge = 1;
            int totalCharge = 0;
            for (int i = 0; i < maxLength; i++)
            {
                GameObject quark = quarks[i];
                if (quark != null)
                {
                    totalCharge += quark.GetComponent<ParticleBehavior>().effectiveCharge;
                    productCharge *= quark.GetComponent<ParticleBehavior>().effectiveCharge;
                }
                else
                {
                    filledUp = false;
                }
            }
            if(filledUp)
            {
                bool maxTwoSame = productCharge < 0;
                return (totalCharge == 3 || totalCharge == 0) && maxTwoSame;
            }
            return true;
        }
    }
    public class BlockConnection
    {
        int forceMultiplier = 1000;
        public Vector3Int otherBlockLocalPosition;
        public GameObject thisBlock;
        public GameObject otherBlock;
        public FixedJoint fixedJoint;
        public List<GameObject> leptons;
        public List<GameObject> otherLeptons;
        public bool inPlace;
        public bool hasBlock
        {
            get { return otherBlock != null; }
        }

        public bool hasFixedJoint
        {
            get { return fixedJoint != null && fixedJoint.connectedBody != null; }
        }

        public int totalLeptonCount
        {
            get { return leptons.Count + otherLeptons.Count; }
        }
        public int netOtherCharge
        {
            get
            {
                int netOtherCharge = 0;
                for (int i = 0; i < otherLeptons.Count; i++)
                {
                    if (leptons[i] != null)
                    {
                        netOtherCharge += otherLeptons[i].GetComponent<ParticleBehavior>().effectiveCharge;
                    }
                }
                return netOtherCharge;
            }
        }
        public BlockConnection(GameObject thisBlock, Vector3Int otherBlockLocalPosition)
        {
            this.otherBlockLocalPosition = otherBlockLocalPosition;
            this.thisBlock = thisBlock;
            otherBlock = null;
            fixedJoint = null;
            inPlace = false;
            leptons = new List<GameObject>();
            otherLeptons = new List<GameObject>();
        }

        public void Refresh()
        {
            DeathCheck();
            if (hasFixedJoint)
            {
                for (int i = 0; i < leptons.Count; i++)
                {
                    leptons[i].GetComponent<ParticleBehavior>().available = false;
                }
                //for (int i = 0; i < otherLeptons.Count; i++)
                //{
                //    otherLeptons[i].GetComponent<ParticleBehavior>().available = false;
                //}
                fixedJoint.breakForce = forceMultiplier * totalLeptonCount; // TODO exponential strength
                fixedJoint.breakTorque = forceMultiplier * totalLeptonCount;
            }
        }

        public void DeathCheck()
        {
            leptons.RemoveAll(lepton => lepton == null);
            otherLeptons.RemoveAll(lepton => lepton == null);

            if( inPlace && (!hasBlock || !hasFixedJoint))
            {
                if (!hasFixedJoint || leptons.Count == 0 || otherLeptons.Count == 0)
                {
                    if(hasBlock && otherBlock != null)
                    {
                        Physics.IgnoreCollision(otherBlock.GetComponent<Collider>(), thisBlock.GetComponent<Collider>(), false);
                    }
                    thisBlock.GetComponent<BlockBehavior>().DisableCollision();
                    Destroy(fixedJoint);
                    otherBlock = null;
                    fixedJoint = null;
                    leptons.Clear();
                    otherLeptons.Clear();
                    inPlace = false;
                    
                    if(thisBlock != null && otherBlock != null)
                    {
                        if (thisBlock.transform.parent == otherBlock.transform)
                        {
                            thisBlock.transform.parent = null;
                        }
                        else if (otherBlock.transform.parent == thisBlock.transform)
                        {
                            otherBlock.transform.parent = null;
                        }
                    }
                }
            }
        }

        public void Reflect(BlockConnection otherBlockConnection)
        {
            fixedJoint = otherBlockConnection.fixedJoint;
            otherBlock = otherBlockConnection.thisBlock;
            leptons = otherBlockConnection.otherLeptons;
            otherLeptons = otherBlockConnection.leptons;
            inPlace = true;
        }

        public void Place()
        {
            int lockOnSpeed = 10;
            if (hasBlock && !inPlace)
            {
                if (otherBlock.transform.localPosition != otherBlockLocalPosition)
                {
                    if (MatterBehavior.V3Equal(otherBlock.transform.localPosition, otherBlockLocalPosition))
                    {
                        otherBlock.transform.SetParent(thisBlock.transform);
                        Vector3 otherBlockEulerAngles = otherBlock.transform.localEulerAngles;
                        otherBlockEulerAngles.x = Mathf.LerpAngle(otherBlockEulerAngles.x, Mathf.Round(otherBlockEulerAngles.x / 90) * 90, 1);
                        otherBlockEulerAngles.y = Mathf.LerpAngle(otherBlockEulerAngles.y, Mathf.Round(otherBlockEulerAngles.y / 90) * 90, 1);
                        otherBlockEulerAngles.z = Mathf.LerpAngle(otherBlockEulerAngles.z, Mathf.Round(otherBlockEulerAngles.z / 90) * 90, 1);
                        otherBlock.transform.localEulerAngles = otherBlockEulerAngles;
                        otherBlock.transform.localPosition = otherBlockLocalPosition;
                        otherBlock.transform.SetParent(null);

                        fixedJoint = thisBlock.AddComponent<FixedJoint>();
                        fixedJoint.enableCollision = false;
                        fixedJoint.connectedBody = otherBlock.GetComponent<Rigidbody>();
                        inPlace = true;

                        //sets other blockConnection to this joint
                        BlockBehavior otherBlockBehavior = otherBlock.GetComponent<BlockBehavior>();
                        thisBlock.transform.SetParent(otherBlock.transform);
                        Vector3Int positionFromOtherBlock = Vector3Int.RoundToInt(thisBlock.transform.localPosition);
                        BlockConnection otherBlockConnection = otherBlockBehavior.connectedBlocks[positionFromOtherBlock.ToString()];
                        otherBlockConnection.Reflect(this);
                        thisBlock.transform.SetParent(null);
                    }
                    else
                    {
                        otherBlock.transform.SetParent(thisBlock.transform);
                        Vector3 otherBlockEulerAngles = otherBlock.transform.localEulerAngles;
                        otherBlockEulerAngles.x = Mathf.LerpAngle(otherBlockEulerAngles.x, Mathf.Round(otherBlockEulerAngles.x / 90) * 90, lockOnSpeed* Time.deltaTime);
                        otherBlockEulerAngles.y = Mathf.LerpAngle(otherBlockEulerAngles.y, Mathf.Round(otherBlockEulerAngles.y / 90) * 90, lockOnSpeed * Time.deltaTime);
                        otherBlockEulerAngles.z = Mathf.LerpAngle(otherBlockEulerAngles.z, Mathf.Round(otherBlockEulerAngles.z / 90) * 90, lockOnSpeed * Time.deltaTime);
                        otherBlock.transform.localEulerAngles = otherBlockEulerAngles;
                        otherBlock.transform.localPosition = Vector3.Lerp(otherBlock.transform.localPosition, otherBlockLocalPosition, lockOnSpeed * Time.deltaTime);
                    }
                }
            }
        }
    }
    public class LeptonPosition
    {
        public GameObject lepton;
        public Vector3 position;
        public int[] neighborIdx;
        public int id;
    }
    public GameObject particleRef;
    public int particleSpeed = 5;
    public int netChargeCache;  // To be used similar to a cache, meaning that while its not the most accurate representation of the current state, it will reduce computations
    List<QuarkGroup> quarkGroups = new List<QuarkGroup>(); //fifteen max
    public int numQuarkGroups
    {
        get { return quarkGroups.Count; }
    }

    Vector3[] quarkPositions;
    int quarkGroupMax;
    
    List<GameObject> leptons = new List<GameObject>();
    LeptonPosition[] allLeptonPositions;
    List<int> openLeptonPositions;
    int leptonsMax = 8;  //TODO programmatically figure out max number of leptons, which we can figure out from number of vertexes in shape
    Dictionary<string, BlockConnection> connectedBlocks;

    public override void Start()
    {
        base.Start();
        SetUpQuarkPositions();
        SetUpLeptonPositions();
        SetUpConnectedBlocks();
        DisableCollision();
        BeginnerElement(); //TODO replace with RandomElement
    }

    protected override void VisualUpdate() //TODO visual update should always reflect actual state
    {
        PlaceQuarkGroups();
        PlaceLeptons();
        PlaceBlocks();
    }
    protected override void RefreshState()
    {
        RefreshQuarkGroups();
        RefreshLeptons();
        RefreshBlocks();
        RefreshNetCharge();
    }
    protected override void DeathCheck()
    {
        if (quarkGroups.Count == 0)
        {
            while (leptons.Count > 0)
            {
                RemoveLepton(0);
            }
            Destroy(gameObject);  // TODO maybe a death loading animation before hand?
        }
    }
    void OnCollisionEnter(Collision col)  // TODO Use C# Job System to avoid extra subatomic particles or leptons than possible
    {
        if (col.gameObject.CompareTag("Particle"))
        {
            DisableCollision();
            CollideParticle(col.gameObject);
        }
        if (col.gameObject.CompareTag("Block"))
        {
            DisableCollision();
            CollideBlock(col.gameObject);
        }
    }
    // max number of particles is 10, acceptable indices are from 0-1, and a factor 3, min -2

    void BeginnerElement()
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject particle = Instantiate(particleRef, transform);
            ParticleBehavior particleBehavior = getParticleData(particle);
            if (i % 3 != 0)
            {
                particleBehavior.particleStateType = "quarkPos";
            }
            else
            {
                particleBehavior.particleStateType = "quarkNeg";
            }

            if ((i + 1) % 4 == 0)
            {
                GameObject lepton = Instantiate(particleRef, transform);
                ParticleBehavior leptonBehavior = getParticleData(lepton);
                leptonBehavior.particleStateType = "leptonNeg";
                CollideParticle(lepton);
            }
            CollideParticle(particle);
        }
    }
    void RandomElement()
    {
        int numberOfParticles = Random.Range(1, 10);
        for (int i = 0; i < numberOfParticles; i++)
        {
            GameObject particle = Instantiate(particleRef);
            ParticleBehavior particleBehavior = getParticleData(particle);
            particleBehavior.RandomState();
            CollideParticle(particle);
        }
    }

    protected void RefreshQuarkGroups() // TODO quarks cooldown, give players a chance to recollect particles, and computers a rendering break
    {
        //If netCharge != all quarks plus each other rescatter.
        if (netChargeCache < 0) //Imbalanced, more electrons than protons
        {
            Dictionary<int, List<GameObject>> quarkDictionary = new Dictionary<int, List<GameObject>>();
            foreach (QuarkGroup quarkGroup in quarkGroups)
            {
                for (int i = 0; i < QuarkGroup.maxLength; i++)
                {
                    GameObject quark = quarkGroup.quarks[i];
                    if (quark != null)
                    {
                        int quarkCharge = quark.GetComponent<ParticleBehavior>().effectiveCharge;
                        if (!quarkDictionary.ContainsKey(quarkCharge))
                        {
                            quarkDictionary[quarkCharge] = new List<GameObject>();
                        }
                        quarkDictionary[quarkCharge].Add(quark);
                    }
                }
            }

            //Make as many protons as possible, till net Charge is back to zero
            quarkGroups = new List<QuarkGroup>(); //reset Quark Groups
            int netLeptonCharge = GetNetLeptonCharge();
            while(netLeptonCharge + GetNetQuarkCharge() < 0)
            {
                if(quarkDictionary.ContainsKey(2) && quarkDictionary.ContainsKey(-1) && quarkDictionary[2].Count >= 2 && quarkDictionary[-1].Count >= 1)
                {
                    QuarkGroup proton = new QuarkGroup(quarkDictionary[-1][0]);
                    quarkDictionary[-1].RemoveAt(0);
                    for (int i = 1; i <= 2; i++)
                    {
                        GameObject upParticle = quarkDictionary[2][0];
                        proton.quarks[i] = upParticle;
                        quarkDictionary[2].RemoveAt(0);
                    }
                    quarkGroups.Add(proton);
                }
                else
                {
                    break;
                }
            }

            foreach (List<GameObject> quarkList in quarkDictionary.Values)
            {
                // For rest of particles just add it back normally
                foreach(GameObject quark in quarkList)
                {
                    if(!AddQuark(quark))
                    {
                        netChargeCache -= quark.GetComponent<ParticleBehavior>().effectiveCharge;
                        quark.GetComponent<ParticleBehavior>().Free();
                    }
                }
            }
        }
        netChargeCache = GetNetLeptonCharge() + GetNetQuarkCharge();
    }

    protected void RefreshLeptons()
    {
        leptons.RemoveAll(lepton => lepton == null);
        for(int i = 0; i < leptons.Count; i++)
        {
            leptons[i].GetComponent<ParticleBehavior>().available = true;
        }

        int leptonI = 0;
        while (leptonI < leptons.Count - 1) //NO NEUTRINOS
        {
            if (leptons[leptonI].GetComponent<ParticleBehavior>().effectiveCharge == 0)
            {
                RemoveLepton(leptonI);
            }
            else
            {
                leptonI += 1;
            }
        }

        leptonI = 0;
        while (!NetLeptonChargeValid() && (leptons.Count > 0) && (leptonI < leptons.Count - 1))
        {
            if (getParticleData(leptons[leptonI]).effectiveCharge < 0)
            {
                RemoveLepton(leptonI);
            }
            else
            {
                leptonI += 1;
            }
        }
    }

    protected void RefreshBlocks()
    {
        foreach(BlockConnection blockConnection in connectedBlocks.Values)
        {
            blockConnection.Refresh();
        }
    }

    protected void RefreshNetCharge()
    {
        //TODO cooldown, every 5 seconds,
        netChargeCache = GetNetLeptonCharge() + GetNetQuarkCharge();
    }
    bool AddQuark(GameObject quark)  //Race Condition
    {
        ParticleBehavior quarkBehavior = quark.GetComponent<ParticleBehavior>();
        if(!quarkBehavior.isQuark)
        {
            return false;
        }
        // TODO figure out if we want particles to fill up sequentually or in parallel
        for(int quarkGroupI=quarkGroups.Count-1; quarkGroupI >=0; quarkGroupI--)
        {
            QuarkGroup quarkGroup = quarkGroups[quarkGroupI];
            for (int i = 0; i < quarkGroup.quarks.Length; i++)
            {
                if (quarkGroup.quarks[i] == null)
                {
                    quarkGroup.quarks[i] = quark;
                    if (quarkGroup.Valid())
                    {
                        netChargeCache += quarkBehavior.effectiveCharge;
                        quark.GetComponent<ParticleBehavior>().Occupy(gameObject);
                        return true;
                    }
                    else
                    {
                        quarkGroup.quarks[i] = null;
                        break;
                    }
                }
            }
        }

        if(quarkGroups.Count < quarkGroupMax)
        {
            netChargeCache += quarkBehavior.effectiveCharge;
            quarkGroups.Add(new QuarkGroup(quark));
            quark.GetComponent<ParticleBehavior>().Occupy(gameObject);
            return true;
        }

        return false;
    }

    void AddLepton(GameObject particle)
    {
        if(NetLeptonChargeValid(particle))
        {
            ParticleBehavior particleBehavior = getParticleData(particle);
            if (!particleBehavior.isLepton)
            {
                Debug.LogError("Particle needs to be lepton");
            }
            leptons.Add(particle);
            netChargeCache += particleBehavior.effectiveCharge;
            particleBehavior.Occupy(gameObject);
        }
    }

    void RemoveLepton(int particleIndex)
    {
        GameObject particle = leptons[particleIndex];
        leptons.RemoveAt(particleIndex);
        ParticleBehavior particleBehavior = getParticleData(particle);
        netChargeCache -= particleBehavior.effectiveCharge;
        particleBehavior.Free();
    }
    public void CollideParticle(GameObject particle)
    {
        ParticleBehavior particleBehavior = getParticleData(particle);
        if (particleBehavior.isFermion)
        {
            if (!AddQuark(particle))
            {
                AddLepton(particle);
            }
        }
    }
    void CollideBlock(GameObject otherBlock) //TODO animation for connecting
    {
        if (otherBlock.transform.parent == transform || transform.parent == otherBlock.transform)
        {
            return;
        }
        otherBlock.transform.SetParent(transform);
        string closestBlockConnectionI = "";
        float closestDistance = 0;
        foreach(KeyValuePair<string, BlockConnection> connectedBlock in connectedBlocks)
        {
            float distance = Vector3.Distance(connectedBlock.Value.otherBlockLocalPosition, otherBlock.transform.localPosition);
            if(closestBlockConnectionI == "" || distance < closestDistance)
            {
                closestBlockConnectionI = connectedBlock.Key;
                closestDistance = distance;
            }
        }
        if(!connectedBlocks[closestBlockConnectionI].hasBlock)
        {
            BlockBehavior otherBlockBehavior = otherBlock.GetComponent<BlockBehavior>();
            List<GameObject> otherBlockAvailableLeptons = otherBlockBehavior.GetAvailableLeptons();
            int otherBlockAvailableLeptonSpace = otherBlockBehavior.GetAvailableLeptonSpace();

            List<GameObject> availableLeptons = GetAvailableLeptons();
            int availableLeptonSpace = GetAvailableLeptonSpace();

            bool enoughSpace = otherBlockAvailableLeptonSpace > 0 && availableLeptonSpace > 0;
            bool enoughCharge = otherBlockBehavior.netChargeCache > 0 && netChargeCache > 0;
            bool enoughLeptons = otherBlockAvailableLeptons.Count > 0 && availableLeptons.Count > 0;
            if (enoughSpace && enoughCharge && enoughLeptons) //Both blocks must contribute leptons
            {
                // How many electrons to give up, depends on the available space from the otherBlock and how many this block can and have to give up
                // How many you have to give up depends on how many connected blocks you have
                if (LastLeptonPairing())
                {
                    //pop all
                    if (otherBlockAvailableLeptonSpace >= availableLeptons.Count)
                    {
                        List<GameObject> newOtherLeptons = otherBlockAvailableLeptons.GetRange(0, System.Math.Min(otherBlockAvailableLeptons.Count, availableLeptonSpace));
                        AddBlock(blockConnection: connectedBlocks[closestBlockConnectionI], otherBlock: otherBlock, leptons: availableLeptons, otherLeptons: newOtherLeptons);
                        return;
                    }
                }
                //else if(otherBlockBehavior.LastLeptonPairing())
                //{
                //    //take all
                //    if (availableLeptonSpace >= otherBlockAvailableLeptons.Count)
                //    {
                //        List<GameObject> newLeptons = availableLeptons.GetRange(0, System.Math.Min(availableLeptons.Count, otherBlockAvailableLeptonSpace));
                //        AddBlock(blockConnection: connectedBlocks[closestPoint], otherBlock: otherBlock, leptons: newLeptons, otherLeptons: otherBlockAvailableLeptons);
                //    }
                //}
                else
                {
                    AddBlock(blockConnection: connectedBlocks[closestBlockConnectionI], otherBlock: otherBlock, leptons: availableLeptons.GetRange(0,1), otherLeptons: otherBlockAvailableLeptons.GetRange(0, 1));
                    return;
                }
            }
        }
        otherBlock.transform.SetParent(null);
    }
    void AddBlock(BlockConnection blockConnection, GameObject otherBlock, List<GameObject> leptons, List<GameObject> otherLeptons)
    {
        Physics.IgnoreCollision(otherBlock.GetComponent<Collider>(), GetComponent<Collider>(), true);
        blockConnection.otherBlock = otherBlock;
        blockConnection.leptons = leptons;
        blockConnection.otherLeptons = otherLeptons;
        blockConnection.Refresh();
    }

    bool LastLeptonPairing()
    {
        return GetAvailableLeptons().Count + GetAvailableLeptonSpace() <= 4;
    }

    int GetAvailableLeptonSpace()
    {
        int availableSpace = leptonsMax - leptons.Count;
        foreach(BlockConnection blockConnection in connectedBlocks.Values)
        {
            availableSpace -= blockConnection.totalLeptonCount;
        }
        return System.Math.Max(availableSpace, 0);
    }

    List<GameObject> GetAvailableLeptons()
    {
        List<GameObject> availableLeptons = new List<GameObject>();
        foreach(GameObject lepton in leptons)
        {
            if (lepton.GetComponent<ParticleBehavior>().available)
            {
                availableLeptons.Add(lepton);
            }
        }
        return availableLeptons;
    }


    // Visual Update Utils
    void PlaceQuarkGroups()
    {
        int quarkPositionI = 0;
        for (int i = 0; i < quarkGroups.Count; i++)
        {
            for(int ii=0; ii < QuarkGroup.maxLength; ii++)
            {
                GameObject quark = quarkGroups[i].quarks[ii];
                if(quark != null)
                {
                    quark.transform.localPosition = Vector3.Lerp(quark.transform.localPosition, quarkPositions[quarkPositionI], particleSpeed / 2 * Time.deltaTime);
                    quarkPositionI++; // So that the particles will be displaced from each other, if they are not in the same groups
                }
            }
        }
    }

    void PlaceBlocks() //Once it has been put in place add the fixed joint
    {
        foreach (KeyValuePair<string, BlockConnection> connectedBlock in connectedBlocks)
        {
            connectedBlock.Value.Place();
        }
    }


    void PlaceLeptons()
    {
        // TODO lock leptons if they are used to connect blocks
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
            leptonPosition.lepton.transform.localPosition = Vector3.Lerp(leptonPosition.lepton.transform.localPosition, leptonPosition.position, particleSpeed * Time.deltaTime);
        }
    }
    void SetUpConnectedBlocks()
    {
        connectedBlocks = new Dictionary<string, BlockConnection>();
        for (int i = 1; i <= 2; i++)
        {
            for (int ii = 0; ii < 3; ii++)
            {
                Vector3Int newFixedJointPosition = Vector3Int.zero;
                newFixedJointPosition[ii] = (int) System.Math.Pow(-1, i) * (int) scalingFactor;
                connectedBlocks.Add(newFixedJointPosition.ToString(), new BlockConnection(gameObject, newFixedJointPosition));
            }
        }
    }
    void SetUpLeptonPositions()
    {
        allLeptonPositions = new LeptonPosition[leptonsMax];
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

    void SetUpQuarkPositions()
    {
        //Setting up quarkGroupMax from leptonMax, ie bottlenecked on max neg charge
        quarkGroupMax = 1;
        int quarkGroupFibonnacciI = 2;
        while (quarkGroupMax < leptonsMax)
        {
            quarkGroupMax += quarkGroupFibonnacciI++;
        }

        quarkPositions = new Vector3[quarkGroupMax*QuarkGroup.maxLength];
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

    int Vector3ToIdx(int i, int ii, int iii)
    {
        string binaryRep = string.Format("{0}{1}{2}", iii, ii, i);
        return System.Convert.ToInt32(binaryRep, 2);
    }

    // Refresh State Utils
    public int GetNetLeptonCharge() //TODO get outside lepton charge too
    { 
        int leptonNetCharge = 0;
        for(int i = 0; i < leptons.Count; i++)
        {
            ParticleBehavior particleBehavior = getParticleData(leptons[i]);
            leptonNetCharge += particleBehavior.effectiveCharge;
        }

        foreach(BlockConnection blockConnection in connectedBlocks.Values)
        {
            if(blockConnection.hasBlock)
            {
                leptonNetCharge += blockConnection.netOtherCharge;
            }
        }

        return leptonNetCharge;
    }

    public int GetNetQuarkCharge()
    {
        int quarkNetCharge = 0;
        foreach (QuarkGroup quarkGroup in quarkGroups)
        {
            quarkNetCharge += quarkGroup.netCharge;
        }
        return quarkNetCharge;
    }

    bool NetLeptonChargeValid(GameObject particle=null)
    {
        bool check;
        if (particle is null)
        {
            check = (netChargeCache >= 0) && (leptons.Count <= leptonsMax - 1);
        }
        else
        {
            ParticleBehavior particleBehavior = particle.GetComponent<ParticleBehavior>();
            check = particleBehavior.isLepton && (particleBehavior.effectiveCharge + netChargeCache >= 0) && (leptons.Count <= leptonsMax-1);
        }
        return check;
    }


    ParticleBehavior getParticleData(GameObject particle)
    {
        return particle.GetComponent<ParticleBehavior>();
    }
}
