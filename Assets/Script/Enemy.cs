using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { Tower = 1, TankTrap = 2, Mortar = 3, EnemyTank = 4 }
    public EnemyType enemyType;

    public float health;
    public float maxHealth;
    public float damage;
    public float attackRange;
    public float attackCooldown = 2f;   
    private float attackCooldownTimer = 0f; 

    public GameObject healthBarPrefab;
    private Image healthBarFill;
    private Transform player;

    private bool AttackBuffed = false;

    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;  


    public GameObject projectilePrefab;      
    public Transform projectileSpawnPoint;    

    public GameObject DieEffect;

    public float enemyStrengh = 1.0f; 

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SetupEnemy();
        AttachHealthBar();

    }

    void SetupEnemy()
    {
        switch (enemyType)
        {
            case EnemyType.Tower:
                maxHealth = 10f* enemyStrengh;
                health = maxHealth;
                damage = 0f;
                attackRange = 10f; 
                break;
            case EnemyType.TankTrap:
                maxHealth = 60f * enemyStrengh;
                health = maxHealth;
                damage = 100f * enemyStrengh;
                attackRange = 0f; 
                break;
            case EnemyType.Mortar:
                maxHealth = 30f * enemyStrengh;
                health = maxHealth;
                damage = 60f * enemyStrengh;
                attackRange = 10f;
                attackCooldown = 6f;
                break;

            case EnemyType.EnemyTank:
                maxHealth = 40f * enemyStrengh;
                health = maxHealth;
                damage = 60f * enemyStrengh;
                attackRange = 6f;
                attackCooldown = 7f;
                break;
        }
    }

    void AttachHealthBar()
    {
        GameObject healthBar = Instantiate(healthBarPrefab, transform.position, Quaternion.Euler(-90, 0, 0), transform);

        healthBarFill = healthBar.transform.Find("Health").GetComponent<Image>();
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        Debug.Log("尝试更改生命值！");
        Debug.Log("尝试更改生命值！"+ health / maxHealth);
        
        healthBarFill.fillAmount = health / maxHealth;
        healthBarFill.color = Color.Lerp(Color.red, Color.green, health / maxHealth);

    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        AudioManager.Instance.PlayClip(3);
        UpdateHealthBar();

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {

        AudioManager.Instance.PlayClip(6);
        StatsManager.instance.ChangeScore(+5);
        if (enemyType == EnemyType.EnemyTank)
        {
            StatsManager.instance.ChangeScore(+15);
        }
        else if (enemyType == EnemyType.Tower)
        {
            StatsManager.instance.ChangeScore(+20);
        }

        Destroy(gameObject);
    }

    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }


        switch (enemyType)
        {
            case EnemyType.Tower:
                BoostNearbyEnemies();
                break;
            case EnemyType.Mortar:
                if (IsPlayerInRange())
                {

                    RotateTowardsPlayer();
                    if (attackCooldownTimer <= 0f && IsPlayerInRange())
                    {
                        ShootProjectile();
                        attackCooldownTimer = attackCooldown; 
                    }
                }

                break;
            case EnemyType.EnemyTank:


                if (IsPlayerInRange())
                {

                    RotateTowardsPlayer();
                    if (attackCooldownTimer <= 0f && IsPlayerInRange())
                    {
                        ShootPlayer();
                        attackCooldownTimer = attackCooldown; 
                    }
                }




                break;
        }
    }

    void RotateTowardsPlayer()
    {
        Vector3 directionToPlayer = (transform.position - player.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }


    void BoostNearbyEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider nearbyObject in colliders)
        {
            Enemy enemy = nearbyObject.GetComponent<Enemy>();
            if (enemy != null && enemy != this && enemy.enemyType != EnemyType.Tower)
            {
                enemy.AttackBuffed = true;
                enemy.attackCooldown = 4f;
            }
        }
    }

    void ShootProjectile()
    {
        AudioManager.Instance.PlayClip(12);
        GameObject projectileObject = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);

        EnemyProjectile projectile = projectileObject.GetComponent<EnemyProjectile>();


        projectile.damage = AttackBuffed ? damage : damage * 2.5f;

        projectile.Initialize(player.position);

        Destroy(projectileObject,2.8f);



    }
    
    void ShootPlayer()
    {
        AudioManager.Instance.PlayClip(1);
        GameObject bulletObject = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

        EnemyBullet bullet = bulletObject.GetComponent<EnemyBullet>();

        bullet.damage = AttackBuffed ? damage: damage*2.5f ;

        Vector3 directionToPlayer = (player.position - bulletSpawnPoint.position).normalized;

        bulletObject.transform.forward = directionToPlayer;

        Destroy(bulletObject, 1.5f);

    }

    bool IsPlayerInRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        return distanceToPlayer <= attackRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enemyType == EnemyType.TankTrap && other.CompareTag("Player"))
        {

            AudioManager.Instance.PlayClip(17);

            StatsManager.instance.ChangeHealth(-damage);
            Destroy(gameObject);
        }
    }


}
