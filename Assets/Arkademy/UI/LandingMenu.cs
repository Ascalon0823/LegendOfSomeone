using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arkademy.UI
{
    public class LandingMenu : MonoBehaviour
    {
        public void OnStartButtonClick()
        {
            if (Sys.HasSave())
            {
                Sys.Continue();
                return;
            }
            Sys.NewGame();
        }
    }
}
