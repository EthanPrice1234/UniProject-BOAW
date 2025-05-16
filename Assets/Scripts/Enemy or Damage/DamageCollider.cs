using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    Collider damageCollider;
    public int currentWeaponDamage = 25; // default damage for all colliders 

    private void Awake()
    {
        damageCollider = GetComponent<Collider>(); 
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true; 
        damageCollider.enabled = false;
    }

    public void EnableDamageCollider()
    {
        // Enable collider during attack animation
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider() 
    {
        // Disable collider after attack animation
        damageCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("DamageCollider triggered");
        
        // If hit player damage the player
        if (collision.tag == "Player")
        {
            PlayerStats playerStats = collision.GetComponent<PlayerStats>();

            if (playerStats != null) 
            {
                playerStats.TakeDamage(currentWeaponDamage);
            }
        }

        // If hit enemy damage the enemy
        // This does mean enemies can hurt eachother, but that's fine as I'd like to later add AI to let enemies target different enemy types
        if (collision.tag == "Enemy")
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

            if (enemyStats != null)
            {
                enemyStats.TakeDamage(currentWeaponDamage);
            }
        }
    }
}
