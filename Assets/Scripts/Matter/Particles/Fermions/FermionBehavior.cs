using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FermionBehavior : ParticleBehavior
{
    public bool isFree = true;
    public bool isChargeChanging = false;

    protected override void Start()
    {
        base.Start();
        if (isFree)
        {
            Free();
        }
    }

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
        //Account for double collision effects for fermions
        DisableCollision();
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
        isFree = true;
        transform.SetParent(null);
        EnableCollisionCooldownTimer();
        rigidbody.drag = 0.25f;
        rigidbody.angularDrag = 0.25f;
        rigidbody.isKinematic = false;
        gameObject.layer = layer; //TODO make a better layer
    }

    public virtual void Occupy(GameObject block)
    {
        isFree = false;
        transform.SetParent(block.transform);
        DisableCollisionCooldownTimer();
        DisableCollision();
        rigidbody.velocity = new Vector3(0, 0, 0);
        rigidbody.drag = 1;
        rigidbody.angularDrag = 1;
        rigidbody.isKinematic = true;
        gameObject.layer = ParticleUtils.noBlockCollisionLayer;
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
