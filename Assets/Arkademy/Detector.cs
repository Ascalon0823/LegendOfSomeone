using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arkademy
{
    public class Detector : MonoBehaviour
    {
        public List<Collider2D> detected = new List<Collider2D>();
        public List<Collider2D> exclude = new List<Collider2D>();
        public LayerMask effectiveLayer;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!effectiveLayer.HasLayer(other.gameObject.layer)) return;
            if (!detected.Contains(other) && !exclude.Contains(other))
            {
                detected.Add(other);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!effectiveLayer.HasLayer(other.gameObject.layer)) return;
            if (!detected.Contains(other) && !exclude.Contains(other))
            {
                detected.Add(other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!effectiveLayer.HasLayer(other.gameObject.layer)) return;
            if (detected.Contains(other))
            {
                detected.Remove(other);
            }
        }
    }
}