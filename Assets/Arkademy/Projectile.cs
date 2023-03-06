using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Arkademy
{
    public class Projectile : MonoBehaviour
    {
        public float lifeTime;
        public float speed;
        public Transform target;
        public Vector2 targetPos;
        [Range(0,180)]
        public int turnSpeed;
        public float radius;
        public LayerMask effectiveLayer;
        public List<Collider2D> ignores;

        public Func<Projectile, RaycastHit2D,bool> OnHitShouldStop;

        [SerializeField] private readonly RaycastHit2D[] _castResults = new RaycastHit2D[100];

        private void FixedUpdate()
        {
            if (Sys.Paused) return;
            lifeTime -= Time.fixedDeltaTime;

            if (lifeTime <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            if (target)
            {
                targetPos = target.position;
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                Quaternion.LookRotation(Vector3.forward, (targetPos - (Vector2) transform.position).normalized),
                turnSpeed);


            var dir = transform.up;
            var pos = transform.position;
            var nextDisplacement = speed * Time.fixedDeltaTime * dir;
            var hitCount = Physics2D.CircleCastNonAlloc(pos, Mathf.Max(radius, 0.01f), dir, _castResults,
                nextDisplacement.magnitude,
                effectiveLayer);
            if (hitCount != 0)
            {
                var hits = _castResults.Where(x => x && (ignores == null || !ignores.Contains(x.collider)))
                    .OrderBy(x => x.distance);
                foreach (var hit in hits)
                {
                    transform.position = hit.point + hit.normal * radius;
                    Debug.Log(hit.collider, hit.collider);
                    if (OnHitShouldStop?.Invoke(this, hit) ?? false) return;
                }
            }
            transform.position = pos + nextDisplacement;
        }
    }
}