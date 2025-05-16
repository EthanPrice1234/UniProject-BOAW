using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectorBindingManager : MonoBehaviour
{
    [Header("Game Stats")]
    public int trialsCompleted = 0;
    public int successfulRolls = 0;
    public int successfulBlocks = 0;
    public int lightAttacks = 0;
    public int heavyAttacks = 0;

    PlayerCombat playerCombat;

    private void Awake()
    {
        playerCombat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCombat>();
    }   

    public void HandleBinding()
    {
        if (trialsCompleted == 1)   
        {
            // Handle Weapon Binding
            // Really simple formula for bindings, as this is a prototype and a full system is out of scope
            // So a concept system in place, either the player will be bound a greatsword or a sword and shield. 
            // This is based on the number of successful rolls and blocks. 

            if (successfulRolls >= successfulBlocks)
            {
                playerCombat.boundWeapon = WeaponType.GreatSword;
            }
            else {
                playerCombat.boundWeapon = WeaponType.SwordAndShield;
            }
        }
        else {
            Debug.Log("Weapon Already Bound!");
        }
    }
}
