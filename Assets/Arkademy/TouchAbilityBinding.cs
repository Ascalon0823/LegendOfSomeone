using System;
using UnityEngine;

namespace Arkademy
{
    public class TouchAbilityBinding : MonoBehaviour
    {
        public PlayerTouchInput.InputState state;
        public Ability ability;

        private void Update()
        {
            if (Sys.Paused) return;
            if (!ability) return;
            if (!Player.LocalPlayer) return;
            if (!Player.LocalPlayer.playerTouchInput) return;
            if (Player.LocalPlayer.playerTouchInput.CurrInputState != state) return;
            ability.Use();
        }
    }
}