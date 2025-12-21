using System.Collections;
using UnityEngine;

public class HealthKnockback : MonoBehaviour
{
    [Header("HP")]
    public int maxHp = 100;
    public float knockbackMultiplier = 1f;

    int hp;
    Rigidbody2D rb;

    // HPBarFollow bunu okuyacak
    public int CurrentHp => hp;
    public int MaxHp => maxHp;

    [Header("Knockback Curve")]
    public float kbMin = 2.5f;        // HP fullken
    public float kbMaxAtZero = 12f;   // HP 0 iken
    public float kbCurvePower = 1.6f; // 1=lineer, >1 sonlara do�ru artar

    void Awake()
    {
        hp = maxHp;
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeHit(int damage, Vector2 dir, float force)
    {
        int before = hp;
        hp = Mathf.Max(0, hp - damage);

        // 0..1 (ne kadar hasar birikti)
        float t = (maxHp - hp) / (float)maxHp;
        float curved = Mathf.Pow(t, kbCurvePower);

        float scaled = Mathf.Lerp(kbMin, kbMaxAtZero, curved);
        float finalForce = scaled * force * knockbackMultiplier;

        if (rb != null)
        {
            // Senin mant�k: d��ey h�z s�f�r, yatay kals�n
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(dir.normalized * finalForce, ForceMode2D.Impulse);
        }

        StartCoroutine(LockControl(0.15f));

        Debug.Log($"{name} TAKEHIT dmg={damage} hp(before)={before} hp(after)={hp} t={t:0.00} scaled={scaled:0.00} force={force:0.00} final={finalForce:0.00}", this);
    }

    IEnumerator LockControl(float t)
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
