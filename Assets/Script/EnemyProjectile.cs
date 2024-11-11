using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 6f;  // Missile speed
    private Vector3 targetPosition;  // Target location
    public float damage = 20f;      

    public void Initialize(Vector3 target)
    {
        targetPosition = target;
        StartCoroutine(FlyToTarget());
    }

    // Coroutine to make the missile fly toward the target
    private IEnumerator FlyToTarget()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Calculate the direction to the target
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            // Update rotation to face the target
            transform.rotation = Quaternion.LookRotation(direction);

            yield return null;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StatsManager.instance.ChangeHealth(-damage);

            AudioManager.Instance.PlayClip(7);
            Destroy(gameObject);
            return;
        } 
    }

}
