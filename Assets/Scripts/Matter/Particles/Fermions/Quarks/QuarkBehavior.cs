using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuarkBehavior : FermionBehavior
{
    public QuarkGroup quarkGroup;

    protected override void OnCollisionEnter(Collision col)
    {
        base.OnCollisionEnter(col);
        if (ParticleUtils.isQuarkPos(gameObject) && ParticleUtils.isQuarkNeg(gameObject) && col.collider.enabled && particleCollider.enabled) //Strong force interaction, generate a new block
        {
            //By default if quarks interact they are free
            BlockBehavior newBlock = gameMaster.CreateBlock();
            newBlock.transform.position = transform.position;
            newBlock.CollideParticle(gameObject);
            newBlock.CollideParticle(col.gameObject);
            if (newBlock.DeathCheck())
            {
                Debug.LogError("This should never happen.");
                newBlock.Death();
            }
        }
    }

    public void OnChargeChange()
    {
        //TODO implement OnChargeChange
        // Should automatically toggle, and retain knowledge of previous charge, and 
        //quarkGroup.OnChargeChange();
    }

}
