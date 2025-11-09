using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ClickMoveMech : MonoBehaviour
{
    [SerializeField] Camera cam2D;
    [SerializeField] MechMovementAgent playerAdapter;
    [SerializeField] bool blockWhenOverUI = true;

    void Reset() { cam2D = Camera.main; }

    void Update()
    {
        if (Mouse.current == null) return;
        if (blockWhenOverUI && EventSystem.current &&
            EventSystem.current.IsPointerOverGameObject()) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 w = cam2D.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 xy = new Vector2(w.x, w.y);
            playerAdapter.SetDestinationXY(xy);
        }
    }
}
