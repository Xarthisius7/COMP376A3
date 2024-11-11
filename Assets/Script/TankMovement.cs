using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TankMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float acceleration = 2f;
    public float deceleration = 2f;
    private float currentSpeed = 0f;

    public Transform turret;


    public string[] weapons = { "Cannon", "Missiles", "Machine Gun" };
    private int currentWeaponIndex = 0;
    public string currentWeapon = "Cannon";

    public TankFiring tf;
    public UIManager ui;

    void Start()
    {
        currentWeapon = weapons[currentWeaponIndex];
    }

    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        HandleMovement();
        HandleRotation();
        HandleAttack();
        HandleWeaponSwitch();

        HandleTurretRotation();
    }

    void HandleTurretRotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(Vector3.up, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);

            Vector3 direction = turret.position - targetPoint;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            turret.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // 仅在水平轴上旋转炮塔
        }
    }


    void HandleMovement()
    {
        if (Input.GetKey(KeyCode.S))
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, moveSpeed);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            currentSpeed -= acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, -moveSpeed, 0);
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0, moveSpeed);
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += deceleration * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, -moveSpeed, 0);
            }
        }

        Vector3 position = transform.position;
        position.y = 0.015f;
        transform.position = position;

        Quaternion rotation = transform.rotation;
        rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, 0);
        transform.rotation = rotation;

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    void HandleRotation()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    void HandleAttack()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            tf.shootInput();
        }
    }

    void HandleWeaponSwitch()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
            currentWeapon = weapons[currentWeaponIndex];
            tf.currentWeapon = currentWeaponIndex;
            ui.UpdateWeaponDisplay(currentWeaponIndex);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            currentWeaponIndex = (currentWeaponIndex - 1 + weapons.Length) % weapons.Length;
            currentWeapon = weapons[currentWeaponIndex];
            tf.currentWeapon = currentWeaponIndex;
            ui.UpdateWeaponDisplay(currentWeaponIndex);
        }
    }
}
