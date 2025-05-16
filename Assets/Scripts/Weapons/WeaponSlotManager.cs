using System.Collections;
using System.Collections.Generic;
using EP;
using UnityEngine;

namespace EP{
    public class WeaponSlotManager : MonoBehaviour
    {
        public WeaponHolderSlot leftHandSlot;
        public WeaponHolderSlot rightHandSlot;

        DamageCollider leftHandDamageCollider; 
        DamageCollider rightHandDamageCollider;

        PlayerCombat playerCombat;

        void Awake()
        {
            playerCombat = GetComponentInParent<PlayerCombat>();

            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots)
            {
                if(weaponSlot.isLeftHandSlot)
                {
                    leftHandSlot = weaponSlot;
                }
                else if(weaponSlot.isRightHandSlot)
                {
                    rightHandSlot = weaponSlot; 
                }
            }
        }

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
        {
            if(isLeft)
            {
                leftHandSlot.LoadWeaponModel(weaponItem);
                LoadLeftWeaponDamageCollider();
            }
            else
            {
                rightHandSlot.LoadWeaponModel(weaponItem);
                if (playerCombat.currentEquippedWeapon == WeaponType.Unarmed)
                {
                    // Open no colliders
                }
                else
                {
                    LoadRightWeaponDamageCollider();
                } 
            }
        }
    
        #region Handle Weapons Damage Colliders

        private void LoadLeftWeaponDamageCollider() 
        {
            leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }

        private void LoadRightWeaponDamageCollider()
        {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }
    
        public void OpenRightDamageCollider() 
        {
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void OpenLeftDamageCollider()
        {
            leftHandDamageCollider.EnableDamageCollider();
        }

        public void CloseRightDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider();
        }

        public void CloseLefDamageCollider()
        {
            leftHandDamageCollider.DisableDamageCollider();
        }

        #endregion
    }
}