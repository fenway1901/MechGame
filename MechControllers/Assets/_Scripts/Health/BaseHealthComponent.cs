using UnityEngine;
using System;
using UnityEngine.Video;

public class BaseHealthComponent : MonoBehaviour
{
    [SerializeField] protected float maxHealth = 100f;
    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }

    // sender, amount, newCurrent
    public event Action<BaseHealthComponent, float, float> Damaged;
    public event Action<BaseHealthComponent, float, float> Healed;
    public event Action<BaseHealthComponent> Died;

    protected virtual void Awake()
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth <= 0 ? maxHealth : CurrentHealth, 0, maxHealth);
        Died += Death;
    }

    public virtual void TakeDamage(float amount)
    {
        if (amount <= 0f || CurrentHealth <= 0f) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0f, maxHealth);
        Damaged?.Invoke(this, amount, CurrentHealth);

        if (CurrentHealth <= 0f)
        {
            Died?.Invoke(this);
        }
    }

    public virtual void Heal(float amount)
    {
        if (amount <= 0f || CurrentHealth <= 0f) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0f, maxHealth);
        Healed?.Invoke(this, amount, CurrentHealth);
    }

    protected virtual void Death(BaseHealthComponent sender)
    {
        Debug.Log(name + " has died!");
    }

    public virtual void SetMaxHealth(float health)
    {
        maxHealth = health;
        FullHeal();
    }

    public virtual void FullHeal()
    {
        CurrentHealth = maxHealth;
    }

    public float Normalized => maxHealth > 0f ? CurrentHealth / maxHealth : 0f;
}
