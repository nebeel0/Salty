using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FermionBehavior : ParticleBehavior
{
    public int spinFactor = 1;
    public bool isChargeChanging = false;

    protected override void Update()
    {
        base.Update();
        if(isChargeChanging)
        {
            ColorUpdate();
        }
    }

    protected virtual void OnCollisionEnter(Collision col)
    {
        //TODO account for missing collisions when in blocks
        //Account for double collision effects for fermions
        if (ParticleUtils.isParticle(col.gameObject))
        {
            ParticleBehavior otherParticle = col.gameObject.GetComponent<ParticleBehavior>();
            if (ParticleUtils.areSameType(gameObject, col.gameObject) && otherParticle.antiCharge != antiCharge)
            {
                Annihilate(otherParticle);
            }
        }
    }

    public virtual void Free()
    {
        transform.SetParent(null);
        particleCollider.enabled = true;
        rigidbody.drag = 0.25f;
        rigidbody.angularDrag = 0.25f;
        rigidbody.isKinematic = false;
        gameObject.layer = layer;
    }

    public virtual void Occupy(GameObject block)
    {
        transform.SetParent(block.transform);
        particleCollider.enabled = false;
        rigidbody.velocity = new Vector3(0, 0, 0);
        rigidbody.drag = 1;
        rigidbody.angularDrag = 1;
        rigidbody.isKinematic = true;
        gameObject.layer = BlockUtils.noBlockCollisionLayer;
    }

    public void OnChargeChange()
    {
        isChargeChanging = true;
    }

    protected void ColorUpdate()
    {
        if (ColorUtils.ApproxEqual(GetComponent<Renderer>().material.color, particleColor, 0.01f))
        {
            SetParticleColor();
            isChargeChanging = false;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.Lerp(GetComponent<Renderer>().material.color, particleColor, Time.deltaTime);
            GetComponent<TrailRenderer>().startColor = Color.Lerp(GetComponent<Renderer>().material.color, particleColor, Time.deltaTime);
        }
    }
}
