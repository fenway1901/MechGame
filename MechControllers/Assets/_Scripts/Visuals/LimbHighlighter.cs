using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class LimbHighlighter : MonoBehaviour
{
    public static LimbHighlighter instance;

    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask limbMask;   // set to "Limb"
    [SerializeField] private int maxHits = 32;
    [SerializeField] private bool allowUIHoverBlock = true;

    private Collider2D[] hits;
    private ContactFilter2D filter;
    public BaseLimb currentLimb;

    void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(instance);

        if (!cam) cam = Camera.main;
        hits = new Collider2D[maxHits];

        filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = limbMask,
            useTriggers = true // set false if you only want non-trigger colliders
        };
    }

    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) { SetCurrent(null); return; }

        if (allowUIHoverBlock && EventSystem.current &&
            EventSystem.current.IsPointerOverGameObject())
        {
            SetCurrent(null);
            return;
        }

        Vector2 screen = mouse.position.ReadValue();
        Vector2 wp = cam.ScreenToWorldPoint(screen);

        int count = Physics2D.OverlapPoint(wp, filter, hits);

        BaseLimb top = null;
        int bestLayer = int.MinValue;
        int bestOrder = int.MinValue;
        float bestZ = float.NegativeInfinity;

        for (int i = 0; i < count; i++)
        {
            var col = hits[i];
            if (!col) continue;

            var target = col.GetComponentInParent<BaseLimb>();
            if (!target || !target.sr) continue;

            var r = target.sr;
            int layerId = SortingLayer.GetLayerValueFromID(r.sortingLayerID);
            int order = r.sortingOrder;
            float z = -r.transform.position.z;

            bool better =
                layerId > bestLayer ||
               (layerId == bestLayer && order > bestOrder) ||
               (layerId == bestLayer && order == bestOrder && z > bestZ);

            if (better)
            {
                bestLayer = layerId;
                bestOrder = order;
                bestZ = z;
                top = target;
            }
        }

        SetCurrent(top);
    }

    private void SetCurrent(BaseLimb next)
    {
        if (next != null && next.isDestroyed)
            next = null;

        if (currentLimb == next) return;

        if (currentLimb != null)
            currentLimb.SetHovered(false);

        currentLimb = next;

        if (currentLimb != null && !currentLimb.isDestroyed)
            currentLimb.SetHovered(true);
    }
}
