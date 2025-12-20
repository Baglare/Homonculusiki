using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackController : MonoBehaviour
{
    public PlayerController2D move;
    public Collider2D lightCollider;
    public Hitbox2D lightHitbox;

    public Collider2D heavyCollider;
    public Hitbox2D heavyHitbox;

    public ParryState parry;

    bool busy;

    void Awake()
    {
        if (move == null) move = GetComponent<PlayerController2D>();
        if (parry == null) parry = GetComponent<ParryState>();

        var hk = GetComponent<HealthKnockback>();

        lightHitbox.owner = hk;
        lightHitbox.ownerTransform = transform;
        lightCollider.enabled = false;

        heavyHitbox.owner = hk;
        heavyHitbox.ownerTransform = transform;
        heavyCollider.enabled = false;
    }

    public void OnLightAttack(InputValue v)
    {
        if (!v.isPressed) return;
        if (busy) return;
        StartCoroutine(LightAttack());
    }

    IEnumerator LightAttack()
    {
        busy = true;

        Vector2 aim = move != null ? move.Aim : Vector2.right;
        if (aim.sqrMagnitude < 0.01f)
            aim = (transform.localScale.x < 0f) ? Vector2.left : Vector2.right;

        lightHitbox.knockDir = aim;

        lightHitbox.BeginSwing();
        lightCollider.enabled = true;
        yield return new WaitForSeconds(0.12f);
        lightCollider.enabled = false;
        lightHitbox.EndSwing();

        yield return new WaitForSeconds(0.18f);

        busy = false;
    }

    public void OnHeavyAttack(InputValue v)
    {
        if (!v.isPressed) return;
        if (busy) return;
        StartCoroutine(HeavyAttack());
    }

    IEnumerator HeavyAttack()
    {
        busy = true;

        Vector2 aim = move != null ? move.Aim : Vector2.right;
        if (aim.sqrMagnitude < 0.01f)
            aim = (transform.localScale.x < 0f) ? Vector2.left : Vector2.right;

        heavyHitbox.knockDir = aim;

        // STARTUP
        yield return new WaitForSeconds(0.22f);

        // ACTIVE
        heavyHitbox.BeginSwing();
        heavyCollider.enabled = true;
        yield return new WaitForSeconds(0.10f);
        heavyCollider.enabled = false;
        heavyHitbox.EndSwing();

        // RECOVERY
        yield return new WaitForSeconds(0.30f);

        busy = false;
    }

    public void OnParry(InputValue v)
    {
        if (!v.isPressed) return;
        if (busy) return; // istersen parry saldýrý sýrasýnda da olsun, bunu kaldýrýrsýn
        parry.TryParry();
    }
}
