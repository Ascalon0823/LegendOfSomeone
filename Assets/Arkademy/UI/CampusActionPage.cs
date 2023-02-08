using UnityEngine;

namespace Arkademy.UI
{
    public class CampusActionPage : MonoBehaviour
    {
        [SerializeField] private CampusActionButton actionButtonPrefab;

        [SerializeField] private RectTransform buttonsHolder;

        private void Start()
        {
            foreach (var item in Sys.CampusActions.Values)
            {
                Instantiate(actionButtonPrefab, buttonsHolder).AssignAction(item);
            }
        }
    }
}