using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

namespace Controller
{
    public class UISubController : SubController
    {
        void OnGamePause()
        {
            Player.gameMaster.menuManager.TogglePauseMenu();
        }

        void OnToggleControlPanel()
        {

        }

    }
}
