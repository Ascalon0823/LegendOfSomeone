using System;
using TMPro;
using UnityEngine;

namespace Arkademy.UI
{
    public class TimeDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        private void Update()
        {
            if (Sys.CurrState == null) return;
            text.text = $"{Sys.CurrState.weeks + 1} {Sys.CurrState.days + 1} {Sys.CurrState.hours + 8 : 00}:00";
        }

        public void OnButtonClick()
        {
        }
    }
}