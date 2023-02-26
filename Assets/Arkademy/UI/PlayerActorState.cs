using System;
using UnityEngine;

namespace Arkademy.UI
{
    public class PlayerActorState : MonoBehaviour
    {
        [SerializeField] private FillBar lifeBar;

        private void Update()
        {
            if (!Player.LocalPlayer) return;
            if (!Player.LocalPlayer.currActor) return;
            lifeBar.fill =
                Mathf.Clamp01(Player.LocalPlayer.currActor.currLife * 1.0f / Player.LocalPlayer.currActor.maxLife);
        }
    }
}