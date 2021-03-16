using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : GameBehavior
{
    //TODO create a HUD "service" where base panels with layout group and vertical element can be created and organized automatically
    // depending on request features
    public GameObject CustomControlsPanel;
    public GameObject PlayerPropertiesPanel;
    public CustomTextBehavior CustomTextRef;

    PlayerControlManager Player
    {
        get { return transform.parent.GetComponent<PlayerControlManager>(); }
    }

    public override void Start()
    {
        base.Start();
    }

    void ClearPanel(GameObject panel)
    {
        foreach (Transform child in panel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void RefreshPlayerProperties()
    {
        ClearPanel(PlayerPropertiesPanel);
        if (Player.GetCluster() != null)
        {
            if (Player.IsBlock())
            {
                Matter.Block.Base.BlockBehavior block = Player.GetCluster().trackingBlock;
                Matter.Block.Base.BlockPropertyManager blockPropertyManager = block.blockPropertyManager;
                bool hasProperties = blockPropertyManager != null && blockPropertyManager.properties != null;
                if(hasProperties)
                {
                    List<string> readableProperties = blockPropertyManager.GetReadableProperties();
                    foreach (string readableProperty in readableProperties)
                    {
                        CustomTextBehavior propertyText = Instantiate(CustomTextRef, PlayerPropertiesPanel.transform).GetComponent<CustomTextBehavior>();
                        propertyText.SetText(readableProperty);
                    }
                }
            }
        }
    }

    public void RefreshCustomControls()
    {
        ClearPanel(CustomControlsPanel);

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

        foreach (Type subController in Player.currentControls.GetCustomMapping().Values)
        {
            CustomTextBehavior controlsText = Instantiate(CustomTextRef, CustomControlsPanel.transform).GetComponent<CustomTextBehavior>();
            controlsText.SetText(ControlsUtil.StripSubType(subController));
        }
    }

}
