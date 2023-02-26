using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arkademy
{
    [Serializable]
    public struct Damage
    {
        public int damage;
        public float invinTime;
        public bool globalInvin;
        public int dealerInstanceID;
    }

    public class DamageDealer : MonoBehaviour
    {
        public Detector usingDetector;
        public List<Actor> direct = new List<Actor>();
        public Damage damage;

        public void FixedUpdate()
        {
            if (Sys.Paused) return;
            var toBeDamaged = new List<Actor>();
            foreach (var c in usingDetector.detected)
            {
                var actor = c.GetComponentInParent<Actor>();
                if (!actor) continue;
                toBeDamaged.Add(actor);
            }

            toBeDamaged.AddRange(direct);
            damage.dealerInstanceID = GetInstanceID();
            foreach (var actor in toBeDamaged)
            {
                actor.TakeDamage(damage);
            }
        }
    }
}