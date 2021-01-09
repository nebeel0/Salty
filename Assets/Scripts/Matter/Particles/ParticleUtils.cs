using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParticleUtils
{
    public static int maxEnergyFactor = 100;

    public static int quarkLayer = 10;
    public static int leptonNegLayer = 11;
    public static int leptonNeutralLayer = 12;
    public static int pBosonLayer = 13;
    public static int wBosonLayer = 14;
    public static int zBosonLayer = 15;
    public static int noBlockCollisionLayer = 16;  //TODO make a better named layer
    public static int blockLayer = 8;

    public static string leptonNeg = "leptonNeg";
    public static string quarkPos = "quarkPos";
    public static string quarkNeg = "quarkNeg";
    public static string leptonNeutral = "leptonNeutral";
    public static string pBoson = "pBoson";
    public static string wBoson = "wBoson";
    public static string zBoson = "zBoson";

    public static bool isBoson(GameObject particle)
    {
        return isPhoton(particle) || isWBoson(particle) || isZBoson(particle);
    }
    public static bool isPhoton(GameObject particle)
    {
        return getParticleType(particle) == pBoson;
    }
    public static bool isWBoson(GameObject particle)
    {
        return getParticleType(particle) == wBoson;
    }

    public static bool isZBoson(GameObject particle)
    {
        return getParticleType(particle) == zBoson;
    }
    public static bool isFermion(GameObject particle)
    {
        return isLepton(particle) || isQuark(particle);
    }
    public static bool isQuark(GameObject particle)
    {
        return getParticleType(particle) == quarkPos || getParticleType(particle) == quarkNeg;
    }
    public static bool isLepton(GameObject particle)
    {
        return isNeutrino(particle) || isElectron(particle);
    }
    public static bool isNeutrino(GameObject particle)
    {
        return getParticleType(particle) == leptonNeutral;
    }
    public static bool isElectron(GameObject particle)
    {
        return getParticleType(particle) == leptonNeg;
    }

    public static string getParticleType(GameObject particle)
    {
        if(particle.GetComponent<ParticleBehavior>() == null)
        {
            return null;
        }
        else
        {
            return particle.GetComponent<ParticleBehavior>().particleType;
        }
    }

    public static bool areSameType(GameObject particle1, GameObject particle2)
    {
        string particleType1 = getParticleType(particle1);
        string particleType2 = getParticleType(particle2);

        return particleType1 == particleType2 && particle1 != null;
    }


    public static bool isParticle(GameObject gameObject)
    {
        return gameObject.GetComponent<ParticleBehavior>() != null;
    }

    public static string GetRandomState()
    {
        string[] randomInitialStates = { quarkPos, quarkNeg };
        return randomInitialStates[Random.Range(0, randomInitialStates.Length - 1)];  // Could use system.random to get a random pick
    }

    //TODO re-enable wBoson and zBoson

    public static Dictionary<string, ParticleType> possibleTypes = new Dictionary<string, ParticleType>()
    {
        [quarkPos] = new ParticleType(charge: 2, spinFactor: 1, mass: 5, layer: quarkLayer, ignoredCollisions: new string[] { }),  //Quark
        [quarkNeg] = new ParticleType(charge: -1, spinFactor: 1, mass: 10, layer: quarkLayer, ignoredCollisions: new string[] { }),  //Quark
        [leptonNeg] = new ParticleType(charge: -3, spinFactor: 1, mass: 1, layer: leptonNegLayer, ignoredCollisions: new string[] { quarkPos, quarkNeg }),  //Lepton Electron
        [leptonNeutral] = new ParticleType(charge: 0, spinFactor: 1, mass: 0.1f, layer: leptonNeutralLayer, ignoredCollisions: new string[] {quarkPos,quarkNeg }),  //Lepton Neutrino
        [pBoson] = new ParticleType(charge: 0, spinFactor: 2, mass: 0, layer: pBosonLayer, ignoredCollisions: new string[] { pBoson }),  //Boson Gluon, Photon
        //[wBoson] = new ParticleType(charge: 3, spinFactor: 2, mass: 5, layer: wBosonLayer, ignoredCollisions: new string[] { }),  //Boson W- Weak Force
        //[zBoson] = new ParticleType(charge: 0, spinFactor: 2, mass: 5, layer: zBosonLayer, ignoredCollisions: new string[] { quarkPos, quarkNeg, leptonNeg, wBoson }),  //Boson Z Momentum Force, Z Bosons can only interact with neutrinos
        // ["hBoson"] = new ParticleType(charge: 0, spinFactor: 0, maxMass: 1000, layer: 16),  //Save Higgs for last
    };    
}
