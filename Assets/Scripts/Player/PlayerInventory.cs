using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EP;
using System.Runtime.CompilerServices;

namespace EP
{
    public class PlayerInventory : MonoBehaviour
    {
        WeaponSlotManager weaponSlotManager;
        PlayerStats playerStats;

        public WeaponItem rightWeapon; 
        public WeaponItem leftWeapon;

        [Header("Weapon References")]
        public WeaponItem greatSword; 
        public WeaponItem sword;
        public WeaponItem shield;
        public WeaponItem woodenSword;

        public List<Item> itemsInventory;

        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            playerStats = GetComponent<PlayerStats>();
        }

        private void Start()
        {
            // Start with fists 
            rightWeapon = null;
            leftWeapon = null;
        } 

        private void SetInventory(List<Item> items)
        {
            itemsInventory = items;
            ApplyAllItems();
        }

        private void ApplyAllItems()
        {
            foreach (Item item in itemsInventory)
            {
                playerStats.ApplyStatItem(item.itemName);
            }
        }
    }
}
