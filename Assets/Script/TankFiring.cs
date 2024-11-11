using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using System.Reflection;

public class TankFiring : MonoBehaviour
{

    public int currentWeapon = 0;

    [Header("Transforms&Objects")]
    public Transform turretTransform; 
    public Transform tankBodyTransform; 
    public Transform missileLaunchPoint; 
    public Transform cannonLaunchPoint;
    public Transform machineGunLaunchPoint; 

    public GameObject cannonProjectile;
    public GameObject missileProjectile;
    public GameObject machineGunBullet; 

    public GameObject crosshairInstance;

    public UIManager ui;

    [Header("Cannon Settings")]
    public float cannonDamage = 50f;
    public float cannonRange = 100f;
    public float cannonCooldown = 2f;
    private float cannonCooldownTimer = 0f;
    public float cannonSpeed = 20f;  
    public float gravity = 9.8f;        
    public float CannonExistTime = 10f;



    [Header("Missile Settings")]
    public float missileDamage = 30f;
    public float missileExploRadius = 10f;
    public float missileRange = 150f;
    public float missileCooldown = 1f;
    public int missileCount = 4; 
    private float missileCooldownTimer = 0f;
    public float missileInterval = 0.5f;  
    public float MissilesSpeed = 20f;
    public float MissileExistTime = 10f;



    [Header("Machine Gun Settings")]
    public float machineGunSpeed = 40f; 
    public float machineGunDamage = 5f;
    public float machineGunRange = 50f;
    public float machineGunFireRate = 0.1f;
    private float machineGunCooldownTimer = 0f;
    public float MachineGunExistTime = 10f;

    public float machineGunOverheatCooldownTime = 8f;
    public float machineGunOverheatTime = 3f;
    private float machineGunOverheatCounter = 3f;
    private bool machineGunOverheated = false;

    public LineRenderer lineRenderer;
    public Material dashedLineMaterial;







    private void Start()
    {
        machineGunOverheatCounter = machineGunOverheatTime;


        lineRenderer.startWidth = 0.1f; 
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = dashedLineMaterial;
        lineRenderer.textureMode = LineTextureMode.Tile; 
    }


    void Update()
    {
        ui.UpdateMachineGunHeat(1 - (machineGunOverheatCounter / machineGunOverheatTime));
        switch (currentWeapon)
        {
            case 0:
                crosshairInstance.SetActive(true);
                lineRenderer.enabled = false;
                UpdateCannonCrossHair();
                break;

            case 1:
                crosshairInstance.SetActive(true);
                lineRenderer.enabled = false;
                UpdateMissileCrossHair();
                break;

            case 2:
                crosshairInstance.SetActive(false);
                lineRenderer.enabled = true;
                RenderMachineGunCrossHair();
                break;
        }

        CannonPassive();
        MissilePassive();
        MachineGunPassive();

    }


    public void shootInput()
    {

        switch (currentWeapon)
        {
            case 0:
                HandleCannonShooting();
                break;

            case 1:
                HandleMissileShooting();
                break;

            case 2:
                HandleMachineGunShooting();
                break;
        }



    }


    private void CannonPassive()
    {
        cannonCooldownTimer -= Time.deltaTime;
        ui.UpdateCannonCooldown(1 - (cannonCooldownTimer / cannonCooldown));
    }

    private void HandleCannonShooting()
    {
        if (cannonCooldownTimer <= 0f)
        {
            ShootCannon();
            cannonCooldownTimer = cannonCooldown;
        }
    }


    private void MissilePassive()
    {
        missileCooldownTimer -= Time.deltaTime;
        ui.UpdateMissileCooldown(1 - (missileCooldownTimer / missileCooldown));
    }
    private void HandleMissileShooting()
    {
        if (missileCooldownTimer <= 0f)
        {
            StartCoroutine(ShootMissiles());
            missileCooldownTimer = missileCooldown;
        }
    }


    private void MachineGunPassive()
    {

        machineGunCooldownTimer -= Time.deltaTime;

        machineGunOverheatCounter += Time.deltaTime;
        if (!machineGunOverheated && machineGunOverheatCounter < machineGunOverheatTime)
        {
            if (machineGunOverheatCounter > machineGunOverheatTime)
            {
                machineGunOverheatCounter = machineGunOverheatTime;
            }
            ui.UpdateMachineGunHeat(1 - (machineGunOverheatCounter / machineGunOverheatTime));
        }





    }
    private void HandleMachineGunShooting()
    {
        


        machineGunOverheatCounter -= Time.deltaTime;
        ui.UpdateMachineGunHeat(1 - (machineGunOverheatCounter / machineGunOverheatTime));

        if (machineGunCooldownTimer <= 0f && !machineGunOverheated)
        {
            ShootMachineGun();
            machineGunCooldownTimer = machineGunFireRate;
        }

        if (!machineGunOverheated)
        {
            machineGunOverheatCounter -= Time.deltaTime;
            ui.UpdateMachineGunHeat(1 - (machineGunOverheatCounter / machineGunOverheatTime));
            if (machineGunOverheatCounter <= 0f)
            {
                machineGunOverheated = true;

                AudioManager.Instance.PlayClip(15);
                StartCoroutine(CoolDownMachineGun());
            }
        }
    }






    private void UpdateCannonCrossHair()
    {
        //加农炮的  轨迹 准星模拟！
        Vector3 startPosition = cannonLaunchPoint.position;
        Vector3 direction = -cannonLaunchPoint.forward; // 发射方向取反
        Vector3 velocity = direction * cannonSpeed;  // 初始速度

        Vector3 simulatedPosition = startPosition;       // 初始模拟位置
        float simulatedTime = 0f;                        // 模拟时间

        // 模拟抛物线轨迹，找到落地位置
        while (simulatedPosition.y > 0f && simulatedTime < 10f) // 10秒上限防止无限循环
        {
            // 计算当前位置
            simulatedPosition += velocity * Time.deltaTime;
            velocity += Vector3.down * gravity * Time.deltaTime; // 垂直速度受重力影响

            simulatedTime += Time.deltaTime;
        }

        // 落点坐标
        simulatedPosition.y = 0.035f;
        crosshairInstance.transform.position = simulatedPosition;
    }

    private void ShootCannon()
    {
        if (StatsManager.instance.cannonAmmo < 1)
        {
            AudioManager.Instance.PlayClip(21);
        }
        else
        {
            StatsManager.instance.ChangeCannonAmmo(-1);
            AudioManager.Instance.PlayClip(0);

            GameObject projectile = Instantiate(cannonProjectile, cannonLaunchPoint.position, turretTransform.rotation * Quaternion.Euler(0, 180, 0));

            CannonProjectile cannonProjectileScript = projectile.GetComponent<CannonProjectile>();
            if (cannonProjectileScript != null)
            {
                cannonProjectileScript.speed = cannonSpeed;
                cannonProjectileScript.gravity = gravity;
                cannonProjectileScript.damage = 50f;   
            }

            Destroy(projectile, CannonExistTime);
        }


    }





    private Vector3 UpdateMissileCrossHair()
    {
        Vector3 startPosition = cannonLaunchPoint.position;
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        Vector3 direction = (mouseWorldPosition - startPosition).normalized;
        float distanceToMouse = Vector3.Distance(startPosition, mouseWorldPosition);

        Vector3 targetPosition;

        if (distanceToMouse > missileRange)
        {
            targetPosition = startPosition + direction * missileRange;
        }
        else
        {
            targetPosition = mouseWorldPosition;
        }

        targetPosition.y = 0.015f;
        crosshairInstance.transform.position = targetPosition;
        return targetPosition;
    }


    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return ray.GetPoint(100f); 
    }



    private IEnumerator ShootMissiles()
    {

        if (StatsManager.instance.missileAmmo < 1)
        {
            AudioManager.Instance.PlayClip(21);
        }
        else
        {

            StatsManager.instance.ChangeMissileAmmo(-1);
            if (Random.value <= 0.3f) 
            {
                AudioManager.Instance.PlayClip(19);
            }
            for (int i = 0; i < 4; i++) 
            {
                AudioManager.Instance.PlayClip(13);
                Vector3 launchAngle = turretTransform.rotation * Quaternion.Euler(45, 180, 0) * Vector3.forward;
                GameObject missile = Instantiate(missileProjectile, missileLaunchPoint.position, Quaternion.LookRotation(launchAngle));

                Vector3 targetPosition = UpdateMissileCrossHair();

                missile.GetComponent<MissileProjectile>().Initialize(targetPosition);

                missile.GetComponent<MissileProjectile>().damage = missileDamage;
                missile.GetComponent<MissileProjectile>().explosionRadius = missileExploRadius;

                Destroy(missile, MissileExistTime);
                yield return new WaitForSeconds(missileInterval);
            }
        }


    }


















    private void ShootMachineGun()
    {

        AudioManager.Instance.PlayClip(18);

        GameObject projectile = Instantiate(machineGunBullet, machineGunLaunchPoint.position, tankBodyTransform.rotation * Quaternion.Euler(0, 180, 0));

        CannonProjectile cannonProjectileScript = projectile.GetComponent<CannonProjectile>();
        if (cannonProjectileScript != null)
        {
            cannonProjectileScript.speed = machineGunSpeed;
            cannonProjectileScript.gravity = 0; 
            cannonProjectileScript.damage = machineGunDamage; 
        }


        Destroy(projectile,MachineGunExistTime);
    }

    private IEnumerator CoolDownMachineGun()
    {
        yield return new WaitForSeconds(machineGunOverheatCooldownTime); 
        machineGunOverheated = false;
        machineGunOverheatCounter = machineGunOverheatTime; 
        ui.UpdateMachineGunHeat(1 - (machineGunOverheatCounter / machineGunOverheatTime));
    }




    void RenderMachineGunCrossHair()
    {
        lineRenderer.SetPosition(0, machineGunLaunchPoint.position);
        lineRenderer.SetPosition(1, machineGunLaunchPoint.position - machineGunLaunchPoint.forward * machineGunRange);
    }

}
