using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBaseMech : BaseMech
{
    public Objective CurrentObjective;

    [Header("Health Variables")]
    [SerializeField] protected GameObject enemyCanvas;
    [SerializeField] protected Image healthIndicator;
    [SerializeField] protected Gradient healthGradient;

    [Header("Visual Variables")]
    [SerializeField] protected GameObject selectedIndicator;

    private void Awake()
    {
        if (selectedIndicator)
            selectedIndicator.SetActive(false);
    }

    public override void Init()
    {
        base.Init();

        // Set up AI brain
        brain = GetComponent<MechBrain>();

        if (brain == null)
            Debug.LogError(name + " can not find or does not have MechBrain on same object");

        brain.mech = this;

        // Set up enemy heatlh indicator
        // To do make this fade away until mouse over (unless damaged then always be dispalyed?)
        // plan to only have like 2-3 enemies at once, should be tough to kill that many
        healthComp.Damaged += Damaged;
        healthIndicator.color = healthGradient.Evaluate(1f);
    }

    // Basic attack sequence

    public void SetObjective(Objective obj)
    {
        CurrentObjective = obj;
        brain.currentObjective = obj;
    }

    private void Damaged(BaseHealthComponent health, float amount, float currentHealth)
    {
        healthIndicator.color = healthGradient.Evaluate(currentHealth / stats.Get(StatType.Mech_MaxHealth));
    }

    public void Selected() { selectedIndicator.SetActive(true); }
    public void DeSelected() { selectedIndicator.SetActive(false); }

    #region Limb Management

    #endregion
}
