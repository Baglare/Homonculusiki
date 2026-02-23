using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackController : MonoBehaviour
{
    [Header("Refs")]
    public PlayerController2D move;
    public Hitbox2D meleeHitbox;
    public BoxCollider2D meleeCollider;
    public ParryState parry;
    public HealthKnockback hk;

    [Header("Weapon")]
    public WeaponData weapon;

    [Header("Projectile")]
    public Transform muzzle;
    public GameObject bulletPrefab;

    bool busy;

    void Awake()
    {
        if (move == null) move = GetComponent<PlayerController2D>();
        if (parry == null) parry = GetComponent<ParryState>();
        if (hk == null) hk = GetComponent<HealthKnockback>();

        // En sağlamı: inspector'dan bağla. Yine de fallback kalsın:
        if (meleeHitbox == null) meleeHitbox = GetComponentInChildren<Hitbox2D>(true);
        if (meleeCollider == null && meleeHitbox != null) meleeCollider = meleeHitbox.GetComponent<BoxCollider2D>();

        if (meleeHitbox != null)
        {
            meleeHitbox.owner = hk;
            meleeHitbox.ownerTransform = transform;
        }

        if (meleeCollider != null) meleeCollider.enabled = false;
    }

    public void OnHeavyAttack(InputValue v)
    {
        if (!v.isPressed || busy) return;
        if (weapon == null || weapon.heavy == null) return;

        if (weapon.isProjectileHeavy) StartCoroutine(DoProjectile(weapon.heavy, true));
        else StartCoroutine(DoMelee(weapon.heavy, true));
    }

    public void OnLightAttack(InputValue v)
    {
        if (!v.isPressed || busy) return;
        if (weapon == null || weapon.light == null) return;

        if (weapon.isProjectileLight) StartCoroutine(DoProjectile(weapon.light, false));
        else StartCoroutine(DoMelee(weapon.light, false));
    }

    public void OnParry(InputValue v)
    {
        if (!v.isPressed) return;
        if (parry != null) parry.TryParry();
    }

    IEnumerator DoMelee(AttackTypeData data, bool isHeavy)
    {
        busy = true;

        Vector2 aim = (move != null) ? move.Aim : new Vector2(1, 0);
        int facing = aim.x >= 0 ? 1 : -1;
        bool neutral = (move == null) || (move.Aim.sqrMagnitude < 0.01f);

        if (move != null)
        {
            move.facingLock = true;
            move.lockedFacing = facing;
        }

        if (meleeHitbox != null)
        {
            meleeHitbox.damage = data.damage;
            meleeHitbox.knockForce = data.baseForce;
            meleeHitbox.neutralStrike = neutral;
            meleeHitbox.isHeavy = isHeavy;

            // Rotasyon yapildigi icin vector'u her zaman yone gore duz offset olarak belirle:
            // Sola donukse bile 180 derece doneceginden hitbox sol tarafa duser. (facing ile carpmaya gerek yok)
            Vector2 offset = new Vector2(data.hitboxOffset.x, data.hitboxOffset.y);
            meleeHitbox.ApplyMeleeShape(data.hitboxSize, offset);

            // 8-directional rotation based on aim
            float angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
            meleeHitbox.transform.rotation = Quaternion.Euler(0, 0, angle);

            meleeHitbox.knockDir = aim.normalized;
        }

        yield return new WaitForSeconds(data.startup);

        if (meleeHitbox != null) meleeHitbox.BeginSwing();
        if (meleeCollider != null) meleeCollider.enabled = true;

        yield return new WaitForSeconds(data.active);

        if (meleeCollider != null) meleeCollider.enabled = false;
        if (meleeHitbox != null) meleeHitbox.EndSwing();

        yield return new WaitForSeconds(data.recovery);

        if (move != null) move.facingLock = false;
        busy = false;
    }

    IEnumerator DoProjectile(AttackTypeData data, bool isHeavy)
    {
        busy = true;

        if (move != null)
        {
            Vector2 aim = move.Aim;
            int facing = aim.x >= 0 ? 1 : -1;
            move.facingLock = true;
            move.lockedFacing = facing;
        }

        yield return new WaitForSeconds(data.startup);

        if (bulletPrefab != null && muzzle != null)
        {
            Vector2 aim = (move != null) ? move.Aim : new Vector2(1, 0);
            Vector2 dir = aim.normalized;

            var go = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity);
            var b = go.GetComponent<Bullet2D>();
            if (b != null)
                b.Init(dir, hk, transform, data.damage, data.baseForce);
        }

        yield return new WaitForSeconds(data.active + data.recovery);

        if (move != null) move.facingLock = false;
        busy = false;
    }
}
