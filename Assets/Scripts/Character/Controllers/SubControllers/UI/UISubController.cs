using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

namespace Controller
{
    public class UISubController : SubController
    {
        protected bool gamePause = false;

        protected void OnGamePause()
        {
            if (enabled && Player.gameMaster.CurrentGameRules != null)
            {
                Player.gameMaster.menuManager.TogglePauseMenu();
            }
        }

    }
}
