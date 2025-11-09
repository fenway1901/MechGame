using UnityEngine;

public class BaseLimb : MonoBehaviour
{
    public bool isDestroyed;

    // public varibles
    [Header("Stat Varibles")]
    public string limbName;
    public float armor;
    public float moveSpeed;
    public float weight;

    [Header("Visual Varibles")]
    [HideInInspector] public SpriteRenderer sr;
    [SerializeField] protected Color normalColor;
    [SerializeField] protected Color hoverColor;
    [SerializeField] protected Color destroyColor;

    [SerializeField] BaseHealthComponent health;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        health = GetComponent<BaseHealthComponent>();
    }

    private void Reset()
    {
        if (!health) health = GetComponent<BaseHealthComponent>();
    }

    private void OnEnable()
    {
        if (!health) return;
        health.Died += OnHealthDied_Once;   // one-shot
        // Optional: health.Damaged += OnDamaged; health.Healed += OnHealed;
    }

    private void OnDisable()
    {
        if (!health) return;
        health.Died -= OnHealthDied_Once;
        // Optional: health.Damaged -= OnDamaged; health.Healed -= OnHealed;
    }

    private void OnHealthDied_Once(BaseHealthComponent sender)
    {
        if (sender != health) return; // sanity for multi-instance scenes
        DestroyLimb();
        sender.Died -= OnHealthDied_Once;   // ensure one-time subscription
    }

    private void DestroyLimb()
    {
        // your destruction logic
        isDestroyed = true;
        //Destroy(gameObject);
    }

    public void SetHovered(bool hovered)
    {
        if (!sr) return;
        sr.color = hovered ? hoverColor : normalColor;

        // put here bc of call order of SetCurrent in LimbHilighter.cs
        if (isDestroyed)
            sr.color = destroyColor;
    }

    public void DestroyedLimb()
    {
        isDestroyed = true;
    }
}
