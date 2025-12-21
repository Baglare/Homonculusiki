using UnityEngine;
using UnityEngine.UI;

public class HPBarFollow : MonoBehaviour
{
    public HealthKnockback target;
    public Vector3 offset = new Vector3(0, 1.2f, 0);
    public Slider slider;

    void Awake()
    {
        if (slider == null) slider = GetComponentInChildren<Slider>();
    }

    void LateUpdate()
    {
        if (target == null || slider == null) return;

        // oyuncuyu takip
        transform.position = target.transform.position + offset;

        // ters dönmeyi/garip scale’i engelle
        transform.rotation = Quaternion.identity;

        // (Canvas scale'ini inspector’dan ayarladýysan bu satýrý koyma)
        // transform.localScale = Vector3.one * 0.01f;

        slider.maxValue = target.MaxHp;
        slider.value = target.CurrentHp;
    }
}
