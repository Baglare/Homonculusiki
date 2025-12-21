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

    bool active;
    readonly HashSet<HealthKnockback> hitThisSwing = new();

    void Awake()
    {
        if (col == null) col = GetComponent<BoxCollider2D>();
    }

    public void ApplyMeleeShape(Vector2 size, Vector2 offset)
    {
        if (col == null) return;

        // Hitbox objesi yer deðiþtirmesin
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        col.size = size;
        col.offset = offset;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;
        if (!other.CompareTag("Hurtbox")) return;

        var target = other.GetComponentInParent<HealthKnockback>();
        if (target == null || target == owner) return;

        if (hitThisSwing.Contains(target)) return;
        hitThisSwing.Add(target);

        Vector2 dirToUse = knockDir;

        // Neutral vuruþta (input yokken) hedefe göre sað/sol seç
        if (neutralStrike && ownerTransform != null)
        {
            float dx = target.transform.position.x - ownerTransform.position.x;
            if (Mathf.Abs(dx) > 0.01f) dirToUse = new Vector2(Mathf.Sign(dx), 0f);
        }

        var parry = target.GetComponent<ParryState>();
        if (parry != null && parry.Active)
        {
            if (owner != null) owner.Stun(0.25f);
            return;
        }

        target.TakeHit(damage, dirToUse, knockForce);
    }

    public void BeginSwing() { active = true; hitThisSwing.Clear(); }
    public void EndSwing() { active = false; }
}
