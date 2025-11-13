using UnityEngine;

public interface ISelectable
{
    // worldHit: where the selection ray hit this object
    void OnSelected(RaycastHit worldHit);
}
