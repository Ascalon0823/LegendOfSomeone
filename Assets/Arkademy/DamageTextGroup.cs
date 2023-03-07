using System.Collections;
using TMPro;
using UnityEngine;

namespace Arkademy
{
    public class DamageTextGroup : MonoBehaviour
    {
        [SerializeField] private Transform holder;
        [SerializeField] private DamageText damageTextPrefab;
        public float lifeTime;

        public void AddDamage(int damage)
        {
            var text = Instantiate(damageTextPrefab, holder);
            text.tmp.text = damage.ToString();
            text.tmp.transform.localPosition += new Vector3(Random.Range(-5f, 5f), 0f, 0f);
        }
        
        public void AddDamage(int[] damages, float interval)
        {
            StartCoroutine(AddDamageCoroutine(damages, interval));
        }

        private IEnumerator AddDamageCoroutine(int[] damages, float interval)
        {
            foreach (var damage in damages)
            {
                AddDamage(damage);
                yield return new WaitForSeconds(interval);
            }
        }
        
        private void Update()
        {
            if (lifeTime < 0f)
            {
                Destroy(gameObject);
            }

            lifeTime -= Time.deltaTime;
            transform.localPosition += new Vector3(0f, Time.deltaTime, 0f);
        }
    }
}