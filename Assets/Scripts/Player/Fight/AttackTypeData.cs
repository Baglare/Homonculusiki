using UnityEngine;


public enum AttackType { Lineer, Cevik, Genis, Duz }

[CreateAssetMenu(menuName = "Fight/Attack Type Data")]
public class AttackTypeData : ScriptableObject
{
    public AttackType type;

    [Header("Timing")]
    public float startup = 0.10f;
    public float active = 0.10f;
    public float recovery = 0.18f;

    [Header("Hit")]
    public int damage = 10;
    public float forceMultiplier = 1.0f; // HealthKnockback'teki scaled ile çarpýlacak

    [Header("Shape")]
    public Vector2 hitboxSize = new Vector2(1.0f, 0.8f);
    public Vector2 hitboxOffset = new Vector2(0.9f, 0.0f);

    [Header("Genis only (3 phases)")]
    public float phaseActive = 0.06f;
    public float phaseGap = 0.04f;
}
