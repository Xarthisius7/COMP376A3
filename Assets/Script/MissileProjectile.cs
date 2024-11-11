using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileProjectile : MonoBehaviour
{
    public float speed = 10f;  
    private Vector3 targetPosition; 
    public float damage = 20f;       
    public float explosionRadius = 5f; 


    public GameObject explosionEffectPrefab; 

    public void Initialize(Vector3 target)
    {
        targetPosition = target;
        StartCoroutine(FlyToTarget());
    }

    private IEnumerator FlyToTarget()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            transform.rotation = Quaternion.LookRotation(direction);

            yield return null;
        }

    }


    private void ShowExplosionEffect()
    {
        if (explosionEffectPrefab != null)
        {
            GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.Euler(-90, 0, 0));


            Destroy(explosionEffect, 0.4f); 
        }
    }



    void OnTriggerEnter(Collider other)
    {
        if (other == null) { return; }
        if (other.CompareTag("Player"))
        {
            return;
        }
        else
        {
            ShowExplosionEffect();
            AudioManager.Instance.PlayClip(5);
            Vector3 explosionCenter = new Vector3(transform.position.x, 0.5f, transform.position.z);


            Collider[] colliders = Physics.OverlapSphere(explosionCenter, explosionRadius);

            HashSet<Enemy> affectedEnemies = new HashSet<Enemy>();

            foreach (Collider collider in colliders)
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null && !affectedEnemies.Contains(enemy))
                {
                    enemy.TakeDamage(damage);
                    affectedEnemies.Add(enemy);
                }
            }

            Destroy(gameObject);
        }
    }

}
