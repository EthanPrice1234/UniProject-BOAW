using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EP;

public class PlayerCombat : MonoBehaviour 
{
    PlayerManager playerManager;
    PlayerLocomotion playerLocomotion;
    AnimatorManager animatorManager;
    InputManager inputManager;
    Animator animator;
    WeaponSlotManager weaponSlotManager;
    PlayerStats playerStats;
    DirectorBindingManager bindingManager;

    [Header("Combat Flags")]
    public bool isBlocking;
    private bool isBlockingActive = false; 
    public bool wasBlocking;

    [Header("Weapon System")]
    public WeaponType currentEquippedWeapon = WeaponType.Unarmed;
    public WeaponType boundWeapon;
    public bool isChangingWeapon = false;
    public List<WeaponData> weaponDatabase = new List<WeaponData>();
    public bool isWeaponDrawn = false;
    private bool isDrawingWeapon = false;
    private bool isSheathingWeapon = false;

    [Header("Attack Parameters")]
    public bool canAttack = true;
    public float lightAttackCooldown = 0.2f;
    public float heavyAttackCooldown = 1.2f;

    private void Awake()
    {
        bindingManager = GameObject.FindGameObjectWithTag("Director").GetComponent<DirectorBindingManager>();
        animatorManager = GetComponent<AnimatorManager>();
        playerManager = GetComponent<PlayerManager>(); 
        inputManager = GetComponent<InputManager>();
        animator = GetComponent<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        playerStats = GetComponent<PlayerStats>();
        if (weaponDatabase.Count == 0)
        {
            InitializeDefaultWeapons();
        }
    } 

    public void HandleAllCombat() 
    {
        HandleBlocking();
        UpdateWeaponTypeInAnimator();
    }

    private void HandleBlocking()
    {
        animator.SetBool("isBlocking", isBlocking);

        if (isBlocking && !isBlockingActive && !playerManager.isInteracting)
        {
            isBlockingActive = true;
            animatorManager.PlayWeaponSpecificAnimation("Block", true);
        }
        else if (!isBlocking && isBlockingActive)
        {
            isBlockingActive = false;
        }
    }

    private void UpdateWeaponTypeInAnimator()
    {
        if (!isChangingWeapon) {   
            animator.SetInteger("WeaponTypeID", (int)currentEquippedWeapon);
            animator.SetBool("isWeaponDrawn", isWeaponDrawn);
        }
    }

    public void ToggleWeaponDrawn()
    {
        if (!playerManager.isInteracting && !isDrawingWeapon && !isSheathingWeapon)
        {
            if (isWeaponDrawn)
            {
                isSheathingWeapon = true;
                isChangingWeapon = true;
                animatorManager.PlayWeaponSpecificAnimation("Sheath Weapon", true);
            }
            else
            {
                isDrawingWeapon = true;
                animatorManager.PlayBoundWeaponAnimation("Draw Weapon", true);
                currentEquippedWeapon = boundWeapon;
            }
        }
    }

    // Animation Event: when weapon model should appear
    public void WeaponModelDrawn()
    {
        PlayerInventory inventory = GetComponent<PlayerInventory>();

        if (boundWeapon == WeaponType.GreatSword)
        {
            weaponSlotManager.LoadWeaponOnSlot(inventory.greatSword, false);
        } 
        else if (boundWeapon == WeaponType.SwordAndShield)
        {
            weaponSlotManager.LoadWeaponOnSlot(inventory.sword, false);
            weaponSlotManager.LoadWeaponOnSlot(inventory.shield, true);
            playerStats.blockingDamageReduction = 1f;
        }
        else if (boundWeapon == WeaponType.WoodenSword)
        {
            weaponSlotManager.LoadWeaponOnSlot(inventory.woodenSword, false);
        }
        else {
            Debug.Log("No weapon bound!");
        }
    }

    // Animation Event: when weapon model should disappear
    public void WeaponModelSheathed()
    {        
        weaponSlotManager.LoadWeaponOnSlot(null, false);
        playerStats.blockingDamageReduction = 0.8f;
    }

    // Animation Event: called at end of sheath animation
    public void OnWeaponSheathed()
    {
        isWeaponDrawn = false;
        currentEquippedWeapon = WeaponType.Unarmed;
        isChangingWeapon = false;
        isSheathingWeapon = false;
        animator.SetBool("isInteracting", false);
    }

    // Animation Event: called at end of draw animation
    public void OnWeaponDrawn()
    {
        isChangingWeapon = false;
        isDrawingWeapon = false;
        isWeaponDrawn = true;
        animator.SetBool("isInteracting", false);
    }

    // Switching weapons soley for testing
    public void SwitchWeapon(WeaponType newWeaponType)
    {
        if (currentEquippedWeapon == newWeaponType || isDrawingWeapon || isSheathingWeapon)
            return;

        // If weapon is drawn, sheath it first
        if (isWeaponDrawn)
        {
            isSheathingWeapon = true;
            // Play sheath animation
            animatorManager.PlayTargetAnimation("Sheath Weapon", true);
            isWeaponDrawn = false;
        }

        currentEquippedWeapon = newWeaponType;
    }

    public void HandleLightAttack()
    {
        if (!canAttack || playerManager.isInteracting)
            return;
        
        canAttack = false;
        bindingManager.lightAttacks++;
        animatorManager.PlayWeaponSpecificAnimation("Light Attack", true);
    }

    public void HandleHeavyAttack() 
    {
        if (!canAttack || playerManager.isInteracting)
            return;

        canAttack = false;
        bindingManager.heavyAttacks++;
        animatorManager.PlayWeaponSpecificAnimation("Heavy Attack", true);
    }

    public void ResetCanAttack()
    {
        canAttack = true;
    }

    private void InitializeDefaultWeapons()
    {
        WeaponData unarmed = new WeaponData
        {
            type = WeaponType.Unarmed,
            weaponName = "Unarmed"
        };

        WeaponData greatSword = new WeaponData
        {
            type = WeaponType.GreatSword,
            weaponName = "Greatsword"
        };

        WeaponData swordAndShield = new WeaponData
        {
            type = WeaponType.SwordAndShield,
            weaponName = "Sword And Shield"
        };

        WeaponData woodenSword = new WeaponData
        {
            type = WeaponType.WoodenSword,
            weaponName = "Greatsword"
        };

        weaponDatabase.Add(unarmed);
        weaponDatabase.Add(greatSword);
        weaponDatabase.Add(swordAndShield);
        weaponDatabase.Add(woodenSword);
    }

    public WeaponData GetCurrentWeaponData()
    {
        return weaponDatabase.Find(w => w.type == currentEquippedWeapon);
    }

    public WeaponData GetBoundWeaponData()
    {
        return weaponDatabase.Find(w => w.type == boundWeapon);
    }

    public string GetBoundWeaponName()
    {
        WeaponData weapon = GetBoundWeaponData();
        return weapon != null ? weapon.weaponName : "Unarmed";
    }

    public string GetCurrentWeaponName()
    {
        WeaponData weapon = GetCurrentWeaponData();
        return weapon != null ? weapon.weaponName : "Unarmed";
    }
}