using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public ISelectable Current { get; private set; }

    public void SetSelected(ISelectable selectable, RaycastHit hit)
    {
        Current = selectable;
        selectable.OnSelected(hit);
        // Fire events, update HUD, play SFX, etc.
    }
}
