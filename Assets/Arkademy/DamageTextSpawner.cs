using System;
using UnityEngine;

namespace Arkademy
{
    public class DamageTextSpawner : MonoBehaviour
    {
        public DamageTextGroup groupPrefab;
        public float interval;

        private void Start()
        {
            var actor = GetComponentInParent<Actor>();
            if (!actor) return;
            actor.OnTookDamage += OnTookDamage;
        }

        private void OnTookDamage(int damage)
        {
            var group = Instantiate(groupPrefab, transform);
            group.AddDamage(new[] {damage, damage, damage}, interval);
        }
    }
}