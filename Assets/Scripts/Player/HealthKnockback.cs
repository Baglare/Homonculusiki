using System.Collections;
using UnityEngine;

public class HealthKnockback : MonoBehaviour
{
    [Header("HP")]
    public int maxHp = 100;
    public float knockbackMultiplier = 1f;

    [Header("Blast Zones (Death Bounds)")]
    public int maxLives = 3;
    public float deathBottom = -15f; // Asagiya dusme
    public float deathTop = 20f;     // Yukari firlayarak olme
    public float deathLeft = -25f;   // Sola firlayarak olme
    public float deathRight = 25f;   // Saga firlayarak olme
    [HideInInspector] public Vector3 spawnPoint;

    int hp;
    int lives;
    Rigidbody2D rb;

    // HPBarFollow bunu okuyacak
    public int CurrentHp => hp;
    public int MaxHp => maxHp;
    public int CurrentLives => lives;

    [Header("Knockback Curve")]
    public float kbMin = 2.5f;        // HP fullken
    public float kbMaxAtZero = 12f;   // HP 0 iken
    public float kbCurvePower = 1.6f; // 1=lineer, >1 sonlara dogru artar

    [Header("Upward Bounce Curve")]
    public float upForceMin = 0f;     // HP fullken eklenecek dikey guc
    public float upForceMax = 7f;     // HP 0 iken eklenecek dikey guc

    void Awake()
    {
        hp = maxHp;
        lives = maxLives;
        spawnPoint = transform.position; // Baslangic noktasini spawn noktasi olarak kaydet
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Karakter belirledigimiz bu dev gorunmez "dikdortgen" sinirlarin disina firlarsa
        if (transform.position.y < deathBottom || transform.position.y > deathTop ||
            transform.position.x < deathLeft || transform.position.x > deathRight)
        {
            DieAndRespawn();
        }
    }

    public void DieAndRespawn()
    {
        lives--;
        
        if (lives > 0)
        {
            // Geri canlandir
            hp = maxHp;
            transform.position = spawnPoint;
            
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // Havada hizla duserkenki hizi sifirla
            }
            
            Debug.Log($"{name} DIED! Remaining Lives: {lives}");
        }
        else
        {
            // Canlar bitti, tamamen yok et (veya GAME OVER cagir)
            Debug.Log($"{name} IS OUT OF LIVES! GAME OVER!");
            gameObject.SetActive(false); // Destroy yerine simdilik saklayalim ki diger scriptler null reference uretmesin
        }
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

        float upForce = Mathf.Lerp(upForceMin, upForceMax, curved);

        if (rb != null)
        {
            // Dikey hizi sifirla ki hep ayni ziplamayi yapsin
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            // Yon ve uygulanan guc vektorunu hesapla, sonra uzerine ekstra ziplama gucu(upForce) ekle
            Vector2 appliedForce = dir.normalized * finalForce;
            
            // Eger vurus asagi dogru degilse veya eger ekstra sicratmak istiyorsak, Y eksenine ekstra kuvveti ekliyoruz.
            // Sadece yatay vuruslarda veya yukari vuruslarda y eksenine itme gucu ekler:
            if (dir.normalized.y >= -0.1f) {
                appliedForce.y += upForce;
            }

            rb.AddForce(appliedForce, ForceMode2D.Impulse);
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
