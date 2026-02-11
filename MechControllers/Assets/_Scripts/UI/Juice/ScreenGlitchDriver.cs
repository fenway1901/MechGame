using UnityEngine;

public class ScreenGlitchDriver : MonoBehaviour
{
    [SerializeField] private SpriteRenderer screen;

    [Header("Envelope")]
    [SerializeField] private float attack = 0.03f;
    [SerializeField] private float decay = 0.20f;

    [Header("Disable Fade")]
    [SerializeField] private float fadeOut = 0.05f; // seconds to fade alpha to 0 before disabling

    static readonly int GlitchID = Shader.PropertyToID("_Glitch");
    static readonly int SeedID = Shader.PropertyToID("_Seed");

    MaterialPropertyBlock mpb;

    enum Phase { Off, Attack, Decay, FadeOut }
    Phase phase = Phase.Off;

    float amp;         // 0..1 intensity for this hit
    float elapsed;     // time within current phase

    Color baseColor;   // original sprite color (including alpha)

    void Awake()
    {
        if (!screen) screen = GetComponent<SpriteRenderer>();

        mpb = new MaterialPropertyBlock();
        baseColor = screen.color;

        Apply(0f, Random.value * 1000f);
        SetAlpha(baseColor.a);
    }

    void OnEnable()
    {
        // Clean reset every time it’s re-enabled
        phase = Phase.Off;
        elapsed = 0f;

        Apply(0f, 0f);
        SetAlpha(baseColor.a);
    }

    void Update()
    {
        if (phase == Phase.Off) return;

        float dt = Time.unscaledDeltaTime;
        elapsed += dt;

        if (phase == Phase.Attack)
        {
            float a = Mathf.Max(0.0001f, attack);
            float v = amp * Mathf.Clamp01(elapsed / a);
            Apply(v, 0f);

            if (elapsed >= a)
            {
                phase = Phase.Decay;
                elapsed = 0f;
            }
        }
        else if (phase == Phase.Decay)
        {
            float d = Mathf.Max(0.0001f, decay);
            float v = amp * (1f - Mathf.Clamp01(elapsed / d));
            Apply(v, 0f);

            if (elapsed >= d)
            {
                // Glitch is over; now fade alpha out, then disable
                Apply(0f, 0f);

                if (fadeOut <= 0f)
                {
                    SetAlpha(0f);
                    phase = Phase.Off;
                    gameObject.SetActive(false);
                }
                else
                {
                    phase = Phase.FadeOut;
                    elapsed = 0f;
                }
            }
        }
        else // FadeOut
        {
            float f = Mathf.Max(0.0001f, fadeOut);
            float t = Mathf.Clamp01(elapsed / f);

            float a = Mathf.Lerp(baseColor.a, 0f, t);
            SetAlpha(a);

            if (t >= 1f)
            {
                phase = Phase.Off;
                gameObject.SetActive(false);
            }
        }
    }

    public void TriggerHit(float intensity = 1f)
    {
        amp = Mathf.Clamp01(intensity);
        elapsed = 0f;
        phase = Phase.Attack;

        if (!gameObject.activeSelf) gameObject.SetActive(true);

        // ensure visible at start of hit
        SetAlpha(baseColor.a);

        // change pattern per hit
        Apply(0.001f, Random.value * 1000f);
    }

    void SetAlpha(float a)
    {
        var c = screen.color;
        c.a = a;
        screen.color = c;
    }

    void Apply(float glitch, float seedIfNonZero)
    {
        screen.GetPropertyBlock(mpb);

        mpb.SetFloat(GlitchID, glitch);

        if (seedIfNonZero != 0f)
            mpb.SetFloat(SeedID, seedIfNonZero);

        screen.SetPropertyBlock(mpb);
    }
}
