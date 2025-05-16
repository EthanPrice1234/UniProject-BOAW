using UnityEngine;

public class GolemSlamZone : MonoBehaviour
{
    public int damage = 35;
    public float duration = 0.5f;  
    public float shakeDuration = 3f;
    public float shakeIntensity = 0.1f;

    private void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                // Damage the player and make screen shake
                playerStats.TakeDamage(damage);
                ScreenShake.Shake(shakeDuration, shakeIntensity);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
} 