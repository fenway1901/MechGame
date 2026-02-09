using UnityEngine;

public class GameUtils : MonoBehaviour
{
    /// <summary>
    /// Meaningless call idk why i made this??
    /// </summary>
    public static float GetDistance(Vector3 a, Vector3 b)
    {
        float distance = Vector3.Distance(a, b);
        return distance;
    }

    /// <summary>
    /// Minimal call: shows a white number for 1s that floats up.
    /// Requires a DamagePopupManager in the scene.
    /// </summary>
    public static void ShowDamage(float amount, Vector3 worldPos)
    {
        ShowDamage(amount, worldPos, Color.white, 1.0f, true, 1.5f, 36f, Vector3.zero);
    }

    /// <summary>
    /// Full control version.
    /// </summary>
    public static void ShowDamage(
        float amount,
        Vector3 worldPos,
        Color color,
        float duration = 1.0f,
        bool floatUp = true,
        float floatSpeed = 1.5f,
        float size = 36f,
        Vector3 worldOffset = default,
        float startScale = 0.9f,
        float popScale = 1.2f,
        int decimals = 0,
        bool trimTrailingZeros = true)
    {
        if (DamagePopupManager.Instance == null)
        {
            Debug.LogWarning("GameUtils.ShowDamage: No DamagePopupManager found in scene.");
            return;
        }

        string text = FormatNumber(amount, decimals, trimTrailingZeros);

        DamagePopupManager.Instance.Spawn(
            value: text,
            worldPos: worldPos,
            color: color,
            duration: duration,
            floatUp: floatUp,
            floatSpeed: floatSpeed,
            size: size,
            worldOffset: worldOffset,
            startScale: startScale,
            endScale: popScale
        );
    }

    private static string FormatNumber(float value, int decimals, bool trimZeros)
    {
        decimals = Mathf.Clamp(decimals, 0, 6);

        // If you usually want whole-number damage, set decimals=0 (default)
        string s = value.ToString("F" + decimals);

        if (!trimZeros || decimals == 0) return s;

        // Trim trailing zeros and trailing decimal point
        s = s.TrimEnd('0');
        if (s.EndsWith(".")) s = s.TrimEnd('.');
        return s;
    }

    #region Object Transform Effects

    /// <summary>
    /// Rotates a transform around an axis in local or world space.
    /// Call from Update() with Time.deltaTime.
    /// </summary>
    public static void Rotate(
        Transform t,
        float deltaTime,
        float degreesPerSecond,
        Vector3 axis,
        Space space = Space.Self)
    {
        if (!t) return;

        if (axis.sqrMagnitude < 1e-8f) axis = Vector3.up;
        axis.Normalize();

        float deltaDegrees = degreesPerSecond * deltaTime;
        t.Rotate(axis, deltaDegrees, space);
    }

    /// <summary>
    /// Returns a pulse scalar in range [minScale, maxScale] using a sine wave.
    /// Uses unscaled time if you want it unaffected by Time.timeScale (e.g., UI).
    /// </summary>
    public static float PulseScalar(
        float timeSeconds,
        float pulsesPerSecond,
        float minScale = 0.9f,
        float maxScale = 1.1f)
    {
        if (pulsesPerSecond < 0f) pulsesPerSecond = 0f;

        // sine: [-1,1] -> t01: [0,1]
        float t01 = (Mathf.Sin(timeSeconds * Mathf.PI * 2f * pulsesPerSecond) + 1f) * 0.5f;
        return Mathf.Lerp(minScale, maxScale, t01);
    }

    /// <summary>
    /// Pulses localScale uniformly between [minScale, maxScale].
    /// </summary>
    public static void PulseUniformScale(
        Transform t,
        float timeSeconds,
        float pulsesPerSecond,
        float minScale = 0.9f,
        float maxScale = 1.1f,
        float baseScale = 1f)
    {
        if (!t) return;

        float s = PulseScalar(timeSeconds, pulsesPerSecond, minScale, maxScale) * baseScale;
        t.localScale = new Vector3(s, s, s);
    }

    /// <summary>
    /// Pulses localScale per-axis by multiplying the baseScale vector.
    /// </summary>
    public static void PulseScale(
        Transform t,
        float timeSeconds,
        float pulsesPerSecond,
        Vector3 baseScale,
        float minMul = 0.9f,
        float maxMul = 1.1f)
    {
        if (!t) return;

        float mul = PulseScalar(timeSeconds, pulsesPerSecond, minMul, maxMul);
        t.localScale = baseScale * mul;
    }

    #endregion
}
