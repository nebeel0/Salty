﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class BlockBehavior : GameBehavior
{
    // Overall Manager Class Instance

    // TODO on merge change camera position
    // TODO no mixing of anti and regular particles, when they clash, annihilation must happen, nvm I was wrong
    // TODO rotate on block place
    public GameObject ParticleRef;
    public int particleAnimationSpeed = 5;

    public QuarkManagerBehavior quarkManager;
    public ElectronManagerBehavior electronManager;
    public SlotManagerBehavior slotManager;
    public ClusterBehavior cluster;

    Rigidbody rigidbody;

    public int ActualNetCharge //Debugging TODO remove
    {
        get { return GetNetCharge(); }
    }

    public bool BeginnerElementFlag;

    public void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        slotManager.Start();
        electronManager.Start();
        quarkManager.Start();

        if (BeginnerElementFlag)
        {
            StartCoroutine(BeginnerElement()); //TODO replace with RandomElement
        }
        if (cluster == null) //TODO move into callback when blocks break links
        {
            cluster = gameMaster.CreateCluster(new HashSet<BlockBehavior>() { this });
        }
    }

    public void Death()
    {
        //TODO callback to cluster
        transform.DetachChildren();
        quarkManager.Death();
        electronManager.Death();
        slotManager.Death();
    }

    void OnCollisionEnter(Collision col)  // TODO Use C# Job System to avoid extra subatomic particles or leptons than possible
    {
        if (col.gameObject.CompareTag("Particle"))
        {
            //Debug.Log("Particle Colliding");
            CollideParticle(col.gameObject);
        }
    }

    public void CollideParticle(GameObject particle)
    {
        if (ParticleUtils.isFermion(particle))
        {
            if (ParticleUtils.isQuark(particle))
            {
                //Debug.Log("Quark Colliding.");
                quarkManager.SetQuark(particle.GetComponent<QuarkBehavior>());
            }
            else if(ParticleUtils.isElectron(particle))
            {
                //Debug.Log("Lepton Colliding.");
                electronManager.SetElectron(particle.GetComponent<ElectronBehavior>());
            }
            else
            {
                Debug.LogError("Something went wrong.");
            }
        }
    }

    // max number of particles is 10, acceptable indices are from 0-1, and a factor 3, min -2
    IEnumerator BeginnerElement()
    {
        yield return new WaitForSeconds(0.2f); //TODO if game master not "online", via a flag, then manually turn online
        for (int i = 0; i < 15; i++)
        {
            QuarkBehavior quarkBehavior;
            if (i % 3 != 0)
            {
                quarkBehavior = gameMaster.CreateQuarkPos(10, 10, 1);
            }
            else
            {
                quarkBehavior = gameMaster.CreateQuarkNeg(10, 10, 1);
            }
            quarkBehavior.transform.parent = transform;
            quarkBehavior.transform.localPosition = Vector3.zero;
            quarkBehavior.Occupy(gameObject);


            if ((i / 4) == 1 && i <= 4)
            {
                LeptonBehavior leptonBehavior = gameMaster.CreateElectron(10, 10, 1);
                leptonBehavior.transform.parent = transform;
                leptonBehavior.transform.localPosition = Vector3.zero;
                leptonBehavior.Occupy(gameObject);
                CollideParticle(leptonBehavior.gameObject);
            }
            CollideParticle(quarkBehavior.gameObject);
        }
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }

    public int GetNetCharge()
    {
        return electronManager.GetNetCharge() + quarkManager.GetNetCharge();
    }

    public bool DeathCheck()
    {
        return quarkManager.DeathCheck();
    }
}
