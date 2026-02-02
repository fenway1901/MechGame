using UnityEngine;
using System.Collections.Generic;

public class BaseLimb : MonoBehaviour
{
    [HideInInspector] public bool isDestroyed;

    public BaseMech _AttachedMech;

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
    protected BoarderPulse targetPulse;
    [SerializeField] protected Gradient healthGrad;
    [SerializeField] protected Color normalColor;
    [SerializeField] protected Color hoverColor;
    [SerializeField] protected Color destroyColor;

    protected BaseHealthComponent health;
    protected BaseLimbStats stats;
    protected LimbWeaponMounts mount;

    [Header("Death Variables")]
    [SerializeField] protected List<BuffDefinition> deathDebuffs;
    [SerializeField] protected List<BuffDefinition> buffToApply;

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        health = GetComponent<BaseHealthComponent>();
        stats = GetComponent<BaseLimbStats>();
        TryGetComponent<LimbWeaponMounts>(out mount);

        health.Damaged += DamageTaken;

        // TO DO: when connection between levels remove this
        // Prototype health will be 100% at start
        sr.color = healthGrad.Evaluate(1f);

        targetPulse = boarderSR.gameObject.GetComponent<BoarderPulse>();
        targetPulse.SetBaseColor(normalColor);
    }

    protected virtual void Reset()
    {
        if (!health) health = GetComponent<BaseHealthComponent>();
    }

    protected virtual void OnEnable()
    {
        if (!health) return;
        health.Died += OnHealthDied_Once;   // one-shot
        // Optional: health.Damaged += OnDamaged; health.Healed += OnHealed;
    }

    protected virtual void OnDisable()
    {
        if (!health) return;
        health.Died -= OnHealthDied_Once;
        // Optional: health.Damaged -= OnDamaged; health.Healed -= OnHealed;
    }

    protected virtual void OnHealthDied_Once(BaseHealthComponent sender)
    {
        if (sender != health) return; // sanity for multi-instance scenes
        DestroyLimb();
        sender.Died -= OnHealthDied_Once;   // ensure one-time subscription
    }

    protected virtual void LimbRestored()
    {
        // Will be used for when you can repair limbs in or out of combat
        // Decide if it should be a full amount of add a heal amount? or does the mech have a stat for how much limbs come back at
    }

    protected virtual void DamageTaken(BaseHealthComponent sender, float amount, float currentHealth)
    {
        sr.color = healthGrad.Evaluate(currentHealth / stats.Stats.Get(StatType.Limb_MaxHealth));

        GameUtils.ShowDamage(amount, transform.position, Color.red, 1.2f, false, size: 1f, startScale: 0.7f, popScale: 1.2f);
    }

    protected virtual void DestroyLimb()
    {
        isDestroyed = true;
        targetPulse.ClearAllTargeters();
        boarderSR.color = destroyColor;
        sr.color = destroyColor;

        LimbWeaponMounts mounts = GetComponent<LimbWeaponMounts>();
        
        if (mounts != null)
            mounts.DisableAllweapons("Attached limb destroyed");

        if (deathDebuffs.Count > 0)
            ApplyDebuffs();
    }
    protected virtual void ApplyDebuffs()
    {
        for (int i = 0; i < deathDebuffs.Count; ++i)
        {
            _AttachedMech.buffController.Apply(deathDebuffs[i], this, _AttachedMech.stats);
        }
    }

    #region Visual Functions

    public virtual void AddTargeter(int targeterId)
    {
        if (isDestroyed) return;
        targetPulse.AddTargeter(targeterId);
    }

    public virtual void RemoveTargeter(int targeterId)
    {
        targetPulse.RemoveTargeter(targeterId);
    }

    public virtual void SetHovered(bool hovered)
    {
        if (!sr) return;
        if (targetPulse.HasTargeters()) return; // wont keep it red when you hover

        boarderSR.color = hovered ? hoverColor : normalColor;

        // put here bc of call order of SetCurrent in LimbHilighter.cs
        if (isDestroyed)
            sr.color = destroyColor;
    }

    public virtual void DestroyedLimb(bool state)
    {
        isDestroyed = state;

        if (isDestroyed)
            sr.color = destroyColor;
        else
            boarderSR.color = normalColor;
    }

    #endregion

    #region Get Functions

    public BaseLimbStats GetLimbStats() { return stats; }
    public BaseHealthComponent GetHealthComponent() { return health; }
    public LimbWeaponMounts GetMount() {  return mount; }

    #endregion
}
