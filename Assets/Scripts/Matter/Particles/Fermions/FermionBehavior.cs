using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FermionBehavior : ParticleBehavior
{
    ChargeManagerBehavior ChargeManager
    {
        get
        {
            if(transform.parent == null)
            {
                return null;
            }
            else
            {
                return transform.parent.gameObject.GetComponent<ChargeManagerBehavior>();
            }
        }
    }
    public bool isChargeChanging = false;

    protected override void Update()
    {
        base.Update();
        if(isChargeChanging)
        {
            ColorUpdate();
        }
    }

    public virtual void Free()
    {
        if(transform.parent != null || !particleCollider.enabled)
        {
            transform.SetParent(null);
            gameObject.layer = layer;
            particleCollider.enabled = true;
            rigidbody.drag = 0.25f;
            rigidbody.angularDrag = 0.25f;
            rigidbody.isKinematic = false;
        }
    }

    public virtual void Occupy(ChargeManagerBehavior chargeManager)
    {
        if((chargeManager != null && transform.parent != chargeManager.transform) || particleCollider.enabled)
        {
            transform.SetParent(chargeManager.transform);
            particleCollider.enabled = false;
            rigidbody.velocity = new Vector3(0, 0, 0);
            rigidbody.drag = 1;
            rigidbody.angularDrag = 1;
            rigidbody.isKinematic = true;
            gameObject.layer = BlockUtils.noBlockCollisionLayer;
        }
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
