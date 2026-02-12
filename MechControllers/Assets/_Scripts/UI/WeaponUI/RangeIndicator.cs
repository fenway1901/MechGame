using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;

    private float spriteDiameterWorld = 1f;

    private void Reset() => sr = GetComponent<SpriteRenderer>();

    private void Awake()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();

        // Cache sprite diameter in world units at localScale = 1
        if (sr.sprite != null)
            spriteDiameterWorld = sr.sprite.bounds.size.x;

        sr.enabled = false;
    }

    /// <summary>
    /// rangeWorldUnits is the weapon radius in world units (1 = 1 Unity unit).
    /// </summary>
    public void SetRange(float rangeWorldUnits)
    {
        if (sr.sprite == null || spriteDiameterWorld <= 0f) return;

        float desiredDiameter = rangeWorldUnits * 2f;
        float uniformScale = desiredDiameter / spriteDiameterWorld;

        transform.localScale = new Vector3(uniformScale, uniformScale, 1f);
    }

    public void SetVisible(bool visible)
    {
        sr.enabled = visible;
    }
}
