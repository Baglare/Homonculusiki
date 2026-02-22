using System.Collections;
using UnityEngine;

public class ParryState : MonoBehaviour
{
    public bool Active { get; private set; }
    public float parryWindow = 0.2f;
    public float parryCooldown = 0.35f;

    bool cooldown;
    bool parryDisabled;

    public bool TryParry()
    {
        if (cooldown || Active || parryDisabled) return false;
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

    public void DisableParry(float time)
    {
        StartCoroutine(DisableRoutine(time));
    }

    IEnumerator DisableRoutine(float time)
    {
        parryDisabled = true;
        yield return new WaitForSeconds(time);
        parryDisabled = false;
    }
}
