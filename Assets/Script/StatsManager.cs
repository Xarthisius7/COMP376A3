using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager instance;

    [Header("Player Stats")]
    public float playerMaxHealth;
    public int cannonAmmo;
    public int missileAmmo;
    public int score;

    public float playerCurrentHealth;
    private float scoreInterval = 1f; 
    private float scoreTimer = 0f;

    public UIManager ui;

    private float elapsedTime = 0f;   

    public void ResetStats()
    {
        playerCurrentHealth = playerMaxHealth;

        cannonAmmo = 10;
        missileAmmo = 1;
        score = 0;
        elapsedTime = 0f;


        ui = GameObject.FindGameObjectWithTag("uimanager").GetComponent<UIManager>();


        if (ui != null)
        {
            ui.UpdateHealth(playerCurrentHealth / playerMaxHealth);
            ui.UpdateCannonAmmo(cannonAmmo);
            ui.UpdateMissileAmmo(missileAmmo);
            ui.UpdateScore(score);
            ui.UpdateTime(elapsedTime);
        }



    }



    private void Update()
    {
        if (GManager.instance.gameStarted == false) return;

        elapsedTime += Time.deltaTime;
        scoreTimer += Time.deltaTime;
        if (scoreTimer >= scoreInterval)
        {
            ChangeScore(2); 
            scoreTimer = 0f; 
        }
        if(ui == null)
        {
            ui = GameObject.Find("UIManager").GetComponent<UIManager>();
        }

        ui.UpdateTime(elapsedTime);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        playerCurrentHealth = playerMaxHealth;
    }

    public void ChangeHealth(float amount)
    {
        playerCurrentHealth += amount;
        if(playerCurrentHealth >= playerMaxHealth)
        {
            playerCurrentHealth = playerMaxHealth;
        }

        if (playerCurrentHealth < 0) {
            GManager.instance.OnPlayerDeath();
        }

        if (ui != null)
        {
            ui.UpdateHealth(playerCurrentHealth/playerMaxHealth); 
        }
    }

    public void ChangeCannonAmmo(int amount)
    {
        cannonAmmo += amount;
        if (cannonAmmo < 0) cannonAmmo = 0; 

        ui.UpdateCannonAmmo(cannonAmmo); 
    }

    public void ChangeMissileAmmo(int amount)
    {
        missileAmmo += amount;
        if (missileAmmo < 0) missileAmmo = 0; 

        ui.UpdateMissileAmmo(missileAmmo); 
    }

    public void ChangeScore(int amount)
    {
        score += amount;

        if (ui != null)
        {
            ui.UpdateScore(score); 
        }
    }





}
