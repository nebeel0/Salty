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

    public Dictionary<FixedJoint, QuantumSlotBehavior> connectedJoints = new Dictionary<FixedJoint, QuantumSlotBehavior>();
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
        return ToString().GetHashCode() + electronManager.GetHashCode();
    }
    public override string ToString()
    {
        return position.ToString();
    }

    //Jumping Util

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

        ElectronPosition emptyNeighbor = null;
        for (int i = nextNeighbor; i < 3; i++)
        {
            int neighborId = neighborIdx[i];
            ElectronPosition nextElectronPosition = electronManager.electronPositions[neighborId];
            if (nextElectronPosition.electron is null)
            {
                if(emptyNeighbor == null)
                {
                    emptyNeighbor = nextElectronPosition;
                }
                if (nextElectronPosition.IsEntangled)
                {
                    nextElectronPosition.SetElectron(electron);
                    return;
                }
            }
        }

        if (emptyNeighbor != null && electron != null)
        {
            emptyNeighbor.SetElectron(electron);
        }

    }

    public void Jumping()
    {
        electron.transform.localPosition = Vector3.Lerp(electron.transform.localPosition, position, electronManager.Block.particleAnimationSpeed * Time.deltaTime);
    }

    public void Jump()
    {
        electron.transform.localPosition = position;
    }

    //Add and Remove Utils
    public void Entangle(ElectronPosition otherElectronPosition)
    {
        bool notEntangled = !entangledPositions.Contains(otherElectronPosition);
        bool notThis = otherElectronPosition != this;

        if (notEntangled && notThis)
        {
            entangledPositions.Add(otherElectronPosition);
            entangledPositions.UnionWith(otherElectronPosition.entangledPositions);
            otherElectronPosition.Entangle(this);

            if (electron == null && otherElectronPosition.electron != null)
            {
                SetElectron(otherElectronPosition.electron);
            }
            else if (electron != null)
            {
                otherElectronPosition.SetElectron(electron);
            }

            ElectronPosition[] entangledPositionCopy = new ElectronPosition[entangledPositions.Count];
            entangledPositions.CopyTo(entangledPositionCopy);

            for (int i =0; i < entangledPositionCopy.Length; i++)
            {
                entangledPositionCopy[i].entangledPositions.Add(otherElectronPosition);
                otherElectronPosition.entangledPositions.Add(this);
                otherElectronPosition.entangledPositions.UnionWith(otherElectronPosition.entangledPositions);
                if (entangledPositionCopy[i].electron == null && otherElectronPosition.electron != null)
                {
                    entangledPositionCopy[i].SetElectron(otherElectronPosition.electron);
                }
            }
        }
    }

    public void Untangle(ElectronPosition otherElectronPosition)
    {
        //Caution take into account situations where blocks can disconnect but still be connected spatially via other connections
        if(entangledPositions.Contains(otherElectronPosition) && otherElectronPosition != this)
        {
            entangledPositions.Remove(otherElectronPosition);
            otherElectronPosition.Untangle(this);

            ElectronPosition[] entangledPositionCopy = new ElectronPosition[entangledPositions.Count];
            entangledPositions.CopyTo(entangledPositionCopy);

            for (int i = 0; i < entangledPositionCopy.Length; i++)
            {
                entangledPositionCopy[i].entangledPositions.Remove(otherElectronPosition);
                otherElectronPosition.Untangle(entangledPositionCopy[i]);

                entangledPositionCopy[i].Untangle(otherElectronPosition);
            }

            if (electron != null)
            {
                SetElectron(electron); //First come first serve, since it was untangled, it won't release it for the current position
            }
        }
    }

    public void SetElectron(ElectronBehavior electron) //Automatically releases old position //Real broken
    {
        if(this.electron != null && this.electron != electron)
        {
            Debug.LogError("Cannot add electron to an electron position with an electron");
        }

        if(this.electron == null)
        {
            this.electron = electron;
            ElectronPosition oldPosition = electron.electronPosition;
            electron.electronPosition = this;
            if(oldPosition == null || oldPosition.electronManager != electronManager)
            {
                electron.Occupy(electronManager.gameObject);
            }
            else
            {
                if(!entangledPositions.Contains(oldPosition))
                {
                    oldPosition.ReleaseElectron();
                }
            }
            foreach (ElectronPosition entangledPosition in entangledPositions)
            {
                entangledPosition.electron = electron;
            }
        }
    }

    public void ReleaseElectron()
    {
        if (electron != null)
        {
            ElectronBehavior removedElectron = electron;
            electron = null;
            if (removedElectron.electronPosition == this)
            {
                removedElectron.Free();
                if (entangledPositions.Contains(removedElectron.electronPosition))
                {
                    Debug.LogError("Shouldn't be releasing electron.");
                }
                RemoveAllJoints();
                foreach (ElectronPosition entangledPosition in entangledPositions)
                {
                    entangledPosition.ReleaseElectron();
                }
            }
        }
    }

    //Slot Utils
    public void AddJoint(FixedJoint fixedJoint, QuantumSlotBehavior slot)
    {
        if(!connectedJoints.ContainsKey(fixedJoint))
        {
            connectedJoints[fixedJoint] = slot;
            foreach(ElectronPosition entangledPosition in entangledPositions)
            {
                entangledPosition.AddJoint(fixedJoint, slot);
            }
        }
    }

    public void RemoveJoint(FixedJoint fixedJoint)
    {
        if (connectedJoints.ContainsKey(fixedJoint))
        {
            QuantumSlotBehavior slot = connectedJoints[fixedJoint];
            connectedJoints.Remove(fixedJoint);
            if (slot.GetOccupiedElectronPositions().Count == 1)
            {
                slot.ReleaseBlock();
            }
            foreach (ElectronPosition entangledPosition in entangledPositions)
            {
                entangledPosition.RemoveJoint(fixedJoint);
            }
        }
    }

    public void RemoveAllJoints()
    {
        while(connectedJoints.Count > 0)
        {
            RemoveJoint(connectedJoints.First().Key);
        }
    }

}