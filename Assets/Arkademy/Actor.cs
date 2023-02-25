using UnityEngine;

namespace Arkademy
{
    public class Actor : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;

        #region Movement

        [Header("Locomotion")] public Vector2 wantToMove;
        public float speed;

        private void Move()
        {
            rb.MovePosition(rb.position + speed * Time.deltaTime * wantToMove);
        }

        #endregion

        private void FixedUpdate()
        {
            Move();
        }

        public void TakeDamage(int damage)
        {
            Debug.Log(damage);
        }
    }
}