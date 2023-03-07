using System;
using TMPro;
using UnityEngine;

namespace Arkademy
{
    public class DamageText : MonoBehaviour
    {
        public TextMeshProUGUI tmp;

        private void Update()
        {
            tmp.color = new Color(tmp.color.r,tmp.color.g,tmp.color.b,tmp.color.a-Time.deltaTime);
        }
    }
}