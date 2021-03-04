using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ElectronBehavior : LeptonBehavior
{
    LineRenderer lineRenderer;
    void SetUpLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.endWidth = 0.03f;
        lineRenderer.startWidth = 0.03f;
        lineRenderer.startColor = Color.grey;
        lineRenderer.endColor = Color.grey;
        lineRenderer.material = gameMaster.spawnManager.particleLit;
        lineRenderer.enabled = false;
    }


    void OrbitLineUpdate()
    {
        if(electronPosition != null)
        {
            lineRenderer.enabled = true;
            List<Vector3> points = new List<Vector3>();

            BlockBehavior block = electronPosition.electronManager.Block;

            points.Add(transform.position);

            for (int i = 0; i < electronPosition.neighborIdx.Length; i++)
            {
                int neighborId = electronPosition.neighborIdx[i];
                ElectronPosition entangledPosition = block.electronManager.electronPositions[neighborId];
                Vector3 worldElectronPosition = block.transform.TransformPoint(entangledPosition.position);
                if(entangledPosition.electron != null)
                {
                    points.Add(worldElectronPosition);
                }
            }
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }
        else
        {
            lineRenderer.enabled = false;
        }

    }



    public ElectronPosition electronPosition = null; //Total of 8 blocks 

    public bool IsEntangled()
    {
        if(electronPosition != null)
        {
            return electronPosition.IsEntangled;
        }
        return false;
    }

    public override void Start()
    {
        particleType = ParticleUtils.leptonNeg;
        if (lineRenderer == null)
        {
            SetUpLineRenderer();
        }
        base.Start();
    }

    public override void Free()
    {
        electronPosition = null;
        base.Free();
    }

    protected override void Update()
    {
        base.Update();
        //OrbitLineUpdate();
        if (electronPosition != null)
        {
            Orbit();
        }
    }

    void Orbit()
    {
        if (electronPosition.PositionsNotFull())
        {
            if (Vector3Utils.V3Equal(transform.localPosition, electronPosition.position))
            {
                if (!electronPosition.IsEntangled)
                {
                    electronPosition.AttemptJump();
                }
            }
            else
            {
                electronPosition.Jumping();
            }
        }
        else
        {
            electronPosition.Jump(); //TODO optimize further.
        }
    }

}
