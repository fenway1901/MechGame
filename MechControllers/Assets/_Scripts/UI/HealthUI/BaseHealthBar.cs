using UnityEngine;
using UnityEngine.UI;


public class BaseHealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;

    public Image fill;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetHealth(float health)
    {
        slider.value = health;
    }

    public void SetMaxHealth(float max)
    {
        slider.maxValue = max;
        slider.value = max;

        fill.color = gradient.Evaluate(1f);
    }

    public void DamageTaken(BaseHealthComponent comp, float damage, float currentHealth)
    {
        slider.value = currentHealth;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
