using UnityEngine;

public class CockpitUILayout : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    [Header("Panel Roots (Transforms)")]
    [SerializeField] private Transform playerStat_Root;   // top-left bar
    [SerializeField] private Transform playerSillo_Root;     // top-right (player mech outline)
    [SerializeField] private Transform map_Root;     // big left map
    [SerializeField] private Transform enemySillo_Root;  // bottom-right (target mech)

    [Header("Boarders")]
    [SerializeField] private SpriteRenderer playerStat_Boarder;
    [SerializeField] private SpriteRenderer playerSillo_Boarder;
    [SerializeField] private SpriteRenderer map_Boarder;
    [SerializeField] private SpriteRenderer enemySillo_Boarder;

    [Header("Corners")]
    [SerializeField] private SpriteRenderer playerStat_Corner;
    [SerializeField] private SpriteRenderer playerSillo_Corner;
    [SerializeField] private SpriteRenderer map_Corner;
    [SerializeField] private SpriteRenderer enemySillo_Corner;

    [Header("Screens")]
    [SerializeField] private SpriteRenderer playerStat_Screen;
    [SerializeField] private SpriteRenderer playerSillo_Screen;
    [SerializeField] private SpriteRenderer enemySillo_Screen;

    [Header("Map Variables")]
    [SerializeField] private Transform mapRenderQuad;     // assign MinimapObj transform
    [SerializeField] private MeshFilter mapRenderMesh;    // assign MinimapObj mesh filter
    [SerializeField] private float mapRenderInset = 0.10f; // world-units inset inside map_Screen (tweak)

    [Header("Layout (Percent of CAMERA VIEW)")]
    [Range(0.05f, 0.60f)]
    [SerializeField] private float rightColumnWidthPercent = 0.25f;

    [Range(0.05f, 0.40f)]
    [SerializeField] private float topHealthHeightPercent = 0.15f;

    [Tooltip("Split of the right column height: 0.5 = equal halves")]
    [Range(0.10f, 0.90f)]
    [SerializeField] private float rightColumnTopHeightPercent = 0.5f;

    [Header("Spacing (World Units)")]
    [SerializeField] private float edgePadding = 0.30f; // distance from camera edges
    [SerializeField] private float panelGap = 0.25f;    // gap between panels

    [Header("Right Column Behavior")]
    [Tooltip("If true, the right column spans the full height (as in your latest mockup).")]
    [SerializeField] private bool rightColumnSpansFullHeight = true;

    [Header("Editor Preview")]
    [SerializeField] private bool previewInEditor = true;

    private void Reset()
    {
        targetCamera = Camera.main;
    }

    private void OnEnable()
    {
        Layout();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying && previewInEditor)
            Layout();
    }

    private void Update()
    {
        if (!Application.isPlaying && previewInEditor)
            Layout();
    }

    public void Layout()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera == null) return;

        if (!targetCamera.orthographic)
        {
            Debug.LogWarning("CockpitUILayout expects an Orthographic camera.");
            return;
        }

        // Camera view in world units
        float viewH = targetCamera.orthographicSize * 2f;
        float viewW = viewH * targetCamera.aspect;

        float left = -viewW * 0.5f;
        float right = viewW * 0.5f;
        float bottom = -viewH * 0.5f;
        float top = viewH * 0.5f;

        // Inner usable rect (padding from edges)
        float innerLeft = left + edgePadding;
        float innerRight = right - edgePadding;
        float innerBottom = bottom + edgePadding;
        float innerTop = top - edgePadding;

        float innerW = Mathf.Max(0.01f, innerRight - innerLeft);
        float innerH = Mathf.Max(0.01f, innerTop - innerBottom);

        // Column widths
        float rightW = innerW * Mathf.Clamp01(rightColumnWidthPercent);
        float leftW = innerW - rightW - panelGap;
        leftW = Mathf.Max(0.01f, leftW);
        rightW = Mathf.Max(0.01f, rightW);

        // X ranges
        float leftXMin = innerLeft;
        float leftXMax = innerLeft + leftW;

        float rightXMin = leftXMax + panelGap;
        float rightXMax = innerRight;

        // Left column heights (top bar + map)
        float topBarH = innerH * Mathf.Clamp01(topHealthHeightPercent);
        float mapH = innerH - topBarH - panelGap;
        mapH = Mathf.Max(0.01f, mapH);
        topBarH = Mathf.Max(0.01f, topBarH);

        // Left column Y ranges
        float topBarYMax = innerTop;
        float topBarYMin = innerTop - topBarH;

        float mapYMin = innerBottom;
        float mapYMax = topBarYMin - panelGap;

        // Right column Y ranges
        float rightYMin = innerBottom;
        float rightYMax = innerTop;

        if (!rightColumnSpansFullHeight)
        {
            // If I ever want the right column to start below the top health bar,
            // uncomment these two lines:
            // rightYMax = innerTop;
            // rightYMin = mapYMin; // or topBarYMin - panelGap; depending on preference
        }

        float rightH = Mathf.Max(0.01f, rightYMax - rightYMin);
        float rightUsableH = Mathf.Max(0.01f, rightH - panelGap);

        float rightTopH = rightUsableH * Mathf.Clamp01(rightColumnTopHeightPercent);
        float rightBottomH = rightUsableH - rightTopH;

        // Right top panel Y
        float rTopYMax = rightYMax;
        float rTopYMin = rightYMax - rightTopH;

        // Right bottom panel Y
        float rBotYMin = rightYMin;
        float rBotYMax = rightYMin + rightBottomH;

        // Apply rects
        // Player Stat Resize
        SetPanel(playerStat_Root, playerStat_Boarder,
            RectFromMinMax(leftXMin, topBarYMin, leftXMax, topBarYMax));
        SetPanel(playerStat_Root, playerStat_Corner,
            RectFromMinMax(leftXMin, topBarYMin, leftXMax, topBarYMax));
        SetPanel(playerStat_Root, playerStat_Screen,
            RectFromMinMax(leftXMin, topBarYMin, leftXMax, topBarYMax));

        // Player Silloete Resize
        SetPanel(playerSillo_Root, playerSillo_Boarder,
            RectFromMinMax(rightXMin, rTopYMin, rightXMax, rTopYMax));
        SetPanel(playerSillo_Root, playerSillo_Corner,
            RectFromMinMax(rightXMin, rTopYMin, rightXMax, rTopYMax));
        SetPanel(playerSillo_Root, playerSillo_Screen,
            RectFromMinMax(rightXMin, rTopYMin, rightXMax, rTopYMax));

        // Map Resize
        SetPanel(map_Root, map_Boarder,
            RectFromMinMax(leftXMin, mapYMin, leftXMax, mapYMax));
        SetPanel(map_Root, map_Corner,
            RectFromMinMax(leftXMin, mapYMin, leftXMax, mapYMax));
        //SetPanel(map_Root, map_Screen,
        //    RectFromMinMax(leftXMin, mapYMin, leftXMax, mapYMax));
        FitQuadToSpriteScreen(mapRenderQuad, mapRenderMesh, map_Corner, mapRenderInset);

        // EnemyResize
        SetPanel(enemySillo_Root, enemySillo_Boarder,
            RectFromMinMax(rightXMin, rBotYMin, rightXMax, rBotYMax));
        SetPanel(enemySillo_Root, enemySillo_Corner,
            RectFromMinMax(rightXMin, rBotYMin, rightXMax, rBotYMax));
        SetPanel(enemySillo_Root, enemySillo_Screen,
            RectFromMinMax(rightXMin, rBotYMin, rightXMax, rBotYMax));
    }

    private static Rect RectFromMinMax(float xMin, float yMin, float xMax, float yMax)
    {
        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    private static void SetPanel(Transform root, SpriteRenderer bg, Rect rect)
    {
        if (root == null || bg == null) return;

        // Place root at center of its rect (preserve existing Z)
        Vector3 pos = root.position;
        pos.x = rect.center.x;
        pos.y = rect.center.y;
        root.position = pos;

        // Size the background sprite in world units (requires DrawMode Sliced/Tiled)
        bg.size = new Vector2(rect.width, rect.height);
    }

    private static void FitQuadToSpriteScreen(
    Transform quad,
    MeshFilter quadMesh,
    SpriteRenderer screen,
    float inset)
    {
        if (quad == null || quadMesh == null || quadMesh.sharedMesh == null || screen == null)
            return;

        // Target size in world units (match the screen sprite, minus inset)
        float targetW = Mathf.Max(0.01f, screen.size.x - 2f * inset);
        float targetH = Mathf.Max(0.01f, screen.size.y - 2f * inset);

        // Base mesh size in local space (so this works for non-Unity quads too)
        Vector3 baseSize = quadMesh.sharedMesh.bounds.size;
        float baseW = Mathf.Max(1e-5f, baseSize.x);
        float baseH = Mathf.Max(1e-5f, baseSize.y);

        // Scale quad in LOCAL space to match target world size (assuming parent scale is 1)
        Vector3 s = quad.localScale;
        s.x = targetW / baseW;
        s.y = targetH / baseH;
        quad.localScale = s;

        // Center it in the panel (preserve Z)
        Vector3 p = quad.localPosition;
        p.x = 0f;
        p.y = 0f;
        quad.localPosition = p;
    }
}
