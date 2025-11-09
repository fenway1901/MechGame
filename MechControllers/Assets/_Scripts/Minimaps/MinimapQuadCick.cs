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

    [Header("Quad inputs")]
    [SerializeField] Collider clickCollider;          // this quad's collider
    [SerializeField] MeshFilter meshFilter;           // this quad's mesh filter
    [SerializeField] bool flipY = false;              // set true if the texture appears upside-down
    [SerializeField] bool blockOverUI = true;         // ignore clicks when pointer over UI

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

        if (blockOverUI && EventSystem.current && EventSystem.current.IsPointerOverGameObject())
            return;

        if (!mouse.leftButton.wasPressedThisFrame) return;

        var ray = clickRayCamera.ScreenPointToRay(mouse.position.ReadValue());
        if (!Physics.Raycast(ray, out var hit, 10000f)) return;
        if (hit.collider != clickCollider) return;

        // Convert hit point on the quad to local space
        Vector3 local = transform.InverseTransformPoint(hit.point);

        // Map local XY to UV [0..1] using the quad mesh bounds
        Bounds b = meshFilter.sharedMesh.bounds; // for Unity Quad: min = (-0.5,-0.5), max = (0.5,0.5)
        float u = Mathf.InverseLerp(b.min.x, b.max.x, local.x);
        float v = Mathf.InverseLerp(b.min.y, b.max.y, local.y);
        if (flipY) v = 1f - v;

        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);

        // Ray from the minimap camera using those UVs
        var mmRay = minimapCam.ViewportPointToRay(new Vector3(u, v, 0f));

        // Intersect with the NavMesh plane at Y = playerAdapter.navPlaneY (usually 0)
        float denom = mmRay.direction.y;
        if (Mathf.Abs(denom) < 1e-5f) return;

        float planeY = 0; // if for some reason I change the y axis do it here
        float t = (planeY - mmRay.origin.y) / denom;
        if (t < 0f) return;

        Vector3 p3 = mmRay.origin + mmRay.direction * t; // XZ world on nav plane
        playerAdapter.SetDestinationXY(new Vector2(p3.x, p3.z));
    }
}
