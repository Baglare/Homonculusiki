using UnityEngine;


public enum WeaponKind { Tabanca, Mizrak, Asa, Yumruk, Balta }

[CreateAssetMenu(menuName = "Fight/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public WeaponKind weaponKind;
    public AttackTypeData light;
    public AttackTypeData heavy;

    [Header("Projectile")]
    public bool isProjectileLight;
    public bool isProjectileHeavy;
}
