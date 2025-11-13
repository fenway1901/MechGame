using UnityEngine;

public class EnemySelectionManager : MonoBehaviour
{
    public static EnemySelectionManager instance;

    private Transform targetPanel;
    public BaseMech targetMech;
    private GameObject mechLayout;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(instance);

        targetPanel = GameObject.Find("TargetMechPanel").transform; // def make this better
    }

    // each character will have a prefab for its layout
    // spawn that instance here

    public void SetActiveTarget(BaseMech target) 
    {
        if (target == null) return;
        if (target == targetMech) return;

        Debug.Log("setting current target to " + target.gameObject.name);

        targetMech = target;
        
        if (mechLayout != null)
            Destroy(mechLayout);
        
        mechLayout = Instantiate(targetMech.layout, targetPanel);
    }
}
