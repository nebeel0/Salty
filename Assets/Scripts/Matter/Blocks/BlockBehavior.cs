using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class BlockBehavior : MonoBehaviour
{
    // Overall Manager Class Instance

    // TODO on merge change camera position
    // TODO no mixing of anti and regular particles, when they clash, annihilation must happen, nvm I was wrong
    // TODO rotate on block place

    //public int NetChargeCache;  // To be used similar to a cache, meaning that while its not the most accurate representation of the current state, it will reduce computations
    //public GameObject ParticleRef;

    //public BlockQuarkManagerBehavior quarkManager;
    //public BlockLeptonManagerBehavior leptonManager;
    public BlockSlotManagerBehavior slotManager;
    public GameObject ClusterRef;

    public ClusterBehavior ParentCluster
    {
        get
        {
            if (transform.parent != null && transform.parent.tag == "Cluster")
            {
                return transform.parent.gameObject.GetComponent<ClusterBehavior>();
            }
            return null;
        }
    }

    void Start()
    {
        //BeginnerElement(); //TODO replace with RandomElement
    }



    void Update()
    {
        if (ParentCluster == null)
        {
            GameObject cluster = Instantiate(ClusterRef);
            transform.SetParent(cluster.transform);
        }
        //RefreshNetCharge();
        //DeathCheck();
    }

    //void DeathCheck()
    //{
    //    if (quarkManager.quarkGroups.Count == 0)
    //    {
    //        while (leptonManager.leptons.Count > 0)
    //        {
    //            leptonManager.RemoveLepton(0);
    //        }
    //        //TODO destroy all block slot connections
    //        //foreach (BlockConnection blockConnection in connectedBlocks.Values)
    //        //{
    //        //    blockConnection.Death();
    //        //}
    //        Destroy(gameObject);  // TODO maybe a death loading animation before hand?
    //    }
    //}

    //void OnCollisionEnter(Collision col)  // TODO Use C# Job System to avoid extra subatomic particles or leptons than possible
    //{
    //    if (col.gameObject.CompareTag("Particle"))
    //    {
    //        CollideParticle(col.gameObject);
    //    }
    //}

    //public void CollideParticle(GameObject particle)
    //{
    //    ParticleBehavior particleBehavior = particle.GetComponent<ParticleBehavior>();
    //    if (particleBehavior.isFermion)
    //    {
    //        if (!quarkManager.AddQuark(particle))
    //        {
    //            leptonManager.AddLepton(particle);
    //        }
    //    }
    //}

    //// max number of particles is 10, acceptable indices are from 0-1, and a factor 3, min -2
    //void BeginnerElement()
    //{
    //    for (int i = 0; i < 15; i++)
    //    {
    //        GameObject particle = Instantiate(ParticleRef);
    //        particle.transform.position = transform.position;
    //        ParticleBehavior particleBehavior = particle.GetComponent<ParticleBehavior>();
    //        if (i % 3 != 0)
    //        {
    //            particleBehavior.particleStateType = "quarkPos";
    //        }
    //        else
    //        {
    //            particleBehavior.particleStateType = "quarkNeg";
    //        }

    //        if ((i / 4) == 1 && i <= 4)
    //        {
    //            GameObject lepton = Instantiate(ParticleRef);
    //            lepton.transform.position = transform.position;
    //            ParticleBehavior leptonBehavior = lepton.GetComponent<ParticleBehavior>();
    //            leptonBehavior.particleStateType = "leptonNeg";
    //            CollideParticle(lepton);
    //        }
    //        CollideParticle(particle);
    //    }
    //}

    //void RandomElement()
    //{
    //    int numberOfParticles = Random.Range(1, 10);
    //    for (int i = 0; i < numberOfParticles; i++)
    //    {
    //        GameObject particle = Instantiate(ParticleRef);
    //        ParticleBehavior particleBehavior = particle.GetComponent<ParticleBehavior>();
    //        particleBehavior.RandomState();
    //        CollideParticle(particle);
    //    }
    //}

    //protected void RefreshNetCharge()
    //{
    //    //TODO cooldown, every 5 seconds,
    //    NetChargeCache = GetNetLeptonCharge() + GetNetQuarkCharge();
    //}


}
