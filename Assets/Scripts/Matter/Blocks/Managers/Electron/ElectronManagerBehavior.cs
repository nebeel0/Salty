using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectronManagerBehavior : BlockManagerBehavior
{
    public Dictionary<string, ElectronPosition> electronPositionDictionary = new Dictionary<string, ElectronPosition>();
    public ElectronPosition[] electronPositions; //Used for determining neighbors
    public int electronsMax = 8;  //TODO programmatically figure out max number of electrons, which we can figure out from number of vertexes in shape
                                  // Start is called before the first frame update

    LineRenderer lineRenderer;
    void SetUpLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.endWidth = 0.03f;
        lineRenderer.startWidth = 0.03f;
        lineRenderer.startColor = Color.grey;
        lineRenderer.endColor = Color.grey;
        lineRenderer.material = gameMaster.particleLit;
        lineRenderer.enabled = false;
    }

    void LineUpdate()
    {
        lineRenderer.enabled = true;
        List<Vector3> points = new List<Vector3>();
        for(int i = 0; i < electronPositions.Length; i++)
        {
            if(electronPositions[i].IsEntangled)
            {
                points.Add(block.transform.TransformPoint(electronPositions[i].position));
            }
        }
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    public bool IsNetPositiveOrNeutral(ElectronBehavior electron)
    {
        return electron.effectiveCharge + block.GetNetCharge() >= 0;
    }

    public bool HasSpace()
    {
        return GetElectronCount() <= electronsMax - 1;
    }

    public bool IsFull()
    {
        return GetElectronCount() == electronsMax;
    }

    public int GetNetCharge()
    {
        return GetElectronCount() * -1;
    }

    public int GetElectronCount()
    {
        int electronCount = 0;
        for (int i = 0; i < electronPositions.Length; i++)  //Max 8 iterations
        {
            if (electronPositions[i].electron != null)
            {
                electronCount++;
            }
        }
        return electronCount;
    }

    public ElectronPosition GetAvailablePosition()
    {
        for(int i=0; i < electronPositions.Length; i++)
        {
            if(electronPositions[i].electron == null)
            {
                return electronPositions[i];
            }
        }
        return null;
    }

    public void Start()
    {
        if(electronPositions == null)
        {
            SetUpElectronPositions();
        }
        if(lineRenderer == null)
        {
            SetUpLineRenderer();
        }
    }

    void Update()
    {
        LineUpdate();
    }

    void SetUpElectronPositions()
    {
        //TODO cache
        electronPositions = new ElectronPosition[electronsMax];
        for (int i = 0; i < 2; i++)
        {
            float currX = Mathf.Pow(-1, i) * 0.5f * transform.localScale.x * block.slotManager.displacementFactor;
            for (int ii = 0; ii < 2; ii++)
            {
                float currY = Mathf.Pow(-1, ii) * 0.5f * transform.localScale.y * block.slotManager.displacementFactor;
                for (int iii = 0; iii < 2; iii++)
                {
                    float currZ = Mathf.Pow(-1, iii) * 0.5f * transform.localScale.z * block.slotManager.displacementFactor;
                    Vector3 currPos = new Vector3(currX, currY, currZ);
                    ElectronPosition electronPosition = new ElectronPosition
                    {
                        position = currPos,
                        neighborIdx = new int[] { Vector3Utils.Vector3ToIdx(1 - i, ii, iii), Vector3Utils.Vector3ToIdx(i, 1 - ii, iii), Vector3Utils.Vector3ToIdx(i, ii, 1 - iii) },
                    };
                    electronPositions[Vector3Utils.Vector3ToIdx(i,ii,iii)] = electronPosition;
                    electronPositionDictionary[currPos.ToString()] = electronPosition;
                    electronPosition.electronManager = this;
                }
            }
        }
    }

    public void SetElectron(ElectronBehavior electron)
    {
        bool isElectron = electron.antiCharge == 1;
        //Debug.Log(isElectron);
        //Debug.Log(IsNetPositiveOrNeutral(electron));
        //Debug.Log(HasSpace());

        if (isElectron && IsNetPositiveOrNeutral(electron) && HasSpace())
        {
            ElectronPosition openElectronPosition = GetAvailablePosition();
            openElectronPosition.SetElectron(electron);
        }
        else
        {
            electron.Free();
        }
    }

    public void ReleaseElectron(ElectronPosition electronPosition)
    {
        electronPosition.ReleaseElectron();
    }

    public ElectronBehavior GetUnconnectedElectron()
    {
        for(int i=0; i< electronPositions.Length; i++)
        {
            if(electronPositions[i].electron != null && !electronPositions[i].electron.isLocked)
            {
                return electronPositions[i].electron;
            }
        }
        return null;
    }

    public void Death()
    {
        for(int i=0; i<electronPositions.Length; i++)
        {
            electronPositions[i].ReleaseElectron();
        }
        Destroy(this);
    }

}
