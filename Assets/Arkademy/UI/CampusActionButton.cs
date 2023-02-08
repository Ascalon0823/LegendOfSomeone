using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Arkademy.UI
{
    public class CampusActionButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Button button;

        public void AssignAction(Sys.CampusAction action)
        {
            _campusActionAssigned = action;
            text.text = action.displayName;
        }

        private Sys.CampusAction _campusActionAssigned;

        public void OnButtonClick()
        {
            if (_campusActionAssigned == null) return;
            if (!_campusActionAssigned.CanPerform()) return;
            _campusActionAssigned?.OnPerform();
        }

        private void Update()
        {
            if (_campusActionAssigned == null) return;
            button.interactable = _campusActionAssigned.CanPerform();
        }
    }
}