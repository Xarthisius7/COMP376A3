using UnityEngine;

public class PowerUps : MonoBehaviour
{
    // 0 and 1 represent the two types of power-ups
    public int powerUpType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (powerUpType)
            {
                case 0:
                    TriggerPowerUpType0();
                    break;

                case 1:
                    TriggerPowerUpType1();
                    break;
            }
        }
    }

    private void TriggerPowerUpType0()
    {
        StatsManager.instance.ChangeHealth(150);
        AudioManager.Instance.PlayClip(9);
        Destroy(gameObject);
    }

    private void TriggerPowerUpType1()
    {

        StatsManager.instance.ChangeMissileAmmo(2);
        StatsManager.instance.ChangeCannonAmmo(8);
        AudioManager.Instance.PlayClip(2);
        Destroy(gameObject);
    }
}
