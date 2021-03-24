using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Matter.Block.Base;

public class QuantumBlockBehavior : BlockBehavior
{
    // Overall Manager Class Instance

    // TODO on merge change camera position
    // TODO no mixing of anti and regular particles, when they clash, annihilation must happen, nvm I was wrong
    // TODO rotate on block place
    public int particleAnimationSpeed = 5;
    public QuarkManagerBehavior quarkManager;
    public ElectronManagerBehavior electronManager;
    public QuantumSlotManagerBehavior slotManager;
    static float annihilationCooldownTime = 0.5f;
    float annihilationCooldownTimer = annihilationCooldownTime;

    public override SlotManagerBehavior GetSlotManager()
    {
        return slotManager;
    }

    public bool BeginnerElementFlag;

    public override void Start()
    {
        base.Start();
        if (BeginnerElementFlag)
        {
            StartCoroutine(BeginnerElement()); //TODO replace with RandomElement
            BeginnerElementFlag = false;
        }
    }

    protected override void SetUpManagers()
    {
        if (slotManager == null)
        {
            slotManager = new GameObject().AddComponent<QuantumSlotManagerBehavior>();
            Vector3Utils.NeutralParent(parent: transform, child: slotManager.transform);
        }
        if (electronManager == null)
        {
            electronManager = new GameObject().AddComponent<ElectronManagerBehavior>();
            Vector3Utils.NeutralParent(parent: transform, child: electronManager.transform);
        }
        if (quarkManager == null)
        {
            quarkManager = new GameObject().AddComponent<QuarkManagerBehavior>();
            Vector3Utils.NeutralParent(parent: transform, child: quarkManager.transform);
        }
        slotManager.Start();
        electronManager.Start();
        quarkManager.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (DeathCheck())
        {
            Death();
        }
    }


    public override void Death()
    {
        if(quarkManager != null)
        {
            quarkManager.Death();
        }
        electronManager.Death();
        slotManager.Death();
        base.Death();
    }

    protected void OnCollisionEnter(Collision col)  // TODO Use C# Job System to avoid extra subatomic particles or leptons than possible
    {
        if (ParticleUtils.isParticle(col.gameObject))
        {
            CollideParticle(col.gameObject);
        }
    }

    protected void OnCollisionStay(Collision col)
    {
        if(BlockUtils.IsQuantumBlock(col.gameObject))
        {
            QuantumBlockBehavior otherQuantumBlock = col.gameObject.GetComponent<QuantumBlockBehavior>();
            bool antiMatterCheck = otherQuantumBlock.antiMatterFlag != antiMatterFlag;
            bool aysmmetricalCheck = antiMatterFlag;
            bool cooldown = annihilationCooldownTimer < 0;
            annihilationCooldownTimer -= Time.deltaTime;
            if (antiMatterCheck && aysmmetricalCheck && cooldown)
            {
                annihilationCooldownTimer = annihilationCooldownTime;
                List<FermionBehavior> otherFermions = otherQuantumBlock.GetFermions();
                foreach (FermionBehavior fermion in otherFermions)
                {
                    if(AnnihilateFermion(fermion.gameObject))
                    {
                        return;
                    }
                    
                }
            }
        }
    }

    public void CollideParticle(GameObject particle)
    {
        if (ParticleUtils.isFermion(particle))
        {
            if(ParticleUtils.IsAntiMatter(particle) == antiMatterFlag)
            {
                if (ParticleUtils.isQuark(particle))
                {
                    quarkManager.SetQuark(particle.GetComponent<QuarkBehavior>());
                }
                else if (ParticleUtils.isElectron(particle))
                {
                    electronManager.SetElectron(particle.GetComponent<ElectronBehavior>());
                }
            }
            else
            {
                AnnihilateFermion(particle);
            }
        }
    }

    protected bool AnnihilateFermion(GameObject particle)
    {
        FermionBehavior fermion = particle.GetComponent<FermionBehavior>();
        if (ParticleUtils.isElectron(particle))
        {
            if(electronManager.Annihilate(fermion))
            {
                return true;
            }
        }
        else if (ParticleUtils.isQuark(particle))
        {
            if(quarkManager.Annihilate(fermion))
            {
                return true;
            }
        }
        return false;
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
                quarkBehavior = gameMaster.spawnManager.CreateQuarkPos(weightClass: 10, energy: 10, antiMatterFlag: antiMatterFlag);
            }
            else
            {
                quarkBehavior = gameMaster.spawnManager.CreateQuarkNeg(weightClass: 10, energy: 10, antiMatterFlag: antiMatterFlag);
            }
            quarkBehavior.transform.parent = transform;
            quarkBehavior.transform.localPosition = Vector3.zero;


            if ((i / 4) == 1 && i <= 4)
            {
                LeptonBehavior leptonBehavior = gameMaster.spawnManager.CreateElectron(weightClass: 10, energy: 10, antiMatterFlag: antiMatterFlag);
                leptonBehavior.transform.parent = transform;
                leptonBehavior.transform.localPosition = Vector3.zero;
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

    public int GetActualNetCharge()
    {
        return antiMatterFlag ? GetNetCharge() * -1 : GetNetCharge();
    }

    public List<FermionBehavior> GetFermions()
    {
        List<FermionBehavior> fermions = new List<FermionBehavior>();
        fermions.AddRange(electronManager.GetFermions());
        fermions.AddRange(quarkManager.GetFermions());
        return fermions;
    }

    public override bool DeathCheck()
    {
        return quarkManager != null && quarkManager.DeathCheck();
    }
}
