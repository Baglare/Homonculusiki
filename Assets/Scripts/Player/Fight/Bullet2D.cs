using UnityEngine;

public class Bullet2D : MonoBehaviour
{
    public int damage = 10;
    public float baseForce = 8f;

    public float speed = 14f;
    public float lifeTime = 1.2f;

    public Hitbox2D hitbox;

    Vector2 dir;

    public void Init(Vector2 direction, HealthKnockback owner, Transform ownerTf, int dmg, float force)
    {
        dir = direction.normalized;

        damage = dmg;
        baseForce = force;

        if (hitbox == null) hitbox = GetComponent<Hitbox2D>();
        if (hitbox != null)
        {
            hitbox.owner = owner;
            hitbox.ownerTransform = ownerTf;
            hitbox.damage = damage;
            hitbox.knockForce = baseForce;   // artýk multiplier deðil, gerçek force
            hitbox.knockDir = dir;
            hitbox.BeginSwing();
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }
}
