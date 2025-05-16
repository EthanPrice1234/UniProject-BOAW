using UnityEngine;

public class EnemyWeaponManager : MonoBehaviour
{
    [Header("Weapon Slots")]
    public GameObject rightHandWeapon;  
    public GameObject leftHandWeapon;   

    [Header("Damage Colliders")]
    public DamageCollider rightHandDamageCollider;
    public DamageCollider leftHandDamageCollider;

    [Header("Slam Attack")]
    public GameObject slamZonePrefab;  
    public Transform slamPoint;        

    private void Awake()
    {
        if (rightHandWeapon != null)
        {
            rightHandDamageCollider = rightHandWeapon.GetComponentInChildren<DamageCollider>();
        }
        if (leftHandWeapon != null)
        {
            leftHandDamageCollider = leftHandWeapon.GetComponentInChildren<DamageCollider>();
        }
    }

    public void EnableRightHandDamageCollider()
    {
        if (rightHandDamageCollider != null)
        {
            rightHandDamageCollider.EnableDamageCollider();
        }
    }

    public void DisableRightHandDamageCollider()
    {
        if (rightHandDamageCollider != null)
        {
            rightHandDamageCollider.DisableDamageCollider();
        }
    }

    public void EnableLeftHandDamageCollider()
    {
        if (leftHandDamageCollider != null)
        {
            leftHandDamageCollider.EnableDamageCollider();
        }
    }

    public void DisableLeftHandDamageCollider()
    {
        if (leftHandDamageCollider != null)
        {
            leftHandDamageCollider.DisableDamageCollider();
        }
    }

    public void SetWeaponDamage(int damage)
    {
        if (rightHandDamageCollider != null)
        {
            rightHandDamageCollider.currentWeaponDamage = damage;
        }
        if (leftHandDamageCollider != null)
        {
            leftHandDamageCollider.currentWeaponDamage = damage;
        }
    }

    public void GolemSlamAttack()
    {
        // Golem specific attack that creates a slam zone with a damage collider
        if (slamZonePrefab != null && slamPoint != null)
        {
            GameObject slamZone = Instantiate(slamZonePrefab, slamPoint.position, Quaternion.identity);
            
            GolemSlamZone slamZoneComponent = slamZone.GetComponent<GolemSlamZone>();
            if (slamZoneComponent != null)
            {
                EnemyStats enemyStats = GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    slamZoneComponent.damage = enemyStats.damage;
                }
            }
        }
    }
} 