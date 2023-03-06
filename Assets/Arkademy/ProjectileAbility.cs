using System.Collections.Generic;
using UnityEngine;

namespace Arkademy
{
    public class ProjectileAbility : Ability
    {
        public Projectile spawnPrefab;

        public Vector2 posDeviation;
        public int rotDeviation;
        public List<Collider2D> extraIgnores = new List<Collider2D>();

        public Damage damage;

        public override void Use()
        {
            base.Use();
            var projectile = Instantiate(spawnPrefab, user.transform.position, Quaternion.identity);
            projectile.OnHitShouldStop += (projectile1, hit2D) =>
            {
                projectile1.lifeTime = 0f;
                var actor = hit2D.collider.GetComponentInParent<Actor>();
                damage.dealerInstanceID = user.GetInstanceID();
                if (actor)
                {
                    actor.TakeDamage(damage);
                }

                return true;
            };
            projectile.transform.up = user.facing;
            projectile.ignores ??= new List<Collider2D>();
            projectile.ignores.AddRange(extraIgnores);
            projectile.ignores.Add(user.HitBox);
            if (user.currTarget)
            {
                projectile.target = user.currTarget.transform;
            }
            projectile.transform.position +=
                Random.Range(-posDeviation.x, posDeviation.x) * projectile.transform.right
                + Random.Range(-posDeviation.y, posDeviation.y) * projectile.transform.up;
            projectile.transform.eulerAngles += new Vector3(0f, 0f, Random.Range(-rotDeviation, rotDeviation));
        }
    }
}