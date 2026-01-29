using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BaseHealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;

    [SerializeField] protected Image fill;
    [SerializeField] protected Image icon;
    [SerializeField] protected TextMeshProUGUI title;

    protected virtual void Awake()
    {
        slider = GetComponent<Slider>();
    }


    public virtual void DamageTaken(BaseHealthComponent comp, float damage, float currentHealth)
    {
        //Debug.Log("parent: " + comp.gameObject.name + " took damage " + damage);

        slider.value = currentHealth;

        fill.color = gradient.Evaluate(slider.normalizedValue);

        if(icon)
            icon.color = gradient.Evaluate(slider.normalizedValue);
    }

    public virtual void SetName(string name) { title.text = name; }
    public virtual void SetHealth(float health) { slider.value = health; }

    public virtual void SetMaxHealth(float max)
    {
        slider.maxValue = max;
        slider.value = max;

        fill.color = gradient.Evaluate(1f);

        if(icon)
            icon.color = gradient.Evaluate(1f);
    }
}
