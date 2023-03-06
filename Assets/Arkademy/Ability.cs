using UnityEngine;

namespace Arkademy
{
    public class Ability : MonoBehaviour
    {
        public Actor user;
        public virtual void Use()
        {
            if (!user)
            {
                Debug.LogWarning("No user for this ability!");
            }
        }
    }
}
