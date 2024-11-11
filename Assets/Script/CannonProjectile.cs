using UnityEngine;

public class CannonProjectile : MonoBehaviour
{
    public float speed = 20f;     
    public float gravity = 9.8f;      
    public float damage = 50f;       
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
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(damage);


            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
