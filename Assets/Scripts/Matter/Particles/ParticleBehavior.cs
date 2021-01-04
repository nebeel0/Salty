using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBehavior : MatterBehavior
{
    //Only Particle object can destroy itself
    //Codify instability for higher masses
    public GameObject particleRef;
    public GameObject blockRef;
    public class ParticleState
    {
        public int charge;
        public int spinFactor;
        public int maxMass;
        public int layer;
        public string[] ignoredCollisions;
        public ParticleState(int charge, int spinFactor, int maxMass, int layer, string[] ignoredCollisions)
        {
            this.charge = charge;
            this.spinFactor = spinFactor;
            this.maxMass = maxMass;
            this.layer = layer;
            this.ignoredCollisions = ignoredCollisions;
        }
    }
    //Max 6:3 Quarks or 7 Leptons
    //Eight possible charge spin combinations
    public Dictionary<string, ParticleState> possibleStates = new Dictionary<string, ParticleState>()
    {
        ["quarkPos"] = new ParticleState(charge: 2, spinFactor: 1, maxMass: 1000, layer: 10, ignoredCollisions: new string[]{}),  //Quark
        ["quarkNeg"] = new ParticleState(charge: -1, spinFactor: 1, maxMass: 100, layer: 10, ignoredCollisions: new string[] {}),  //Quark
        ["leptonNeg"] = new ParticleState(charge: -3, spinFactor: 1, maxMass: 50, layer: 11, ignoredCollisions: new string[] { "quarkPos" , "quarkNeg" }),  //Lepton Electron
        ["leptonNeutral"] = new ParticleState(charge: 0, spinFactor: 1, maxMass: 10, layer: 12, ignoredCollisions: new string[] { "quarkPos", "quarkNeg" }),  //Lepton Neutrino
        ["pBoson"] = new ParticleState(charge: 0, spinFactor: 2, maxMass: 0, layer: 13, ignoredCollisions: new string[] { "pBoson" }),  //Boson Gluon, Photon
        ["wBoson"] = new ParticleState(charge: 3, spinFactor: 2, maxMass: 500, layer: 14, ignoredCollisions: new string[] {}),  //Boson W- Weak Force
        ["zBoson"] = new ParticleState(charge: 0, spinFactor: 2, maxMass: 500, layer: 15, ignoredCollisions: new string[] { "quarkPos", "quarkNeg", "leptonNeg","wBoson" }),  //Boson Z Momentum Force, Z Bosons can only interact with neutrinos
        // ["hBoson"] = new ParticleState(charge: 0, spinFactor: 0, maxMass: 1000, layer: 16),  //Save Higgs for last
    };
    public int antiCharge = 1;
    public string particleStateType = "quarkPos"; //Have to initialize or methods will have issues
    public float energy;
    public int mass;
    public int charge
    {
        get {return possibleStates[particleStateType].charge;}
    }
    public int effectiveCharge
    {
        get { return antiCharge * charge; }
    }
    public int spinFactor //Determines position in cube.
    {
        get {return possibleStates[particleStateType].spinFactor;}
    }
    public int layer
    {
        get {return possibleStates[particleStateType].layer;}
    }
    public int maxMass
    {
        get {return possibleStates[particleStateType].maxMass;}
    }
    public int maxEnergy
    {
        get {return maxMass*100;}
    }

    public bool isFermion
    {
        get {return spinFactor==1;}
    }
    public bool isBoson
    {
        get {return spinFactor==2;}
    }
    public bool isPhoton
    {
        get {return isBoson && maxMass==0;}
    }
    public bool isQuark
    {
        get {return isFermion && (charge==2 || charge==-1);}
    }
    public bool isLepton
    {
        get {return isFermion && (charge==-3 || charge==0);}
    }
    public bool isNeutrino
    {
        get {return isLepton && charge==0;}
    }
    public bool isElectron
    {
        get { return isLepton && charge == -1; }
    }


    public bool available = true; //Used for placing in blocks, mainly for leptons, don't see a use yet for quarks
    //public BlockBehavior.LeptonPosition leptonPosition; //maybe wrong thing to do, but so much easier to track if its position is being tracked.
    private bool freeFlag = true;
    public void RandomState()
    {
        string[] randomInitialStates = { "quarkPos", "quarkNeg", "leptonNeg", "leptonPos" };
        particleStateType = randomInitialStates[Random.Range(0, randomInitialStates.Length - 1)];  // Could use system.random to get a random pick
    }
    public override void Start()
    {
        base.Start();
        DisableCollision();
        if(mass==0 && maxMass!=0)
        {
            mass = Random.Range(1,10); // Extra mass will be turned into other particles during update
        }
        if(energy==0)
        {
            energy = Random.Range(1, 10);
        }
        if (freeFlag)
        {
            Free();
        }
    }
    protected override void VisualUpdate()
    {
        Color newColor = new Color();
        newColor.r = 1 - ((float)maxMass / 1000);
        newColor.g = ((float)spinFactor / 2);
        newColor.b = ((float)charge + 3) / 6;
        if (antiCharge < 0)
        {
            newColor.r = 1 - newColor.r;
            newColor.g = 1 - newColor.g;
            newColor.b = 1 - newColor.b;
        }
        GetComponent<Renderer>().material.color = Color.Lerp(GetComponent<Renderer>().material.color, newColor, Time.deltaTime);
        GetComponent<TrailRenderer>().startColor = Color.Lerp(GetComponent<Renderer>().material.color, newColor, Time.deltaTime);
        //GetComponent<TrailRenderer>().endColor = Color.Lerp(GetComponent<Renderer>().material.color, newColor, 0.05f);
    }
    protected override void RefreshState()
    {
        if (mass <= 0 && !isPhoton)
        {
            mass = 0;
        }
        Energize(); //Increase energy if Fermion, Decrease if Boson, Do Nothing if Photon
        if (!playerOverride)
        {
            MaxEnergyDecay(); //Emit new energy, Photons
            MaxMassDecay(); //Emit new mass, Z Bosons
        }
        rigidbody.mass = mass;
    }
    protected override void DeathCheck()
    {
        //float particleMinEnergyLevel;

        //if (mass <= 0)
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        //if (isPhoton)
        //{
        //    particleMinEnergyLevel = 10;
        //}
        //else
        //{
        //    particleMinEnergyLevel = maxMass * 100 * 0.5f; //Half Life
        //}

        //if (energy <= particleMinEnergyLevel)
        //{
        //    if (isFermion)
        //    {
        //        if (MassDecayChance(this))
        //        {
        //            if (particleStateType == "quarkPos")
        //            {
        //                CreateWBoson(mass: mass, energy: energy, antiCharge: antiCharge * -1);
        //                mass = 1;
        //                particleStateType = "quarkNeg";
        //            }
        //            else if (particleStateType == "quarkNeg")
        //            {
        //                CreateWBoson(mass: mass, energy: energy, antiCharge: antiCharge);
        //                mass = 1;
        //                particleStateType = "quarkPos";
        //            }
        //            else if (particleStateType == "leptonNeg")
        //            {
        //                CreateWBoson(mass: mass, energy: energy, antiCharge: antiCharge * -1);
        //                Destroy(gameObject);
        //            }
        //            else if (particleStateType == "leptonNeutral")
        //            {
        //                CreateZBoson(mass: mass, energy: energy);
        //                Destroy(gameObject);
        //            }
        //            else
        //            {
        //                Debug.LogError(string.Format("A non-fermion will not decay into the weak force. Not {0}", particleStateType));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (isPhoton)
        //        {
        //            Destroy(gameObject);
        //        }
        //        else
        //        {
        //            BosonDecay();
        //        }
        //    }
        //}
    }
    public void Energize()
    {
        if(isFermion)
        {
            if(Random.Range(0,9) == 0) // 10% chance to increase energy
            {
                energy += 1 * Time.deltaTime;
            }
            if(Random.Range(0,99) == 0) // 1% chance to increase energy
            {
                energy += 10  * Time.deltaTime;
            }
            if(Random.Range(0,999) == 0) // 0.1% chance to increase energy
            {
                energy += 100 * Time.deltaTime;
            }
            if(energy < 1)
            {
                energy=1;
            }
        }
        else if(isBoson)
        {
            if(!isPhoton)
            {
                energy -= 1 * Time.deltaTime;
            }
        }
    }
    public void MaxEnergyDecay(float emittedEnergy=0)
    {
        if (emittedEnergy == 0)
        {
            if(energy > maxEnergy)
            {
                emittedEnergy = maxEnergy-energy+Random.Range(0, maxEnergy*0.50f);
            }
            else
            {
                emittedEnergy = Random.Range(0, energy*0.50f);
            }
        }

        if(isFermion)
        {
            if(energy > maxEnergy)  //TODO include a condition to fire off if playerOverride and fire key is pressed
            {
                ParticleBehavior photonBehavior = CreatePhoton(energy: emittedEnergy);
                energy -= photonBehavior.energy;
            }
        }
    }
    public void MaxMassDecay(int emittedMass=0)
    {
        if (emittedMass == 0)
        {
            if(mass > maxMass)
            {
                emittedMass = (int) (maxMass-mass+Random.Range(0, maxMass*0.50f));
            }
        }

        if(isFermion)
        {
            if(mass > maxMass)  //TODO include a condition to fire off if playerOverride and fire key is pressed
            {
                float emittedEnergy = (emittedMass/mass) * energy;
                mass -= emittedMass;
                energy -= emittedEnergy;
                ParticleBehavior zBosonBehavior = CreateZBoson(mass: emittedMass, emittedEnergy);
            }
        }
    }
    public void BosonDecay() // Initial pos outside of collision range. Create the new particles, offset, give initial velocities the same as curr, and create explosion force
    {
        if (isPhoton || isFermion)
        {
            Debug.LogError(string.Format("Only Z and W Bosons can split. Not {0}", particleStateType));
        }
        List<ParticleBehavior> particleBehaviors = new List<ParticleBehavior>();
        if (charge != 0) //wBoson
        {
            if(MassDecayChance(this))
            {
                particleBehaviors.Add(CreateElectron(mass: mass, energy: energy/2, antiCharge*-1));
                if(mass > ((float)maxMass/2) && mass > 100)
                {
                    particleBehaviors.Add(CreateQuarkNeg(mass: mass/2, energy: energy/2, antiCharge));
                    particleBehaviors.Add(CreateQuarkPos(mass: mass/2, energy: energy/2, antiCharge));
                }
                else
                {
                    particleBehaviors.Add(CreateNeutrino(mass: 1, energy: energy / 2));
                }
            }
            else
            {
                particleBehaviors.Add(CreateElectron(mass: mass, energy: energy, antiCharge*-1));
            }
        }
        else if(charge == 0) //zBoson
        {
            if(MassDecayChance(this))
            {
                for(int i=0; i<2; i++)
                {
                    particleBehaviors.Add(CreateNeutrino(mass: 1, energy: energy/2));
                }
                CreatePhoton(energy: mass*100); //Photons have their own movement system.
            }
        }
        else
        {
            Debug.LogError(string.Format("Okay only W and Z bosons should be decaying. Not {0}", particleStateType));
        }
        SpawnNewParticles(particleBehaviors);
        Debug.Log("Destroying the gameobject via bosonDecay");
        Destroy(gameObject);
    }
    void OnCollisionEnter(Collision col)
    {
        //Account for double collision effects for fermions
        if (col.gameObject.tag == "Particle")
        {
            DisableCollision();
            ParticleBehavior colParticleBehavior = col.gameObject.GetComponent<ParticleBehavior>();
            if (!isPhoton) //Photons don't get affected by anything, they only get absorbed
            {
                if (colParticleBehavior.particleStateType == particleStateType && colParticleBehavior.antiCharge != antiCharge)
                {
                    Annihilate(col.gameObject);
                }
                else if (colParticleBehavior.isQuark && isQuark && transform.parent == null && col.transform.parent == null) //Strong force interaction, generate a new block
                {
                    BlockBehavior newBlock = CreateBlock();
                    //newBlock.CollideParticle(this.gameObject);
                    //newBlock.CollideParticle(col.gameObject);
                    // Two fermion particle should always be able to be coupled
                    // Set relative velocity to zero, perhaps this logic should be in block behavior
                }
                else if (colParticleBehavior.isBoson && isFermion) //Absorb mass and energy if conditions succeed
                {
                    AbsorbEnergy(col.gameObject);
                    if (!colParticleBehavior.isPhoton)
                    {
                        BosonRadiation(col.gameObject);
                    }
                }
            }
        }
    }
    public void Annihilate(GameObject otherParticle)
    {
        ParticleBehavior otherBehavior = otherParticle.GetComponent<ParticleBehavior>();
        if(otherBehavior.particleStateType != particleStateType)
        {
            Debug.LogError("Can't annihilate non-different particles.");
        }
        int massLost = System.Math.Min(otherBehavior.mass, mass);
        otherBehavior.mass -= massLost;
        mass -= massLost;
    }
    //Decay Utils
    public bool MassDecayChance(ParticleBehavior particleBehavior) //If mass is high enough split into two quarks, else create a neutrino
    {
        return false;
        System.Random random = new System.Random();
        bool success = random.Next(1000) == 0;
        if(success)
        {
            return true;
        }
        else
        {
            return false;
        }

        if (mass <= 1 || energy < 0)
        {
            return true;
        }
        float energyChance;
        if(particleBehavior.energy >= particleBehavior.maxEnergy)
        {
            return false;
        }
        else
        {
            energyChance = particleBehavior.energy/particleBehavior.maxEnergy;
        }

        float massChance;
        if(particleBehavior.mass >= particleBehavior.maxMass)
        {
            massChance = 1;
        }
        else
        {
            massChance = ((particleBehavior.maxMass/(particleBehavior.maxMass-particleBehavior.mass))-1)/(particleBehavior.maxMass-1);
            massChance = System.Math.Max(massChance,1);
        }

        float chance = Mathf.Max(0, massChance-energyChance);
        int randomNum = Random.Range(10,100);
        return randomNum <= chance * 100;
    }
    void AbsorbEnergy(GameObject particle)
    // All particles absorb energy. Energy is a scalar.
    {
        if(isPhoton)
        {
            Debug.LogError("Photons cannot absorb energy");
        }

        ParticleBehavior particleBehavior = particle.GetComponent<ParticleBehavior>();

        float remainingEnergy = maxEnergy - energy;
        float energyCap = particleBehavior.energy;
        if (remainingEnergy > energyCap){
            energyCap = remainingEnergy;
        }
        float absorbedEnergy = Random.Range(energyCap/4,energyCap);
        particleBehavior.energy -= absorbedEnergy;
        energy += absorbedEnergy;
    }
    void BosonRadiation(GameObject particle)
    // If particles are anti annihilate
    {
        //Debug.Log("AbsorbMass");
        //float minMass = maxMass*0.1f;
        ParticleBehavior particleBehavior = particle.GetComponent<ParticleBehavior>();
        //if(!particleBehavior.isBoson || particleBehavior.isPhoton)
        //{
        //    Debug.LogError(string.Format("Only Bosons, which aren't photons can transfer mass. Not {0}", particleBehavior.particleStateType));
        //}
        //float remainingMass = maxMass - mass;
        //float massCap = particleBehavior.mass;
        //if (remainingMass > massCap){
        //    massCap = remainingMass;
        //}
        //int absorbedMass = (int)Random.Range(massCap/4,massCap);
        //particleBehavior.mass -= absorbedMass;
        //mass += absorbedMass;
        //if (particleBehavior.charge != 0)
        //{
        //    int newCharge = (particleBehavior.charge*particleBehavior.antiCharge) + (charge*antiCharge);
        //    if(newCharge < 0)
        //    {
        //        antiCharge *= -1;
        //        particleBehavior.antiCharge *= -1;
        //    }
        //}
    }
    //Block Utils
    public void Free()
    {
        freeFlag = true;
        available = true;
        transform.SetParent(null);
        EnableCollisionCooldownTimer();
        rigidbody.drag = 0.25f;
        rigidbody.angularDrag = 0.25f;
        rigidbody.isKinematic = false;
        gameObject.layer = layer;
        //leptonPosition = null;
    }
    public void Occupy(GameObject block)
    {
        freeFlag = false;
        transform.SetParent(block.transform);
        DisableCollisionCooldownTimer();
        DisableCollision();
        rigidbody.velocity = new Vector3(0, 0, 0);
        rigidbody.drag = 1;
        rigidbody.angularDrag = 1;
        rigidbody.isKinematic = true;
        gameObject.layer = noBlockCollisionLayer;
    }
    //Vector Utils
    Vector3 RandomTangent(Vector3 vector)
    {
        var normal = vector.normalized;
        var tangent = Vector3.Cross(normal, new Vector3(-normal.z, normal.x, normal.y));
        var bitangent = Vector3.Cross(normal, tangent);
        var angle = Random.Range(-Mathf.PI, Mathf.PI);
        return tangent * Mathf.Sin(angle) + bitangent * Mathf.Cos(angle);
    }
    Vector3 RandomCircle ( Vector3 center ,   float radius  ) //gets a random point from a sphere around center
    {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }
    void SpawnNewParticles(List<ParticleBehavior> particleBehaviors)
    //Set initial positions to be a sum of scale and colliding offset, should be automatically done
    //Set initial velocities, automatically done
    //Set colling to be Enabled
    //Set explosion
    {
        float radius = GetComponent<SphereCollider>().radius;
        float explosionForce = Mathf.Max(100.0f, energy);
        foreach (ParticleBehavior particleBehavior in particleBehaviors)
        {
            particleBehavior.DisableCollision();
        }
        foreach (ParticleBehavior particleBehavior in particleBehaviors)
        {
            particleBehavior.gameObject.transform.position = RandomCircle(transform.position, (radius+0.1f)*scalingFactor);
        }
        foreach (ParticleBehavior particleBehavior in particleBehaviors)
        {
            particleBehavior.rigidbody.AddExplosionForce(explosionForce: explosionForce, explosionPosition: transform.position, explosionRadius: (radius+0.2f)*scalingFactor);
        }

    }
    //Creating Particle Utils, have to deal with conversation of mass, energy, momentum
    ParticleBehavior CreateNeutrino(int mass, float energy, GameObject particleRef=null)
    //Neutrinos can be created by wBosons decaying
    {
        particleRef = particleRef == null ? this.particleRef : particleRef;
        GameObject neutrino = Instantiate(particleRef);
        ParticleBehavior neutrinoBehavior = neutrino.GetComponent<ParticleBehavior>();
        neutrinoBehavior.particleStateType = "leptonNeutral";
        neutrinoBehavior.rigidbody.velocity = rigidbody.velocity;
        neutrinoBehavior.rigidbody.isKinematic = false; // Movement should be affected by physics.
        return neutrinoBehavior;
    }
    public ParticleBehavior CreateElectron(int mass, float energy, int antiCharge=1, GameObject particleRef = null)
    //Leptons can be created by wBosons decaying
    {
        particleRef = particleRef == null ? this.particleRef : particleRef;
        GameObject electron = Instantiate(particleRef);
        ParticleBehavior electronBehavior = electron.GetComponent<ParticleBehavior>();
        electronBehavior.particleStateType = "leptonNeg";
        electronBehavior.antiCharge = antiCharge; 
        electronBehavior.rigidbody.velocity = rigidbody.velocity;
        electronBehavior.rigidbody.isKinematic = false; // Movement should be affected by physics.
        return electronBehavior;
    }
    //Quarks can be created by wBosons decaying
    ParticleBehavior CreateQuarkPos(int mass, float energy, int antiCharge=1, GameObject particleRef = null)
    {
        particleRef = particleRef == null ? this.particleRef : particleRef;
        GameObject posQuark = Instantiate(particleRef);
        ParticleBehavior posQuarkBehavior = posQuark.GetComponent<ParticleBehavior>();
        posQuarkBehavior.particleStateType = "quarkPos";
        posQuarkBehavior.antiCharge = antiCharge; 
        posQuarkBehavior.rigidbody.velocity = rigidbody.velocity;
        posQuarkBehavior.rigidbody.isKinematic = false; // Movement should be affected by physics.
        return posQuarkBehavior;
    }
    ParticleBehavior CreateQuarkNeg(int mass, float energy, int antiCharge=1, GameObject particleRef = null)
    {
        particleRef = particleRef == null ? this.particleRef : particleRef;
        GameObject negQuark = Instantiate(particleRef);
        ParticleBehavior negQuarkBehavior = negQuark.GetComponent<ParticleBehavior>();
        negQuarkBehavior.particleStateType = "quarkNeg";
        negQuarkBehavior.antiCharge = antiCharge; 
        negQuarkBehavior.rigidbody.velocity = rigidbody.velocity;
        negQuarkBehavior.rigidbody.isKinematic = false; // Movement should be affected by physics.
        return negQuarkBehavior;
    }
    ParticleBehavior CreatePhoton(float energy, GameObject particleRef = null)
    //Created by annihilation
    //Created by decay processes 
    //Created by collisions
    //All processes can cause photon creation, as it takes extra mass and converts to energy.
    {
        particleRef = particleRef == null ? this.particleRef : particleRef;
        GameObject photon = Instantiate(particleRef);
        ParticleBehavior photonBehavior = photon.GetComponent<ParticleBehavior>();
        photonBehavior.particleStateType = "pBoson";
        photonBehavior.rigidbody.velocity = rigidbody.velocity; //Velocity needs to be adjusted for photons. 
        photonBehavior.rigidbody.isKinematic = true; // Movement should not be affected by physics.
        photonBehavior.mass = 0;
        //TODO Set initial velocity
        return photonBehavior;
    }
    ParticleBehavior CreateZBoson(int mass, float energy, GameObject particleRef = null)
    //Created when collions occur between neutrinos
    {
        particleRef = particleRef == null ? this.particleRef : particleRef;
        GameObject zBoson = Instantiate(particleRef);
        ParticleBehavior zBosonBehavior = zBoson.GetComponent<ParticleBehavior>();
        zBosonBehavior.particleStateType = "zBoson";
        zBosonBehavior.rigidbody.velocity = rigidbody.velocity;
        zBosonBehavior.rigidbody.isKinematic = true; // Movement should not be affected by physics.
        return zBosonBehavior;
    }
    ParticleBehavior CreateWBoson(int mass, float energy, int antiCharge=1, GameObject particleRef = null)
    //Created when collions occur between same types. Quarks, Leptons.
    //Created when mass is too high
    {
        particleRef = particleRef == null ? this.particleRef : particleRef;
        GameObject wBoson = Instantiate(particleRef);
        ParticleBehavior wBosonBehavior = wBoson.GetComponent<ParticleBehavior>();
        wBosonBehavior.particleStateType = "wBoson";
        wBosonBehavior.antiCharge = antiCharge; 
        wBosonBehavior.rigidbody.velocity = rigidbody.velocity;
        wBosonBehavior.rigidbody.isKinematic = true; // Movement should not be affected by physics.
        return wBosonBehavior;
    }
    BlockBehavior CreateBlock()
    //Created when collions occur between same types. Quarks, Leptons.
    //Created when mass is too high
    {
        GameObject block = Instantiate(blockRef);
        BlockBehavior blockBehavior = block.GetComponent<BlockBehavior>();
        return blockBehavior;
    }
    //Visual Utils
}
