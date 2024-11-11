using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 9f;
    public float gravity = 0.0f;
    public float damage = 20f;
    private Vector3 velocity;
    private Vector3 startPosition;

    public float existTime = 10f;

    void Start()
    {
        startPosition = transform.position;
        velocity = transform.forward * speed; 
    }

    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        velocity += Vector3.down * gravity * Time.deltaTime; 
        transform.position += velocity * Time.deltaTime;    

        if (transform.position.y <= 0f)
        {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z); 
            Destroy(gameObject); 
        }

    }
    void OnTriggerEnter(Collider other)
    {
         if (other.CompareTag("Player"))
        {
            StatsManager.instance.ChangeHealth(-damage);
            Destroy(gameObject);

            return;
        }
        else if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);

        }
    }
}
