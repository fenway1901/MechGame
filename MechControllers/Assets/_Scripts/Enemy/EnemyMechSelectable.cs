using UnityEngine;

public class EnemyMechSelectable : MonoBehaviour, ISelectable
{
    public BaseMech attachedMech;

    private void Awake()
    {
        TryGetComponent<BaseMech>(out BaseMech mech);

        if (mech != null)
            attachedMech = mech;
    }

    public void OnSelected(RaycastHit hit)
    {
        EnemySelectionManager.instance.SetActiveTarget(attachedMech);
    }
}
