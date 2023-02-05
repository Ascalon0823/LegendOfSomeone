using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Arkademy.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private Button campusButton;
        private void Awake()
        {
            gameObject.SetActive(false);
            Sys.OnPause += OnPause;
        }

        private void OnEnable()
        {
            campusButton.gameObject.SetActive(SceneManager.GetActiveScene().name != "Campus");
        }

        private void OnDestroy()
        {
            Sys.OnPause -= OnPause;
        }

        private void OnPause(bool pause)
        {
            gameObject.SetActive(pause);
        }

        public void OnCloseClicked()
        {
            Sys.Pause(false);
        }

        public void OnTitleClicked()
        {
            SceneManager.LoadScene("Scenes/Landing");
        }

        public void OnCampusClicked()
        {
            SceneManager.LoadScene("Scenes/Campus");
        }
    }
}