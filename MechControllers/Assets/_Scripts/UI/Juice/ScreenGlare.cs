using UnityEngine;

public class ScreenGlare : MonoBehaviour
{
    [SerializeField] private SpriteRenderer glare;

    [Header("Base")]
    [SerializeField, Range(0f, 1f)] private float baseAlpha = 0.05f;

    [Header("Pulse")]
    [SerializeField] private float pulseAddAlpha = 0.35f;
    [SerializeField] private float pulseDuration = 0.10f;

    [Header("Fade Out Before Disable")]
    [SerializeField] private float fadeOutDuration = 0.08f;

    enum Phase { Off, Pulse, Fade }
    Phase phase = Phase.Off;

    float timer;
    float peakAlpha;
    Color baseColor;

    void Awake()
    {
        if (!glare) glare = GetComponent<SpriteRenderer>();
        baseColor = glare.color;
        ResetVisual();
    }

    void OnEnable()
    {
        ResetVisual();
    }

    void Update()
    {
        if (phase == Phase.Off) return;

        timer -= Time.unscaledDeltaTime;

        if (phase == Phase.Pulse)
        {
            float t = 1f - Mathf.Clamp01(timer / pulseDuration);
            float alpha = Mathf.Lerp(peakAlpha, baseAlpha, t);
            SetAlpha(alpha);

            if (timer <= 0f)
            {
                if (fadeOutDuration <= 0f)
                {
                    DisableNow();
                }
                else
                {
                    phase = Phase.Fade;
                    timer = fadeOutDuration;
                }
            }
        }
        else if (phase == Phase.Fade)
        {
            float t = 1f - Mathf.Clamp01(timer / fadeOutDuration);
            float alpha = Mathf.Lerp(baseAlpha, 0f, t);
            SetAlpha(alpha);

            if (timer <= 0f)
            {
                DisableNow();
            }
        }
    }

    public void TriggerHitPulse(float intensity = 1f)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        peakAlpha = baseAlpha + pulseAddAlpha * Mathf.Clamp01(intensity);

        phase = Phase.Pulse;
        timer = pulseDuration;

        SetAlpha(peakAlpha);
    }

    void SetAlpha(float a)
    {
        var c = glare.color;
        c.a = a;
        glare.color = c;
    }

    void DisableNow()
    {
        phase = Phase.Off;
        gameObject.SetActive(false);
    }

    void ResetVisual()
    {
        phase = Phase.Off;
        timer = 0f;
        SetAlpha(0f);
    }
}
