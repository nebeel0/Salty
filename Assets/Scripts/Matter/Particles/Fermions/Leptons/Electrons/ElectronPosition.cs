using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ElectronPosition : IEquatable<ElectronPosition>
{
    public ElectronManagerBehavior electronManager;
    public ElectronBehavior electron;
    public Vector3 position;
    public int[] neighborIdx;
    public int id;
    public int nextNeighbor = 0;


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

    public bool PositionsNotFull()
    {
        return !electronManager.IsFull();
    }

    public void AttemptJump()
    {
        nextNeighbor += 1;
        if (nextNeighbor >= 3)
        {
            nextNeighbor = 0;
        }

        //Checks one by one which neighbor might be empty, and if so returns that position
        for (int i = nextNeighbor; i < 3; i++)
        {
            int neighborId = neighborIdx[i];
            ElectronPosition nextElectronPosition = electronManager.electronPositions[neighborId];
            if (nextElectronPosition.electron is null)
            {
                nextElectronPosition.SetElectron(electron);
                return;
            }
        }

    }

    public void Jumping()
    {
        electron.transform.localPosition = Vector3.Lerp(electron.transform.localPosition, position, electronManager.block.particleAnimationSpeed * Time.deltaTime);
    }

    public void Jump()
    {
        electron.transform.localPosition = position;
    }

    //Add and Remove Utils
    public void Entangle(ElectronPosition otherElectronPosition)
    {
        if(!entangledPositions.Contains(otherElectronPosition))
        {
            if (otherElectronPosition.electron != null && electron != null && otherElectronPosition.electron != electron)
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
                SetElectron(electron); //First come first serve, since it was untangled, it won't release it for the current position
            }
        }
    }

    public void SetElectron(ElectronBehavior electron) //Automatically releases old position
    {
        this.electron = electron;
        ElectronPosition oldPosition = electron.electronPosition;
        if (oldPosition != this && !entangledPositions.Contains(oldPosition))
        {
            electron.electronPosition = this;
            electron.Occupy(electronManager.gameObject);

            if(oldPosition != null)
            {
                oldPosition.ReleaseElectron();
            }

            foreach (ElectronPosition entangledPosition in entangledPositions)
            {
                entangledPosition.SetElectron(electron);
            }
        }
    }

    public void ReleaseElectron()
    {
        if (electron != null)
        {
            if (electron.electronPosition == this)
            {
                electron.Free();
            }
            electron = null;
        }
    }

    //Block Utils

    public void ConnectBlock(BlockBehavior block)
    {
        if (electron != null)
        {
            Jump();
            electron.ConnectBlock(block);
        }
    }

}