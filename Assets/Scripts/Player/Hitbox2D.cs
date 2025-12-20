using UnityEngine;

public class Hitbox2D : MonoBehaviour
{
    public HealthKnockback owner;
    public int damage = 10;
    public float knockForce = 1f;
    public Transform ownerTransform;
    public bool useTargetRelativeDirWhenNeutral = true;
    private bool active;
    public BoxCollider2D col;
    private readonly System.Collections.Generic.HashSet<HealthKnockback> hitThisSwing =
        new System.Collections.Generic.HashSet<HealthKnockback>();

    [HideInInspector] public Vector2 knockDir = Vector2.right;

    void Awake()
    {
        if (col == null) col = GetComponent<BoxCollider2D>();
    }

    public void ApplyMeleeShape(Vector2 size, Vector2 offset)
    {
        if (col == null) return;
        col.size = size;
        col.offset = offset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active) return;
        if (!other.CompareTag("Hurtbox")) return;

        var target = other.GetComponentInParent<HealthKnockback>();
        if (target == null) return;
        if (target == owner) return;

        Vector2 dirToUse = knockDir;

        // Eðer aim “boþ”sa (duruyorsan) hedefin konumuna göre sað/sol seç
        if (useTargetRelativeDirWhenNeutral && ownerTransform != null)
        {
            // "Neutral" durumunu þöyle yakala: knockDir sadece sað/sol ve sen duruyorsun gibi
            // Daha net istersen AttackController'dan ayrýca bayrak yollayacaðýz.
            float dx = target.transform.position.x - ownerTransform.position.x;
            if (Mathf.Abs(dx) > 0.01f)
                dirToUse = new Vector2(Mathf.Sign(dx), 0f);
        }

        var parry = target.GetComponent<ParryState>();
        if (parry != null && parry.Active)
        {
            // saldýran stun yesin
            var attackerHK = owner;
            if (attackerHK != null)
                attackerHK.Stun(0.25f);

            // hedefe hiç vurma
            return;
        }

        if (hitThisSwing.Contains(target)) return;
        hitThisSwing.Add(target);

        target.TakeHit(damage, dirToUse, knockForce);
    }

    public void BeginSwing()
    {
        active = true;
        hitThisSwing.Clear();
    }

    public void EndSwing()
    {
        active = false;
    }

}
