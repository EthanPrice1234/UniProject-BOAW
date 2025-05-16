using UnityEngine;

public enum WeaponType
{
    Unarmed = 0,
    WoodenSword = 1, 
    GreatSword = 2,
    SwordAndShield = 3,
    Axe = 4
}

[System.Serializable]
public class WeaponData
{
    public WeaponType type;
    public string weaponName;
    public float lightAttackDamage;
    public float heavyAttackDamage;
} 