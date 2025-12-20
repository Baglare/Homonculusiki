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

        if (meleeHitbox == null) meleeHitbox = GetComponentInChildren<Hitbox2D>();
        if (meleeCollider == null) meleeCollider = meleeHitbox != null ? meleeHitbox.GetComponent<BoxCollider2D>() : null;

        if (meleeHitbox != null)
        {
            meleeHitbox.owner = hk;
            meleeHitbox.ownerTransform = transform;
        }

        if (meleeCollider != null) meleeCollider.enabled = false;
    }

    public void OnLightAttack(InputValue v)
    {
        if (!v.isPressed) return;
        if (busy || weapon == null || weapon.light == null) return;

        if (weapon.isProjectileLight)
            FireProjectile(weapon.light);
        else
            StartCoroutine(DoMelee(weapon.light));
    }

    public void OnHeavyAttack(InputValue v)
    {
        if (!v.isPressed) return;
        if (busy || weapon == null || weapon.heavy == null) return;

        if (weapon.isProjectileHeavy)
            FireProjectile(weapon.heavy);
        else
            StartCoroutine(DoMelee(weapon.heavy));
    }

    public void OnParry(InputValue v)
    {
        if (!v.isPressed) return;
        if (parry != null) parry.TryParry();
    }

    IEnumerator DoMelee(AttackTypeData data)
    {
        busy = true;

        // Facing lock: saldýrý boyunca yön sabit
        int facing = (transform.localScale.x < 0f) ? -1 : 1;
        if (move != null)
        {
            move.facingLock = true;
            move.lockedFacing = facing;
        }

        // Melee þekil ayarý
        if (meleeHitbox != null)
        {
            meleeHitbox.damage = data.damage;
            meleeHitbox.knockForce = data.forceMultiplier; // çarpan mantýðý
            meleeHitbox.ApplyMeleeShape(data.hitboxSize, new Vector2(data.hitboxOffset.x * facing, data.hitboxOffset.y));
        }

        // aim yoksa sað/sol kilitli yönden seç
        Vector2 dir = (facing == 1) ? Vector2.right : Vector2.left;
        if (meleeHitbox != null) meleeHitbox.knockDir = dir;

        // STARTUP
        yield return new WaitForSeconds(data.startup);

        if (data.type == AttackType.Genis)
        {
            // 3 faz: üst-orta-alt
            Vector2[] dirs = new Vector2[]
            {
                new Vector2(facing,  1).normalized,
                new Vector2(facing,  0).normalized,
                new Vector2(facing, -1).normalized
            };

            for (int i = 0; i < 3; i++)
            {
                meleeHitbox.knockDir = dirs[i];

                meleeHitbox.BeginSwing();
                meleeCollider.enabled = true;
                yield return new WaitForSeconds(data.phaseActive);
                meleeCollider.enabled = false;
                meleeHitbox.EndSwing();

                yield return new WaitForSeconds(data.phaseGap);
            }
        }
        else
        {
            // tek faz
            meleeHitbox.BeginSwing();
            meleeCollider.enabled = true;
            yield return new WaitForSeconds(data.active);
            meleeCollider.enabled = false;
            meleeHitbox.EndSwing();
        }

        // RECOVERY
        yield return new WaitForSeconds(data.recovery);

        if (move != null) move.facingLock = false;
        busy = false;
    }

    void FireProjectile(AttackTypeData data)
    {
        if (bulletPrefab == null || muzzle == null) return;

        int facing = (transform.localScale.x < 0f) ? -1 : 1;
        Vector2 dir = (facing == 1) ? Vector2.right : Vector2.left;

        var go = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity);

        var b = go.GetComponent<Bullet2D>();
        if (b != null)
        {
            b.damage = data.damage;
            b.forceMultiplier = data.forceMultiplier;
            b.Init(dir, hk, transform);
        }
    }
}
