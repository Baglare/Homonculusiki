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

    public void OnLightAttack(InputValue v)
    {
        if (!v.isPressed || busy) return;
        if (weapon == null || weapon.light == null) return;

        if (weapon.isProjectileLight) FireProjectile(weapon.light);
        else StartCoroutine(DoMelee(weapon.light));
    }

    public void OnHeavyAttack(InputValue v)
    {
        if (!v.isPressed || busy) return;
        if (weapon == null || weapon.heavy == null) return;

        if (weapon.isProjectileHeavy) FireProjectile(weapon.heavy);
        else StartCoroutine(DoMelee(weapon.heavy));
    }

    public void OnParry(InputValue v)
    {
        if (!v.isPressed) return;
        if (parry != null) parry.TryParry();
    }

    IEnumerator DoMelee(AttackTypeData data)
    {
        busy = true;

        int facing = (move != null) ? move.Facing : 1;
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

            Vector2 offset = new Vector2(data.hitboxOffset.x * facing, data.hitboxOffset.y);
            meleeHitbox.ApplyMeleeShape(data.hitboxSize, offset);

            meleeHitbox.knockDir = (facing == 1) ? Vector2.right : Vector2.left;
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

    void FireProjectile(AttackTypeData data)
    {
        if (bulletPrefab == null || muzzle == null) return;

        int facing = (move != null) ? move.Facing : 1;
        Vector2 dir = (facing == 1) ? Vector2.right : Vector2.left;

        var go = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity);
        var b = go.GetComponent<Bullet2D>();
        if (b != null)
            b.Init(dir, hk, transform, data.damage, data.baseForce);
    }
}
