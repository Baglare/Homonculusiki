using UnityEngine;
using System.Collections.Generic;

public class Hitbox2D : MonoBehaviour
{
    public HealthKnockback owner;
    public Transform ownerTransform;

    public int damage = 10;
    public float knockForce = 8f;

    public BoxCollider2D col;

    [HideInInspector] public Vector2 knockDir = Vector2.right;
    [HideInInspector] public bool neutralStrike;
    [HideInInspector] public bool isHeavy;
    [HideInInspector] public bool hasCrit;
    [HideInInspector] public bool isProjectile; // Mermi oldugunu belirtmek icin

    bool active;
    readonly HashSet<HealthKnockback> hitThisSwing = new();

    void Awake()
    {
        if (col == null) col = GetComponent<BoxCollider2D>();
    }

    public void ApplyMeleeShape(Vector2 size, Vector2 offset)
    {
        if (col == null) return;

        // Hitbox objesi yer değiştirmesin
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        col.size = size;
        col.offset = offset;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;

        // Weapon Clash System
        if (other.CompareTag("Hitbox"))
        {
            var otherHitbox = other.GetComponent<Hitbox2D>();
            if (otherHitbox != null && otherHitbox != this && otherHitbox.owner != owner)
            {
                if (isHeavy && !otherHitbox.isHeavy) {
                    otherHitbox.Interrupt(); // Heavy beats light
                } else if (!isHeavy && !otherHitbox.isHeavy) {
                    this.Interrupt(); // Light vs Light (Clash)
                    otherHitbox.Interrupt();
                } else if (!isHeavy && otherHitbox.isHeavy) {
                    this.Interrupt(); // We are light, they are heavy
                } else {
                    this.Interrupt(); // Heavy vs Heavy (Clash)
                    otherHitbox.Interrupt();
                }
            }
            return;
        }

        if (!other.CompareTag("Hurtbox")) return;

        var target = other.GetComponentInParent<HealthKnockback>();
        if (target == null || target == owner) return;

        if (hitThisSwing.Contains(target)) return;
        hitThisSwing.Add(target);

        Vector2 dirToUse = knockDir;

        // Neutral vuruşta (input yokken) hedefe göre sağ/sol seç
        if (neutralStrike && ownerTransform != null)
        {
            float dx = target.transform.position.x - ownerTransform.position.x;
            if (Mathf.Abs(dx) > 0.01f) dirToUse = new Vector2(Mathf.Sign(dx), 0f);
        }

        var parry = target.GetComponent<ParryState>();
        if (parry != null && parry.Active)
        {
            bool isFacingCorrectly = true;
            var targetController = target.GetComponent<PlayerController2D>();
            if (targetController != null)
            {
                // Saldiri kaynaginin X konumu (mermiyse merminin kendisi, yakin dovusse saldirgan)
                float sourceX = isProjectile ? transform.position.x : (ownerTransform != null ? ownerTransform.position.x : transform.position.x);
                float targetX = target.transform.position.x;
                
                // Saldiri (tehdit) hedefin sagi(+) mi yoksa solu(-) mu?
                int threatDir = (sourceX > targetX) ? 1 : -1;

                if (targetController.Facing != threatDir)
                {
                    isFacingCorrectly = false; // Tehdide bakmiyoruz!
                }
            }

            // Parry sadece karakter saldirgana donukse calisir
            if (isFacingCorrectly)
            {
                if (owner != null) {
                    owner.Stun(0.5f); // Stun the attacker
                    var ownerParry = owner.GetComponent<ParryState>();
                    if (ownerParry != null) ownerParry.DisableParry(5f); // Opponent cant block for 5s
                }
                
                // Critical on next hit for the parrier
                var targetHitbox = target.GetComponentInChildren<Hitbox2D>(true);
                if (targetHitbox != null) targetHitbox.hasCrit = true;

                // Eger bu vurus bir mermiyse parrylendikten sonra hemen yok et
                if (isProjectile)
                {
                    Destroy(transform.parent != null ? transform.parent.gameObject : gameObject);
                }

                return;
            }
        }

        int finalDamage = damage;
        if (hasCrit) {
            finalDamage *= 2; // Critical damage
            hasCrit = false;
        }

        // Apply Heartbeat damage multiplier (lower HP = more damage)
        if (owner != null) {
            float t = (owner.MaxHp - owner.CurrentHp) / (float)owner.MaxHp;
            finalDamage = Mathf.RoundToInt(finalDamage * (1f + t)); // up to double damage at 0 HP
        }

        target.TakeHit(finalDamage, dirToUse, knockForce);

        // Mermi ise hedefe basariyla vurduktan sonra yok olmali
        if (isProjectile)
        {
            Destroy(transform.parent != null ? transform.parent.gameObject : gameObject);
        }
    }

    public void Interrupt() {
        active = false;
        if (col != null) col.enabled = false;
        Debug.Log("Clash/Interrupt! Attacker stopped.");
    }

    public void BeginSwing() { active = true; hitThisSwing.Clear(); }
    public void EndSwing() { active = false; }
}
