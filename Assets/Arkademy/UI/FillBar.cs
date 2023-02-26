using UnityEngine;
using UnityEngine.UI;

namespace Arkademy.UI
{
    public class FillBar : MonoBehaviour
    {
        public float fill;
        public float delayFillSpeed;
        [SerializeField] private Image realFill;
        [SerializeField] private Image delayedFill;

        // Update is called once per frame
        private void Update()
        {
            realFill.fillAmount = fill;
            delayedFill.fillAmount = Mathf.Lerp(delayedFill.fillAmount, fill, Time.deltaTime * delayFillSpeed);
        }
    }
}