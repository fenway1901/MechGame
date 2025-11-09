using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapClickBuiltInNav : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Camera minimapCam;       // orthographic 2D minimap camera
    [SerializeField] MechMovementAgent playerAdapter;
    RectTransform rt;

    void Awake() { rt = GetComponent<RectTransform>(); }

    public void OnPointerClick(PointerEventData e)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rt, e.position, e.pressEventCamera, out var local)) return;

        var r = rt.rect;
        float u = Mathf.Clamp01((local.x - r.xMin) / r.width);
        float v = Mathf.Clamp01((local.y - r.yMin) / r.height);

        // Ray from minimapCam into XY world
        var ray = minimapCam.ViewportPointToRay(new Vector3(u, v, 0));
        // In 2D ortho, use plane z of camera. Compute intersection at z=0 of 2D world
        float t = -ray.origin.z / ray.direction.z;
        Vector3 world = ray.origin + ray.direction * t; // XY coords in world
        playerAdapter.SetDestinationXY(new Vector2(world.x, world.y));
    }
}
