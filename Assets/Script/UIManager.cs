using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image healthBar;
    public Image cannonCooldownBar;
    public Image missileCooldownBar;
    public Image machineGunHeatBar;
    public Image ChaseDistanceBar;
    public TextMeshProUGUI cannonAmmoText;
    public TextMeshProUGUI missileAmmoText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public Image currentWeaponIcon;
    public Image crosshair;
    public Color ChaseCloseColor = Color.white;
    public Color machineGunColdColor = Color.blue;
    public Color machineGunHotColor = Color.red;
    public Sprite[] weaponIcons;

    public GameObject GameOverPannel;
    public TextMeshProUGUI GameOverText;

    public float blinkSpeed = 2f; 
    private bool isBlinking = false;

    public void StartBlinking()
    {
        isBlinking = true;
        StartCoroutine(BlinkEffect());
    }

    public void StopBlinking()
    {
        isBlinking = false;
        healthBar.color = new Color(healthBar.color.r, healthBar.color.g, healthBar.color.b, 1);  
    }

    private IEnumerator BlinkEffect()
    {
        while (isBlinking)
        {
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);  
            healthBar.color = new Color(healthBar.color.r, healthBar.color.g, healthBar.color.b, alpha);
            yield return null;
        }
    }





    public void ShowGameOverScreen(int score)
    {
        GameOverPannel.SetActive(true);
        GameOverText.text = score+"";
    }

    private void Start()
    {
        UpdateHealth(1.0f);
        UpdateCannonCooldown(1.0f);
        UpdateMissileCooldown(1.0f);
        UpdateMachineGunHeat(0.0f);
        UpdateWeaponDisplay(0); 
    }

    public void UpdateTime(float elapsedSeconds)
    {
        int minutes = Mathf.FloorToInt(elapsedSeconds / 60);
        int seconds = Mathf.FloorToInt(elapsedSeconds % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateChaseDistance(float value)
    {
        ChaseDistanceBar.fillAmount = value;
        ChaseDistanceBar.color = Color.Lerp(ChaseCloseColor, machineGunHotColor, value);
    }


    public void UpdateScore(int score)
    {
        scoreText.text = "" + score.ToString();
    }


    public void UpdateHealth(float value)
    {
        healthBar.fillAmount = value;

        if(value<0.3)
        {
            StartBlinking();
        }
    }

    public void UpdateCannonCooldown(float value)
    {
        cannonCooldownBar.fillAmount = value;
    }

    public void UpdateMissileCooldown(float value)
    {
        missileCooldownBar.fillAmount = value;
    }

    public void UpdateMachineGunHeat(float value)
    {
        machineGunHeatBar.fillAmount = value;
        machineGunHeatBar.color = Color.Lerp(machineGunColdColor, machineGunHotColor, value);
    }

    public void UpdateWeaponDisplay(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weaponIcons.Length)
        {
            currentWeaponIcon.sprite = weaponIcons[weaponIndex];
        }
    }

    public void UpdateCannonAmmo(int ammo)
    {
        cannonAmmoText.text = ammo+"";
    }

    public void UpdateMissileAmmo(int ammo)
    {
        missileAmmoText.text = ammo + "";
    }

}
