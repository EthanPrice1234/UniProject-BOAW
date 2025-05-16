using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
// using UnityEditor.PackageManager;
using UnityEngine.SceneManagement;
public class PlayerStats : CharacterStats
{
    [Header("Health Regeneration Stats")]
    public float healthRegenDelay = 5.0f;
    public float healthRegen = 1.0f;
    public float healthRegenSpeed = 1.0f;
    private float lastDamageTime;
    private float nextHealTime;

    [Header("Defence Stats ")]
    public float damageReduction = 0.0f;
    public float blockingDamageReduction = 0.8f;

    [Header("Damage Stats")]
    public float damageMultiplier = 1.0f;

    [Header("Speed Stats")]
    public float speedMultiplier = 1.0f;

    [Header("Gold Stats")]
    public int gold = 0;
    public float goldMultiplier = 1.0f;
    
    [Header("Damage Particles")]
    public ParticleSystem[] hitParticles;
    public float particleScale = 2f;

    [Header("Level Stats")]
    public int currentXP = 0;
    public int nextLevelXP = 100;
    public int level = 1;
    
    [Header("UI Components")]
    public HealthBar healthbar;
    public TextMeshProUGUI goldText;
    public LevelBar levelBar;
    public TextMeshProUGUI levelText;
    private GameObject deathScreen;
    public TextMeshProUGUI deathText;
    public TextMeshProUGUI bindingBookText;

    [Header("Dead?!")]
    public bool isDead = false; 

    AnimatorManager animatorManager;
    PlayerLocomotion playerLocomotion;
    PlayerCombat playerCombat;
    Animator animator;
    DirectorBindingManager bindingManager;

    void Awake()
    {
        animatorManager = GetComponentInChildren<AnimatorManager>();
        playerLocomotion = GetComponentInChildren<PlayerLocomotion>();
        playerCombat = GetComponentInChildren<PlayerCombat>();
        bindingManager = GameObject.FindGameObjectWithTag("Director").GetComponent<DirectorBindingManager>();
        animator = GetComponent<Animator>();
                
        // Setup the level bar
        levelBar.ResetLevelBar(nextLevelXP);
        levelText.text = $"Lvl. {level}";
        
        deathScreen = GameObject.Find("Death Screen");
        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }
    } 

    void Start()
    {
        SetMaxHealthFromHealthLevel();
        currentHealth = maxHealth;     
        healthbar.SetMaxHealth(maxHealth);

        lastDamageTime = -healthRegenDelay; 

        // Reset gold after each stage 
        gold = 0;
        goldText.text = string.Format("{0:00}", gold);
    }

    void Update()
    {
        // Check if enough time has passed since last damage
        if (Time.time >= lastDamageTime + healthRegenDelay)
        {
            // Check if it's time to heal
            if (Time.time >= nextHealTime)
            {
                SlowRegenHeal();
                nextHealTime = Time.time + 1f;
            }
        }
    }

    public void AddGold(int amount)
    {
        gold += amount * (int)goldMultiplier;
        goldText.text = string.Format("{0:00}", gold);
    }

    public void RemoveGold(int amount)
    {
        gold -= amount;
        goldText.text = string.Format("{0:00}", gold);
    }

    private void SetMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
    }

    private void SlowRegenHeal()
    {
        if (currentHealth < maxHealth && !isDead)
        {
            currentHealth = Mathf.Min(currentHealth + Mathf.RoundToInt(healthRegen * healthRegenSpeed), maxHealth);
            healthbar.SetCurrentHealth(currentHealth);
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healthbar.SetCurrentHealth(currentHealth);
    }

    public void AddXP(int amount)
    {
        currentXP += amount;
        levelBar.SetCurrentXP(currentXP);

        if (currentXP >= nextLevelXP)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        nextLevelXP = (int)(nextLevelXP * 1.1f);
        levelBar.ResetLevelBar(nextLevelXP);
        levelText.text = $"Lvl. {level}";

        // Handle Stats Increase
        healthLevel++;
        SetMaxHealthFromHealthLevel();
        damageReduction += 0.05f;
        damageMultiplier += 0.05f;
        speedMultiplier += 0.05f;
        goldMultiplier += 0.05f;
        healthRegen += 0.05f;
        healthRegenSpeed += 0.05f;
    }

    public void ApplyStatItem(string itemName)
    {
        switch (itemName.ToLower())
        {
            case "shield":
                damageReduction += 0.1f;
                break;
            case "heart":
                maxHealth = (int)(maxHealth * 1.1f);
                healthbar.SetMaxHealth(maxHealth);
                healthbar.SetCurrentHealth(currentHealth);
                break;
            case "skull":
                damageMultiplier = (float)(damageMultiplier * 1.1f);
                break;
            case "lightning":
                speedMultiplier = (float)(speedMultiplier * 1.1f);
                break;
            case "goldpile":
                goldMultiplier = (float)(goldMultiplier * 1.1f);
                break;
            default:
                Debug.LogWarning($"Unknown stat item: {itemName}");
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        if (animator.GetBool("isRolling"))
        {
            bindingManager.successfulRolls++;
            return;
        }

        if (playerCombat.isBlocking)
        {
            bindingManager.successfulBlocks++;
            animator.SetBool("isBlockingHit", true);
            damage -= (int)(damage * blockingDamageReduction);
            currentHealth -= damage;
            Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");

            // Reset heal timer so player doesn't heal while holding block
            lastDamageTime = Time.time;
            nextHealTime = Time.time + 1f; 
            healthbar.SetCurrentHealth(currentHealth);
            return; 
        }

        // Take damage and reset heal timer 
        damage -= (int)(damage * damageReduction);
        currentHealth -= damage;
        lastDamageTime = Time.time;
        nextHealTime = Time.time + 1f; 

        healthbar.SetCurrentHealth(currentHealth);

        animatorManager.PlayWeaponSpecificAnimation("Take Damage", true);

        // Spawn hit particles
        if (hitParticles != null && hitParticles.Length > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 frontPosition = transform.position + transform.forward * 0.5f;
                frontPosition += transform.right * UnityEngine.Random.Range(-0.3f, 0.3f);
                frontPosition += transform.up * UnityEngine.Random.Range(0.5f, 1.2f);
                
                int randomIndex = UnityEngine.Random.Range(0, hitParticles.Length);
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

        if (currentHealth <= 0)
        {
            HandleDeath();
        }

        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");
    }    

    private void HandleDeath()
    {
        isDead = true;
        currentHealth = 0;
        animatorManager.PlayWeaponSpecificAnimation("Death", true);
        playerLocomotion.StopMovement();

        InputManager inputManager = GetComponent<InputManager>();
        if (inputManager != null)
            inputManager.enabled = false;

        deathScreen.SetActive(true);
        Debug.Log(deathText.text);
        deathText.text = $"You have died. Trials Completed {bindingManager.trialsCompleted}.";
        Debug.Log(deathText.text);

        // Binding book story hints!
        if (bindingBookText != null)
        {
            if (bindingManager.trialsCompleted == 0)
            {
                bindingBookText.text = "Truly disappointing. I thought I found the one.";
            }
            else if (bindingManager.trialsCompleted == 1)
            {
                bindingBookText.text = "It's a shame when potential is wasted.";
            }
            else if (bindingManager.trialsCompleted == 2)
            {
                bindingBookText.text = "You're not even close to being good enough.";
            }
            else if (bindingManager.trialsCompleted >= 3)
            {
                bindingBookText.text = "Wait no. You were starting to look like the one.";
            }
        }

        StartCoroutine(ShowDeathScreen());
    }

    private IEnumerator ShowDeathScreen()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("MainMenuScene");
    }
}
