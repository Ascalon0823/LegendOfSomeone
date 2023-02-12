using UnityEngine;

namespace Arkademy
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Vector2 offset;
        private void LateUpdate()
        {
            var player = Player.LocalPlayer;
            if (!player) return;
            if (!player.currActor) return;
            transform.position = player.currActor.transform.position + new Vector3(offset.x,offset.y,-10);
        }
    }
}
