using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Runtime.Serialization.Formatters.Binary;
using Character;

public class PlayerControlManager : GameBehavior
{
    public Controls currentControls;
    Controls ghostControls; //locked
    Controls blockControls; //editable and should be loadable from player's side. For any default, or custom loaded controls. Just needs to go through verification.
    Controls clusterControls; //editable

    public Camera primaryCamera;
    public GameObject gimbal;
    public HUDManager hudManager;

    public ClusterBehavior GetCluster()
    {
        if(transform.parent == null)
        {
            return null;
        }
        else
        {
            return transform.parent.GetComponent<ClusterBehavior>();
        }
    }
    public bool IsAI()
    {
        return GetComponent<AI>() != null;
    }
    public bool IsGhost()
    {
        return GetCluster() == null;
    }
    public bool IsBlock()
    {
        return !IsGhost() && GetCluster().blocks.Count == 1;
    }
    public bool IsCluster()
    {
        return !IsGhost() && GetCluster().blocks.Count > 1;
    }

    public override void Start()
    {
        SetUpCharacter();
        SetUpControllers();
    }

    void SetUpCharacter()
    {
        gameObject.AddComponent<CharacterBehavior>();
    }

    void SetUpControllers()
    {
        ghostControls = ControlsUtil.GhostControls();
        blockControls = ControlsUtil.DefaultBlockControls();
        clusterControls = ControlsUtil.DefaultClusterControls();
        currentControls = ghostControls;
        RefreshControllers();
    }
    void RefreshControllers()
    {
        HashSet<Type> neededSubControllers = new HashSet<Type>();
        foreach (Type subController in currentControls.GetCustomMapping().Values)
        {
            neededSubControllers.Add(subController);
        }
        foreach (Type subController in currentControls.LockedMapping.Values)
        {
            neededSubControllers.Add(subController);
        }

        Controller.SubController[] currentSubControllers = GetComponents<Controller.SubController>();
        for (int i = 0; i < currentSubControllers.Length; i++)
        {
            bool neededSubController = neededSubControllers.Contains(currentSubControllers[i].GetType());
            if (neededSubController)
            {
                neededSubControllers.Remove(currentSubControllers[i].GetType());
            }
            else
            {
                Destroy(currentSubControllers[i]);
            }
        }

        foreach (Type subController in neededSubControllers)
        {
            gameObject.AddComponent(subController);
        }
        if (!IsAI())
        {
            hudManager.RefreshCustomControls();
        }
    }

    void Update()
    {
        bool alive = GetCluster() != null;
        bool dead = GetCluster() == null;
        bool becomeAlive = currentControls == ghostControls && alive;
        bool becomeDead = currentControls != ghostControls && dead;
        bool becomeBlock = currentControls != blockControls && IsBlock();
        bool becomeCluster = currentControls != clusterControls && IsCluster();


        if(becomeDead)
        {
            currentControls = ghostControls;
            RefreshControllers();
        }
        else if (becomeAlive || becomeBlock || becomeCluster)
        {
            currentControls = GetCluster().blocks.Count > 1 ? clusterControls : blockControls;
            RefreshControllers();
        }

        if(!IsAI())
        {
            HUDUpdates();
        }
    }

    void HUDUpdates()
    {
        //TODO make this process more efficient
        if (gameMaster.CurrentGameRules == null && hudManager.gameObject.activeSelf)
        {
            hudManager.gameObject.SetActive(false);
        }
        else if (gameMaster.CurrentGameRules != null && !hudManager.gameObject.activeSelf)
        {
            hudManager.gameObject.SetActive(true);
        }

        //if(hudManager.isActiveAndEnabled)
        //{
        //    hudManager.RefreshPlayerProperties();
        //}
    }

    public void AttachCluster(ClusterBehavior cluster)
    {
        transform.parent = cluster.transform;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }

    public void OnDetachCluster()
    {
        transform.parent = null;
        transform.position = primaryCamera.transform.position;
    }

    void OnScrollSlot(InputValue inputValue)
    {
        OnScrollSlot(inputValue.Get<Vector2>());
    }
    public void OnScrollSlot(Vector2 scrollValue)
    {
        if (currentControls.CustomSlots.Count > 1)
        {
            bool scrollUp = scrollValue.y > 0;
            bool scrollDown = scrollValue.y < 0;
            if (scrollUp)
            {
                if (currentControls.CustomSlotIdx >= currentControls.CustomSlots.Count - 1)
                {
                    currentControls.CustomSlotIdx = 0;
                }
                else
                {
                    currentControls.CustomSlotIdx += 1;
                }
            }
            else if (scrollDown)
            {
                if (currentControls.CustomSlotIdx <= 0)
                {
                    currentControls.CustomSlotIdx = currentControls.CustomSlots.Count - 1;
                }
                else
                {
                    currentControls.CustomSlotIdx -= 1;
                }
            }
            if (scrollUp || scrollDown)
            {
                RefreshControllers();
            }
        }
    }
}
