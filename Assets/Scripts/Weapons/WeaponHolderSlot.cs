using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EP;

namespace EP{
    public class WeaponHolderSlot : MonoBehaviour
    {
        public Transform parentOverride;
        public bool isLeftHandSlot;
        public bool isRightHandSlot;

        public GameObject currentWeaponModel; 

        public void UnloadWeapon() 
        {
            if (currentWeaponModel != null)
            {
                currentWeaponModel.SetActive(false);
            }
        }

        public void UnloadWeaponAndDestroy() 
        {
            if (currentWeaponModel != null) 
            {
                Destroy(currentWeaponModel);
            }
        }

        public void LoadWeaponModel(WeaponItem weaponItem)
        {
            // Unload current weapon and destroy it
            UnloadWeaponAndDestroy();

            if(weaponItem == null) 
            {
                Debug.Log("No weapon item to load.");
                return; 
            }

            GameObject model = Instantiate(weaponItem.modelPrefab) as GameObject;
            if(model != null) 
            {
                if (parentOverride != null)
                {
                    model.transform.parent = parentOverride;
                }
                else 
                {
                    model.transform.parent = transform; 
                }

                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one; 
            }

            currentWeaponModel = model; 
        }
    }
}
