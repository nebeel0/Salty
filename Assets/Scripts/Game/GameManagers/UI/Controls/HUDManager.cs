using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : GameBehavior
{
    public GameObject CustomControlsPanel;
    public CustomTextBehavior CustomTextRef;

    PlayerControlManager Player
    {
        get { return transform.parent.GetComponent<PlayerControlManager>(); }
    }

    public override void Start()
    {
        base.Start();
    }

    public void RefreshHUD()
    {
        ClearPanel();
        UpdateStateHUD();
        UpdateCustomTraitsHUD();
    }

    void ClearPanel()
    {
        foreach (Transform child in CustomControlsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void UpdateStateHUD()
    {
        CustomTextBehavior stateText = Instantiate(CustomTextRef, CustomControlsPanel.transform).GetComponent<CustomTextBehavior>();
        string state = "";

        if (Player.IsGhost())
        {
            state += "Ghost";
        }
        else if (Player.IsBlock())
        {
            state += "Block";

        }
        else if (Player.IsCluster())
        {
            state += "Cluster";
        }
        else
        {
            state += "Unknown";
        }

        state += Player.currentControls.CustomSlots.Count > 1 ? " [" + Player.currentControls.CustomSlotIdx.ToString() + "]" : "";
        stateText.SetText(state);
    }

    void UpdateCustomTraitsHUD()
    {
        foreach (Type subController in Player.currentControls.GetCustomMapping().Values)
        {
            CustomTextBehavior controlsText = Instantiate(CustomTextRef, CustomControlsPanel.transform).GetComponent<CustomTextBehavior>();
            controlsText.SetText(ControlsUtil.StripSubType(subController));
        }
    }

}
