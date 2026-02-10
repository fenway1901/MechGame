using UnityEngine;

public class HullHealthBar : BaseHealthBar
{
    [SerializeField] private SpriteRenderer panelBoarder;
    [SerializeField] private SpriteRenderer silloBoarder;
    [SerializeField] private float flashThreashHold;

    private bool flashBoarder;

    protected override void Awake()
    {
        base.Awake();

        panelBoarder.color = gradient.Evaluate(1f);
    }

    private void Update()
    {
        if (!flashBoarder) return;

        panelBoarder.color = GameUtils.PulseColor(
                                gradient.Evaluate(0f),
                                Color.red,
                                Time.time,
                                1.5f
                                );
    }

    public override void DamageTaken(BaseHealthComponent comp, float damage, float currentHealth)
    {
        base.DamageTaken(comp, damage, currentHealth);

        if (flashThreashHold < (currentHealth / (comp as MechHealthComponent)._AttachedMech.GetComponent<BaseMech>().stats.Get(StatType.Mech_MaxHealth)))
        {
            flashBoarder = false;
            panelBoarder.color = gradient.Evaluate(slider.normalizedValue);
        }
        else
        {
            flashBoarder = true;
        }

        silloBoarder.color = gradient.Evaluate(slider.normalizedValue);
    }
}
