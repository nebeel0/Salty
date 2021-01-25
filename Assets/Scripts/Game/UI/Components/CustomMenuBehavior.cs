using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomMenuBehavior : CustomMultiPanelBehavior
{
    // TODO offload transition to another class
    public MenuManager menuManager;
   
    public abstract void PressButton(GameObject button);
}
