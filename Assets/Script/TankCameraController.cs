using UnityEngine;

public class TankCameraController : MonoBehaviour
{
    public Transform tank; 
    public Vector3 offset = new Vector3(0, 5, -10); 
    public float followSpeed = 5f;
    public float rotationSpeed = 2f; 

    private void LateUpdate()
    {
        if (tank == null)
            return;

        Vector3 targetPosition = tank.TransformPoint(offset);

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.LookRotation(tank.position - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
