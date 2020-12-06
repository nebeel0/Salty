using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBehaviorTest : MonoBehaviour
{
    public class ParticleState
    {
        public int charge;
        public int spinFactor;
        public int maxMass;
        public int layer;

        public ParticleState(int charge, int spinFactor, int maxMass, int layer)
        {
            this.charge = charge;
            this.spinFactor = spinFactor;
            this.maxMass = maxMass;
            this.layer = layer;
        }
    }
    //Max 6:3 Quarks or 7 Leptons
    //Eight possible charge spin combinations
    public Dictionary<string, ParticleState> possibleStates = new Dictionary<string, ParticleState>()
    {
        ["quarkPos"] = new ParticleState(charge: 2, spinFactor: 1, maxMass: 1000, layer: 10),  //Quark
        ["quarkNeg"] = new ParticleState(charge: -1, spinFactor: 1, maxMass: 10, layer: 10),  //Quark
        ["leptonNeg"] = new ParticleState(charge: -3, spinFactor: 1, maxMass: 5, layer: 11),  //Lepton Electron
        ["leptonNeutral"] = new ParticleState(charge: 0, spinFactor: 1, maxMass: 1, layer: 12),  //Lepton Neutrino
        ["pBoson"] = new ParticleState(charge: 0, spinFactor: 2, maxMass: 0, layer: 13),  //Boson Gluon, Photon
        ["wBoson"] = new ParticleState(charge: 3, spinFactor: 2, maxMass: 500, layer: 14),  //Boson W- Weak Force
        ["zBoson"] = new ParticleState(charge: 0, spinFactor: 2, maxMass: 500, layer: 15),  //Boson Z Momentum Force
        // ["hBoson"] = new ParticleState(charge: 0, spinFactor: 0, maxMass: 1000, layer: 16),  //Save Higgs for last
    };
    public bool playerOverride=false;
    public int antiCharge = 1;
    public string particleStateType = "quarkPos";
    public float energy;
    public int mass;
    public int charge
    {
        get {return possibleStates[particleStateType].charge;}
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
    private Rigidbody rigidbody
    {
        get {return GetComponent<Rigidbody>();}
    }
    private Collider collider
    {
        get {return GetComponent<Collider>();}
    }

    void Start()
    {
        if(particleStateType is null)
        {
            string[] randomInitialStates = {"quarkPos", "quarkNeg", "leptonNeg", "leptonPos"};
            particleStateType = randomInitialStates[Random.Range(0,randomInitialStates.Length-1)];
        }
        if(mass==0 && maxMass!=0)
        {
            mass = Random.Range(1,10); // Extra mass will be turned into other particles during update
        }
        if(energy==0)
        {
            energy = Random.Range(1, 10);
        }
        gameObject.layer = layer;
        Debug.Log(string.Format("particleStateType: {}",particleStateType.ToString()));
        Debug.Log(string.Format("energy: {}",energy.ToString()));
        Debug.Log(string.Format("mass: {}",mass.ToString()));
    }

}
