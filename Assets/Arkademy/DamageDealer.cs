using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arkademy
{
    public class DamageDealer : MonoBehaviour
    {
        public Detector usingDetector;
        public List<Actor> direct = new List<Actor>();
        public int damage;
        public float interval;
        public bool immidiate;

        private readonly Dictionary<Actor, float> _colliderTimeStamp = new Dictionary<Actor, float>();
        public void Update()
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
            ProcessDamage(toBeDamaged);
        }

        public void ProcessDamage(List<Actor> toBeDamaged)
        {
            foreach (var candidate in toBeDamaged)
            {
                if (!_colliderTimeStamp.ContainsKey(candidate))
                {
                    _colliderTimeStamp.Add(candidate,Time.time);
                    if (immidiate)
                    {
                        candidate.TakeDamage(damage);
                        return;
                    }
                }

                if (Time.time - _colliderTimeStamp[candidate] > interval)
                {
                    candidate.TakeDamage(damage);
                    _colliderTimeStamp[candidate] = Time.time;
                }
            }
        }
    }
}