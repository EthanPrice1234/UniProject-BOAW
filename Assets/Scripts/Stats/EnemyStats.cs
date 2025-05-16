using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStats : CharacterStats
{
    [Header("Damage Particles")]
    public ParticleSystem[] hitParticles;
    public float particleScale = 2f; 

    [Header("Death Effect")]
    public float waitBeforeSink = 6f;     
    public float sinkDuration = 8f;       
    public float sinkDistance = 2f;

    [Header("Damage Stats")]
    public int damage = 20; 

    [Header("Drops Stats")]
    public int gold = 5;
    public int minXP = 10;
    public int maxXP = 20;

    public PlayerStats playerStats;

    Animator animator;
    EnemyController enemyController;
    Rigidbody enemyRigidbody;

    // damage handling params
    public bool isTakingDamage = false;
    private bool isDead = false;
    private float animCooldown = 0f;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        enemyController = GetComponent<EnemyController>();
        enemyRigidbody = GetComponent<Rigidbody>();
        playerStats = FindAnyObjectByType<PlayerStats>();
    }

    private void Update()
    {
        if (animCooldown > 0f)
        {
            animCooldown -= Time.deltaTime;
        }
    }

    void Start()
    {
        // Set max health and give a random amount of gold
        maxHealth = SetMaxHealthFromHealthLevel();
        gold = Random.Range(1, 15);
        currentHealth = maxHealth;     
    }

    private int SetMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    public void TakeDamage(int damage)
    {
        // Prevents spamming damage
        if (isTakingDamage) 
            return; 
        
        if (animCooldown <= 0f) {
            animator.Play("Take Damage");
            animCooldown = 4f;
            isTakingDamage = true;
        }
        
        currentHealth -= damage;

        // Lets enemy attack although it's taking damage, to prevent spamming damage
        if (hitParticles != null && hitParticles.Length > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 frontPosition = transform.position + transform.forward * 0.5f;
                frontPosition += transform.right * Random.Range(-0.3f, 0.3f);
                frontPosition += transform.up * Random.Range(0.5f, 1.2f);
                
                int randomIndex = Random.Range(0, hitParticles.Length);
                ParticleSystem selectedEffect = hitParticles[randomIndex];
                
                if (selectedEffect != null)
                {
                    ParticleSystem spawnedEffect = Instantiate(selectedEffect, frontPosition, Quaternion.identity);
                    spawnedEffect.transform.forward = transform.forward;
                    spawnedEffect.transform.localScale *= particleScale;
                    Destroy(spawnedEffect.gameObject, selectedEffect.main.duration);
                }
            }
        }

        // Enemy is dead so give xp and gold and sink the body
        if (currentHealth <= 0 && !isDead)
        {
            currentHealth = 0;
            animator.SetBool("isDead", true);
            playerStats.AddGold(gold);
            int xpDropped = Random.Range(minXP, maxXP + 1);
            playerStats.AddXP(xpDropped);

            enemyController.enabled = false;
            enemyRigidbody.isKinematic = true;
            enemyRigidbody.detectCollisions = false;

            isDead = true;
            StartCoroutine(SinkBody());
        }
    }   

    private IEnumerator SinkBody()
    {
        yield return new WaitForSeconds(waitBeforeSink);

        float sinkTimer = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + Vector3.down * sinkDistance;

        // After some time sink the enemy body into the ground
        while (sinkTimer < sinkDuration)
        {
            sinkTimer += Time.deltaTime;
            float sinkProgress = sinkTimer / sinkDuration;
            transform.position = Vector3.Lerp(startPosition, endPosition, sinkProgress);
            yield return null;
        }

        Destroy(gameObject);
    }

    public void ResetTakeDamage()
    {
        animator.SetBool("takeDamage", false);
        isTakingDamage = false;
    }
}
