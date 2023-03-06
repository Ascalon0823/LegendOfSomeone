using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Arkademy
{
    public class Actor : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Detector detector;
        [SerializeField] private GameObject graphicRoot;
        [SerializeField] private Transform facingRoot;
        [SerializeField] private Collider2D hitBox;

        #region Movement

        [Header("Locomotion")] public Vector2 wantToMove;
        public float speed;

        private void Move()
        {
            rb.MovePosition(rb.position + speed * Time.deltaTime * wantToMove);
        }

        #endregion

        #region Targeting

        [Header("Targeting")] public GameObject currTarget;
        [Header("Facing")] public Vector2 facing;
        [SerializeField] private float originalFaceLeft;

        private void FindTarget()
        {
            var nearest = detector.detected.Where(x=>x.GetComponentInParent<Actor>()).OrderBy(x => Vector2.Distance(x.transform.position, transform.position))
                .FirstOrDefault();
            currTarget = nearest ? nearest.gameObject : null;
        }

        private void Face()
        {
            facing = currTarget
                ? (Vector2) (currTarget.transform.position - transform.position).normalized
                : wantToMove.magnitude > 0f
                    ? wantToMove.normalized
                    : facing;
        }

        #endregion


        private void Start()
        {
            originalFaceLeft = Mathf.Sign(facingRoot.localScale.x);
        }

        private void FixedUpdate()
        {
            if (Sys.Paused) return;
            if (killed) return;
            Move();
            FindTarget();
            Face();
            UpdateInvincibility();
        }

        private void LateUpdate()
        {
            if (Sys.Paused) return;
            if (killed)
            {
                timeBeforeDestroy -= Time.deltaTime;
                if (timeBeforeDestroy < 0f)
                {
                    Destroy(gameObject);
                }

                return;
            }

            //Default is left
            facingRoot.localScale =
                new Vector3(Vector2.Dot(facing, Vector2.left) < 0 ? -originalFaceLeft : originalFaceLeft, 1f, 1f);
        }

        #region Killable

        [Header("Life and Damage")] public int maxLife;
        public int currLife;

        public float invinTime;
        public readonly Dictionary<int, float> IndividualInvinTime = new Dictionary<int, float>();
        public bool killed;
        public float timeBeforeDestroy;

        public UnityAction OnKilled;
        public Collider2D HitBox => hitBox;

        public void TakeDamage(int damage)
        {
            if (Sys.Paused) return;
            currLife -= damage;
            currLife = Mathf.Max(0, currLife);
            if (currLife == 0)
            {
                Kill();
            }
        }

        public void TakeDamage(Damage damage)
        {
            if (damage.globalInvin && CanBeDamaged())
            {
                TakeDamage(damage.damage);
                invinTime = damage.invinTime;
                return;
            }

            if (!damage.globalInvin && CanBeDamagedBy(damage.dealerInstanceID))
            {
                TakeDamage(damage.damage);
                IndividualInvinTime[damage.dealerInstanceID] = damage.invinTime;
            }
        }

        public void Kill()
        {
            killed = true;
            OnKilled?.Invoke();
        }

        public bool CanBeDamagedBy(int dealerID)
        {
            return CanBeDamaged() && !IndividualInvinTime.ContainsKey(dealerID);
        }

        public bool CanBeDamaged()
        {
            return invinTime <= 0f;
        }

        private void UpdateInvincibility()
        {
            if (invinTime > 0f)
            {
                invinTime -= Time.fixedDeltaTime;
            }

            foreach (var id in IndividualInvinTime.Keys.ToList())
            {
                IndividualInvinTime[id] -= Time.fixedDeltaTime;
                if (IndividualInvinTime[id] <= 0f)
                {
                    IndividualInvinTime.Remove(id);
                }
            }
        }

        #endregion
    }
}