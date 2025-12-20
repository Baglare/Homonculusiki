using UnityEngine;

public class HealthKnockback : MonoBehaviour
{
    public int maxHp = 100;
    public float knockbackMultiplier = 1f;

    private int hp;
    private Rigidbody2D rb;

    [Header("Knockback Curve")]
    public float kbMin = 2.5f;        // HP fullken
    public float kbMaxAtZero = 12f;   // HP 0 iken hedeflediðin max
    public float kbCurvePower = 1.6f; // 1.0 lineer, >1 sonlara doðru hýzlanýr, <1 baþta agresif

    public void TakeHit(int damage, Vector2 dir, float force)
    {
        hp = Mathf.Max(0, hp - damage);

        float t = (maxHp - hp) / (float)maxHp;     // 0..1 (ne kadar hasar birikti)
        float curved = Mathf.Pow(t, kbCurvePower); // eðri

        float scaled = Mathf.Lerp(kbMin, kbMaxAtZero, curved);

        // force burada saldýrý çarpaný: Light 1.0, Heavy 1.5 gibi
        float finalForce = scaled * force * knockbackMultiplier;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(dir.normalized * finalForce, ForceMode2D.Impulse);

        StartCoroutine(LockControl(0.15f));
    }

    void Awake()
    {
        hp = maxHp;
        rb = GetComponent<Rigidbody2D>();
    }

    private System.Collections.IEnumerator LockControl(float t)
    {
        var pc = GetComponent<PlayerController2D>();
        if (pc == null) yield break;

        pc.controlLocked = true;
        yield return new WaitForSeconds(t);
        pc.controlLocked = false;
    }

    public void Stun(float t)
    {
        StartCoroutine(LockControl(t));
    }
}
