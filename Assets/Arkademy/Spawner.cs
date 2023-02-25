using System;
using UnityEngine;

namespace Arkademy
{
    public class Spawner : MonoBehaviour
    {
        public GameObject[] toBeSpawned;
        public float interval;
        public bool immidiate;
        [SerializeField] private float lastSpawn;

        private void OnEnable()
        {
            lastSpawn = Time.time;
            if (!immidiate) return;
            Spawn();
        }

        private void Update()
        {
            if (Time.time - lastSpawn > interval)
            {
                Spawn();
            }
        }

        public void Spawn()
        {
            if (toBeSpawned == null) return;
            foreach (var toBeSpawn in toBeSpawned)
            {
                Instantiate(toBeSpawn, transform.position, transform.rotation);
            }

            lastSpawn = Time.time;
        }
    }
}