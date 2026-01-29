using UnityEngine;

public class HullHealthBar : BaseHealthBar
{
    [SerializeField] private SpriteRenderer panelBoarder;

    public override void DamageTaken(BaseHealthComponent comp, float damage, float currentHealth)
    {
        base.DamageTaken(comp, damage, currentHealth);

        panelBoarder.color = gradient.Evaluate(slider.normalizedValue);
    }
}
