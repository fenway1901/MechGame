using UnityEngine;
using UnityEngine.UI;

public class LimbHealthBar : BaseHealthBar
{
    [SerializeField] private Image limbIcon;

    private BaseLimb attachedLimb;

    public void Init(BaseLimb limb)
    {
        // Don't need now but might be useful just to have
        attachedLimb = limb;

        SetName(limb.limbName);
        SetMaxHealth(limb.GetLimbStats().Stats.Get(StatType.Limb_MaxHealth));
        limb.GetHealthComponent().Damaged += DamageTaken;
        limbIcon.sprite = limb.icon;
    }
}
