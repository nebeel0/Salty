using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ElectronManagerBehavior : QuantumBlockManagerBehavior
{
    public Dictionary<string, ElectronPosition> electronPositionDictionary = new Dictionary<string, ElectronPosition>();
    public ElectronPosition[] electronPositions; //Used for determining neighbors
    public int electronsMax = 8;  //TODO programmatically figure out max number of electrons, which we can figure out from number of vertexes in shape
                                  // Start is called before the first frame update

    public bool IsNetPositiveOrNeutral(ElectronBehavior electron)
    {
        return electron.effectiveCharge + Block.GetNetCharge() >= 0;
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
        gameObject.name = "Electron Manager";
        if (electronPositions == null)
        {
            SetUpElectronPositions();
        }
    }

    void SetUpElectronPositions()
    {
        //TODO cache
        electronPositions = new ElectronPosition[electronsMax];
        for (int i = 0; i < 2; i++)
        {
            float currX = Mathf.Pow(-1, i) * 0.5f * transform.localScale.x * Block.slotManager.displacementFactor;
            for (int ii = 0; ii < 2; ii++)
            {
                float currY = Mathf.Pow(-1, ii) * 0.5f * transform.localScale.y * Block.slotManager.displacementFactor;
                for (int iii = 0; iii < 2; iii++)
                {
                    float currZ = Mathf.Pow(-1, iii) * 0.5f * transform.localScale.z * Block.slotManager.displacementFactor;
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
            if(electronPositions[i].electron != null && !electronPositions[i].electron.IsEntangled())
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
