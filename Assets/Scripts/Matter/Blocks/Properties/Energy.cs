using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Matter.Block.Property.Base;

namespace Matter
{
    namespace Block
    {
        namespace Property
        {
            public class Energy : BlockProperty<float>
            {
                public LineRenderer lineRenderer;
                float maxDistance = 100;
                float timer = 0;
                float ttl = 0.01f;

                QuantumBlockBehavior quantumBlock
                {
                    get { return GetComponent<QuantumBlockBehavior>(); }
                }
                Vector3 Origin
                {
                    get { return Block.transform.position; }
                }

                Vector3 LaserDirection
                {
                    get { return Block.transform.forward;  }
                }
                public override bool ReadOnly()
                {

                    return quantumBlock != null;
                }

                public override float Get()
                {
                    if(quantumBlock == null)
                    {
                        return base.Get();
                    }
                    else
                    {
                        float energy = 0;
                        foreach(FermionBehavior fermion in quantumBlock.GetFermions())
                        {
                            energy += fermion.energy;
                        }
                        return energy;
                    }
                }


                void Start()
                {
                    if(GetComponent<LineRenderer>() == null)
                    {
                        lineRenderer = Block.gameMaster.spawnManager.AttachLineRenderer(gameObject);
                        lineRenderer.enabled = false;
                    }
                    if (quantumBlock == null)
                    {
                        Set(100);
                    }
                }

                void Update()
                {
                    if(timer > 0)
                    {
                        timer -= Time.deltaTime;
                        if(!lineRenderer.enabled)
                        {
                            lineRenderer.enabled = true;
                        }
                    }
                    else if(lineRenderer.enabled)
                    {
                        lineRenderer.enabled = false;
                        GetComponent<Outline>().enabled = false;
                    }

                    if (lineRenderer.enabled)
                    {
                        LineUpdate();
                        GetComponent<Outline>().enabled = true;
                        GetComponent<Rigidbody>().velocity = Vector3.zero;
                        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    }
                }

                void LineUpdate()
                {
                    Vector3[] points = new Vector3[2];
                    points[0] = Origin;
                    points[1] = GetFurthestPoint();
                    lineRenderer.positionCount = points.Length;
                    lineRenderer.SetPositions(points);
                }

                Vector3 GetFurthestPoint()
                {
                    RaycastHit hit;
                    if (Physics.Raycast(origin: Origin + (Block.transform.localScale.z * LaserDirection), direction: LaserDirection, out hit, layerMask: -1, maxDistance: maxDistance, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
                    {
                        if(BlockUtils.IsBlock(hit.collider.gameObject))
                        {
                            Energy otherEnergy = hit.collider.gameObject.GetComponent<Energy>();
                            otherEnergy.FireLaser();
                        }
                        else
                        {
                            Debug.Log("Hitting a non-block: " + hit.collider.gameObject.name);
                        }
                        return hit.point;
                    }
                    return Origin + LaserDirection * maxDistance;
                }


                public void FireLaser()
                {
                    timer = ttl;
                }

            }
        }
    }
}