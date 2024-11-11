using UnityEngine;

public class ChasingEnemy : MonoBehaviour
{
    public Transform player;         
    public float speed = 5f;        
    private float currentSpeed;       
    public float attackDistance = 3f; 
    public float damage = 1f;       
    public float speedUpRange = 10f; 

    private float damageCooldown = 0.2f;
    private float damageTimer = 0f; 

    public UIManager ui;

    private void Start()
    {
        currentSpeed = speed;  
    }

    private void Update()
    {

        if (Time.timeScale == 0f)
            return;
        damageTimer += Time.deltaTime;

        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        float distanceToPlayer = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(player.position.x, 0, player.position.z));

        if (distanceToPlayer < attackDistance && damageTimer >= damageCooldown)
        {
            StatsManager.instance.ChangeHealth(-damage); 
            AudioManager.Instance.PlayClip(20); 
            damageTimer = 0f;
        }

        if (distanceToPlayer > speedUpRange)
        {
            currentSpeed = speed * 5;  
        }
        else
        {
            currentSpeed = speed;  
        }

        if (distanceToPlayer > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * DifficultyManager.instance.chaseSpeed * Time.deltaTime);
        }

        float distancePercentage = CalculateDistancePercentage(distanceToPlayer);
        ui.UpdateChaseDistance(distancePercentage);
    }
    private float CalculateDistancePercentage(float distanceToPlayer)
    {
        if (distanceToPlayer >= speedUpRange)
        {
            return 0f; 
        }
        else if (distanceToPlayer <= attackDistance)
        {
            return 1f;
        }
        else
        {
            return 1f - ((distanceToPlayer - attackDistance) / (speedUpRange - attackDistance));
        }
    }

}
