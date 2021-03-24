using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuarkBehavior : FermionBehavior
{
    public QuarkGroup quarkGroup;

    protected override void OnCollisionEnter(Collision col)
    {
        base.OnCollisionEnter(col);
        if(ParticleUtils.isParticle(col.gameObject) && pseudoCollidingParticle == null)
        {
            bool asymmetricalCheck = ParticleUtils.isQuarkPos(gameObject) && ParticleUtils.isQuarkNeg(col.gameObject);
            bool sameMatter = ParticleUtils.IsAntiMatter(col.gameObject) == IsAntiMatter();
            if (asymmetricalCheck && sameMatter) //Strong force interaction, generate a new block
            {
                //By default if quarks interact they are free
                QuantumBlockBehavior newBlock = gameMaster.spawnManager.CreateQuantumBlock();
                newBlock.transform.position = transform.position;
                newBlock.CollideParticle(gameObject);
                newBlock.CollideParticle(col.gameObject);
            }
        }
    }

    public override void Free()
    {
        if (quarkGroup != null)
        {
            quarkGroup.RemoveParticle(this);
        }
        base.Free();
    }

}
