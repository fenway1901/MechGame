using UnityEngine;
using System.Collections.Generic;

public class BaseLimb : MonoBehaviour
{
    [HideInInspector] public bool isDestroyed;

    [HideInInspector] public BaseMech _AttachedMech;

    // public varibles
    [Header("Stat Varibles")]
    public string limbName;
    public float armor;
    public float moveSpeed;
    public float weight;

    [Header("Visual Varibles")]
    protected SpriteRenderer sr;
    public SpriteRenderer boarderSR;
    public Sprite icon;
    [SerializeField] protected Gradient healthGrad;
    [SerializeField] protected Color normalColor;
    [SerializeField] protected Color hoverColor;
    [SerializeField] protected Color destroyColor;

    protected BaseHealthComponent health;
    protected BaseLimbStats stats;
    protected LimbWeaponMounts mount;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        health = GetComponent<BaseHealthComponent>();
        stats = GetComponent<BaseLimbStats>();
        TryGetComponent<LimbWeaponMounts>(out mount);

        health.Damaged += DamageTaken;

        // TO DO: when connection between levels remove this
        // Prototype health will be 100% at start
        sr.color = healthGrad.Evaluate(1f);
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

    private void LimbRestored()
    {
        // Will be used for when you can repair limbs in or out of combat
        // Decide if it should be a full amount of add a heal amount? or does the mech have a stat for how much limbs come back at
    }

    private void DamageTaken(BaseHealthComponent sender, float amount, float currentHealth)
    {
        sr.color = healthGrad.Evaluate(currentHealth / stats.Stats.Get(StatType.Limb_MaxHealth));

        GameUtils.ShowDamage(amount, transform.position, Color.red, 1.2f, false, size: 0.8f, startScale: 0.6f, popScale: 1f);
    }

    private void DestroyLimb()
    {
        isDestroyed = true;
        boarderSR.color = destroyColor;
        sr.color = destroyColor;

        LimbWeaponMounts mounts = GetComponent<LimbWeaponMounts>();
        if (mounts != null)
            mounts.DisableAllweapons("Attached limb destroyed");
    }

    private void LimbTargeted()
    {

    }

    public void SetHovered(bool hovered)
    {
        if (!sr) return;
        boarderSR.color = hovered ? hoverColor : normalColor;

        // put here bc of call order of SetCurrent in LimbHilighter.cs
        if (isDestroyed)
            sr.color = destroyColor;
    }

    public void DestroyedLimb(bool state)
    {
        isDestroyed = state;

        if (isDestroyed)
            sr.color = destroyColor;
        else
            boarderSR.color = normalColor;
    }

    #region Get Functions

    public BaseLimbStats GetLimbStats() { return stats; }
    public BaseHealthComponent GetHealthComponent() { return health; }
    public LimbWeaponMounts GetMount() {  return mount; }

    #endregion
}
