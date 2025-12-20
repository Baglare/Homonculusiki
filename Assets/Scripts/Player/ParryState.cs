using System.Collections;
using UnityEngine;

public class ParryState : MonoBehaviour
{
    public bool Active { get; private set; }
    public float parryWindow = 0.15f;
    public float parryCooldown = 0.35f;

    bool cooldown;

    public bool TryParry()
    {
        if (cooldown || Active) return false;
        StartCoroutine(ParryRoutine());
        return true;
    }

    IEnumerator ParryRoutine()
    {
        Active = true;
        yield return new WaitForSeconds(parryWindow);
        Active = false;

        cooldown = true;
        yield return new WaitForSeconds(parryCooldown);
        cooldown = false;
    }
}
