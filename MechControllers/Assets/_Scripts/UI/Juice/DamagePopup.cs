using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rect;

    // runtime state
    private float t;
    private float duration;
    private Vector3 startWorldPos;
    private Vector3 floatDir;
    private float floatSpeed;
    private float startScale;
    private float endScale;

    // simple curves without requiring AnimationCurve assets
    private bool floatUp;

    public bool IsActive { get; private set; }

    private void Reset()
    {
        rect = GetComponent<RectTransform>();
        text = GetComponentInChildren<TMP_Text>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Init(
        string value,
        Color color,
        float duration,
        bool floatUp,
        float floatSpeed,
        float size,
        Vector3 worldPos,
        Vector3 worldOffset,
        float startScale,
        float endScale)
    {
        if (rect == null) rect = (RectTransform)transform;
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

        this.duration = Mathf.Max(0.05f, duration);
        this.floatUp = floatUp;
        this.floatSpeed = Mathf.Max(0f, floatSpeed);
        this.startWorldPos = worldPos + worldOffset;
        this.startScale = startScale;
        this.endScale = endScale;

        t = 0f;
        IsActive = true;

        text.text = value;
        text.color = color;
        text.fontSize = size;

        canvasGroup.alpha = 1f;
        rect.localScale = Vector3.one * startScale;

        floatDir = floatUp ? Vector3.up : Vector3.zero;

        gameObject.SetActive(true);
    }

    public void Tick(float dt)
    {
        if (!IsActive) return;

        t += dt;
        float u = Mathf.Clamp01(t / duration);

        // Position: ease-out float
        if (floatUp && floatSpeed > 0f)
        {
            // ease-out on movement so it slows near the end
            float move = (1f - (1f - u) * (1f - u));
            Vector3 p = startWorldPos + floatDir * (floatSpeed * move * duration);
            DamagePopupManager.Instance.SetPopupWorldPosition(rect, p);
        }
        else
        {
            DamagePopupManager.Instance.SetPopupWorldPosition(rect, startWorldPos);
        }

        // Scale: quick pop then settle
        // (a simple “pop” shape)
        float pop = (u < 0.2f) ? Mathf.Lerp(startScale, endScale, u / 0.2f) : Mathf.Lerp(endScale, 1f, (u - 0.2f) / 0.8f);
        rect.localScale = Vector3.one * pop;

        // Alpha: hold then fade
        float alpha = (u < 0.7f) ? 1f : Mathf.Lerp(1f, 0f, (u - 0.7f) / 0.3f);
        canvasGroup.alpha = alpha;

        if (t >= duration)
        {
            IsActive = false;
            gameObject.SetActive(false);
            DamagePopupManager.Instance.Release(this);
        }
    }
}