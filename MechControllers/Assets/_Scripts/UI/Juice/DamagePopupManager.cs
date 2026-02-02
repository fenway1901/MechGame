using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance { get; private set; }

    [Header("Setup")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Canvas worldCanvas; // world-space canvas
    [SerializeField] private DamagePopup popupPrefab;

    [Header("Pooling")]
    [SerializeField] private int prewarm = 30;

    private readonly Queue<DamagePopup> pool = new();
    private readonly List<DamagePopup> active = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (targetCamera == null) targetCamera = Camera.main;

        EnsureCanvas();

        Prewarm(prewarm);
    }

    private void EnsureCanvas()
    {
        if (worldCanvas != null) return;

        // Create a world-space canvas automatically if none assigned
        GameObject go = new GameObject("DamagePopupCanvas");
        go.transform.SetParent(transform, false);

        worldCanvas = go.AddComponent<Canvas>();
        worldCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        go.AddComponent<CanvasScaler>(); // optional, OK in overlay
        go.AddComponent<UnityEngine.UI.GraphicRaycaster>(); // harmless
    }

    private void Prewarm(int count)
    {
        if (popupPrefab == null) return;

        for (int i = 0; i < count; i++)
        {
            DamagePopup p = Instantiate(popupPrefab, worldCanvas.transform);
            p.gameObject.SetActive(false);
            pool.Enqueue(p);
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // Iterate backwards so removals are safe if needed (we also Release from popup)
        for (int i = active.Count - 1; i >= 0; i--)
        {
            DamagePopup p = active[i];
            if (p == null)
            {
                active.RemoveAt(i);
                continue;
            }
            if (p.IsActive)
                p.Tick(dt);
        }
    }

    public void Release(DamagePopup popup)
    {
        active.Remove(popup);
        pool.Enqueue(popup);
    }

    public DamagePopup Spawn(
        string value,
        Vector3 worldPos,
        Color color,
        float duration,
        bool floatUp,
        float floatSpeed,
        float size,
        Vector3 worldOffset,
        float startScale,
        float endScale)
    {
        if (popupPrefab == null)
        {
            Debug.LogError("DamagePopupManager: popupPrefab is not assigned.");
            return null;
        }

        DamagePopup p = pool.Count > 0 ? pool.Dequeue() : Instantiate(popupPrefab, worldCanvas.transform);
        active.Add(p);

        p.transform.SetParent(worldCanvas.transform, false);

        p.Init(
            value: value,
            color: color,
            duration: duration,
            floatUp: floatUp,
            floatSpeed: floatSpeed,
            size: size,
            worldPos: worldPos,
            worldOffset: worldOffset,
            startScale: startScale,
            endScale: endScale);

        return p;
    }

    // Screen-space overlay conversion
    public void SetPopupWorldPosition(RectTransform rect, Vector3 worldPos)
    {
        if (targetCamera == null) targetCamera = Camera.main;

        // Convert world->screen; if you want pure UI positions, this is enough for overlay
        Vector3 screen = targetCamera != null ? targetCamera.WorldToScreenPoint(worldPos) : worldPos;
        rect.position = screen;
    }
}