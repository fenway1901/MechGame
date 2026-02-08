using UnityEngine;
using UnityEngine.UI;

public class EnemySelectionManager : MonoBehaviour
{
    public static EnemySelectionManager instance;

    public BaseMech targetMech;
    private GameObject mechLayout;


    [SerializeField] private Image hullIndicator;

    public Gradient healthGradient;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    // each character will have a prefab for its layout
    // spawn that instance here

    public void SetActiveTarget(BaseMech target) 
    {
        if (target == null) return;
        if (target == targetMech) return;

        if (targetMech != null)
            targetMech.GetHealthComponent().Damaged -= MechDamaged;

        if (mechLayout != target.spawnedLayout && mechLayout != null)
        {
            (targetMech as EnemyBaseMech).DeSelected();
            targetMech.spawnedLayout.SetActive(false);
        }

        //Debug.Log("setting current target to " + target.gameObject.name);

        targetMech = target;
        
        mechLayout = targetMech.spawnedLayout;
        mechLayout.SetActive(true);
        (targetMech as EnemyBaseMech).Selected();

        hullIndicator.color = healthGradient.Evaluate(targetMech.GetHealthComponent().CurrentHealth / targetMech.stats.Get(StatType.Mech_MaxHealth));

        targetMech.GetHealthComponent().Damaged += MechDamaged;
    }

    private void MechDamaged(BaseHealthComponent healthComp, float amount, float currentHealth)
    {
        hullIndicator.color = healthGradient.Evaluate(currentHealth / targetMech.stats.Get(StatType.Mech_MaxHealth));
    }
}
