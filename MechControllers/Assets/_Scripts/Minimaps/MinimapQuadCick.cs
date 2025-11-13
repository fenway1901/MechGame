using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MinimapQuadCick : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] Camera clickRayCamera;           // camera that renders the cockpit quad (Main or Cockpit cam)
    [SerializeField] Camera minimapCam;               // the top-down minimap camera (orthographic)

    [Header("Movement target")]
    [SerializeField] MechMovementAgent playerAdapter; // player adapter
    [SerializeField] float groundY = 0f;

    [Header("Quad inputs")]
    [SerializeField] Collider clickCollider;          // this quad's collider
    [SerializeField] MeshFilter meshFilter;           // this quad's mesh filter
    [SerializeField] bool flipY = false;              // set true if the texture appears upside-down
    [SerializeField] bool blockOverUI = true;         // ignore clicks when pointer over UI

    [Header("Selection")]
    [SerializeField] LayerMask selectableMask;  // e.g., "Enemy","Objective"
    [SerializeField] float maxRayDistance = 5000f;
    [SerializeField] SelectionController selection; // optional, else call ISelectable directly

    void Reset()
    {
        clickCollider = GetComponent<Collider>();
        meshFilter = GetComponent<MeshFilter>();
        if (!clickRayCamera) clickRayCamera = Camera.main;
    }

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (blockOverUI && EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;
        if (!mouse.leftButton.wasPressedThisFrame) return;

        // 1) Click must land on the cockpit quad
        var ray = clickRayCamera.ScreenPointToRay(mouse.position.ReadValue());
        if (!Physics.Raycast(ray, out var cockpitHit, 10000f)) return;
        if (cockpitHit.collider != clickCollider) return;

        // 2) Map quad local XY to UV
        Vector3 local = transform.InverseTransformPoint(cockpitHit.point);
        Bounds b = meshFilter.sharedMesh.bounds;
        float u = Mathf.InverseLerp(b.min.x, b.max.x, local.x);
        float v = Mathf.InverseLerp(b.min.y, b.max.y, local.y);
        if (flipY) v = 1f - v;
        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);

        // 3) Make a world ray from the minimap camera
        Ray mmRay = minimapCam.ViewportPointToRay(new Vector3(u, v, 0f));

        // 3a) Try selecting first
        if (Physics.Raycast(mmRay, out var worldHit, maxRayDistance, selectableMask, QueryTriggerInteraction.Ignore))
        {
            // Hit any selectable actor
            var selectable = worldHit.collider.GetComponentInParent<ISelectable>();
            if (selectable != null)
            {
                if (selection) selection.SetSelected(selectable, worldHit);
                else selectable.OnSelected(worldHit);
                return; // consume click
            }
        }

        // 3b) Otherwise treat as move to ground plane
        float denom = mmRay.direction.y;
        if (Mathf.Abs(denom) < 1e-5f) return;
        float t = (groundY - mmRay.origin.y) / denom;
        if (t < 0f) return;

        Vector3 p3 = mmRay.origin + mmRay.direction * t;
        playerAdapter.SetDestinationXY(new Vector2(p3.x, p3.z));
    }
}
