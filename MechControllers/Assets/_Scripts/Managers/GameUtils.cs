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
}
