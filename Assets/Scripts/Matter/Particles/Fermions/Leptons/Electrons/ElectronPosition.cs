using UnityEngine;
using System;
using System.Collections.Generic;

public class ElectronPosition : IEquatable<ElectronPosition>
{
    public ElectronManagerBehavior electronManager;
    public ElectronBehavior electron;
    public Vector3 position;
    public int[] neighborIdx;
    public int id;

    public HashSet<ElectronPosition> entangledPositions = new HashSet<ElectronPosition>();
    public bool IsEntangled
    {
        get { return entangledPositions.Count > 0; }
    }
    public bool Equals(ElectronPosition electronPosition)
    {
        return electronPosition.ToString() == ToString();
    }
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }
    public override string ToString()
    {
        return position.ToString();
    }

    //Jumping Utils
    public bool IsLocked()
    {
        return electron.isLocked;
    }

    public bool CanJump()
    {
        return electronManager.IsFull();
    }

    public void AttemptJump()
    {
        //Checks one by one which neighbor might be empty, and if so returns that position
        for (int i = 0; i < 3; i++)
        {
            int neighborId = neighborIdx[i];
            ElectronPosition nextElectronPosition = electronManager.electronPositions[neighborId];
            if (nextElectronPosition.electron is null)
            {
                nextElectronPosition.SetElectron(electron);
                ReleaseElectron();
                return;
            }
        }
    }

    public void Jumping()
    {
        electron.transform.localPosition = Vector3.Lerp(electron.transform.localPosition, position, electronManager.block.particleAnimationSpeed * Time.deltaTime);
    }

    //Add and Remove Utils
    public void Entangle(ElectronPosition otherElectronPosition)
    {
        if(!entangledPositions.Contains(otherElectronPosition))
        {
            if (otherElectronPosition.electron != null && otherElectronPosition.electron != null)
            {
                Debug.LogError("Electron positions can't both be entangled if they both have electrons.");
            }

            entangledPositions.Add(otherElectronPosition);
            otherElectronPosition.entangledPositions.Add(this);

            if (electron != null)
            {
                otherElectronPosition.SetElectron(electron);
            }
            else if (otherElectronPosition.electron != null)
            {
                SetElectron(otherElectronPosition.electron);
            }
        }
    }

    public void Untangle(ElectronPosition otherElectronPosition)
    {
        //Caution take into account situations where blocks can disconnect but still be connected spatially via other connections
        if(entangledPositions.Contains(otherElectronPosition))
        {
            entangledPositions.Remove(otherElectronPosition);
            otherElectronPosition.entangledPositions.Remove(this);

            if (electron != null)
            {
                otherElectronPosition.ReleaseElectron(); //First come first serve, since it was untangled, it won't release it for the current position
            }
        }
    }

    public void SetElectron(ElectronBehavior electron)
    {
        if(!electron.electronPositions.Contains(this))
        {
            electron.Occupy(electronManager.gameObject);
            electron.electronPositions.Add(this);
            this.electron = electron;

            foreach(ElectronPosition entangledPosition in entangledPositions)
            {
                entangledPosition.SetElectron(electron);
            }
        }
    }

    public void ReleaseElectron()
    {
        if (electron != null)
        {
            electron.electronPositions.Remove(this);
            if (electron.transform.parent == electronManager.transform)
            {
                if (electron.electronPositions.Count > 0)
                {
                    electron.transform.parent = electron.GetRootElectronPosition().electronManager.transform;
                }
                else
                {
                    electron.Free();
                }
            }
            foreach (ElectronPosition entangledPosition in entangledPositions)
            {
                entangledPosition.ReleaseElectron();
            }
            electron = null;
        }
    }

    //Block Utils

    public void ConnectBlock(BlockBehavior block)
    {
        if (electron != null)
        {
            electron.ConnectBlock(block);
        }
    }

}