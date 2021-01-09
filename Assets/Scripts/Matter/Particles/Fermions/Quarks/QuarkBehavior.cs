using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuarkBehavior : FermionBehavior
{
    public QuarkGroup quarkGroup;

    protected override void Start()
    {
        base.Start();
        gameObject.layer = isFree ? ParticleUtils.quarkLayer : ParticleUtils.noBlockCollisionLayer;
    }

    protected override void OnCollisionEnter(Collision col)
    {
        base.OnCollisionEnter(col);
        //Account for double collision effects for fermions
        if (ParticleUtils.isQuark(col.gameObject)) //Strong force interaction, generate a new block
        {
            //By default if quarks interact they are free
            BlockBehavior newBlock = gameMaster.CreateBlock();
            newBlock.transform.position = transform.position;
            newBlock.CollideParticle(gameObject);
            newBlock.CollideParticle(col.gameObject);
        }
    }

    public void OnChargeChange()
    {
        //TODO implement OnChargeChange
        // Should automatically toggle, and retain knowledge of previous charge, and 
        //quarkGroup.OnChargeChange();
    }

}
