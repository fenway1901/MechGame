using UnityEngine;

public class WeaponLimbIndicator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float scaleSpeed;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;

    private Vector3 baseScale;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        GameUtils.Rotate(transform, Time.deltaTime, rotationSpeed, Vector3.forward);
        GameUtils.PulseScale(transform, Time.time, scaleSpeed, baseScale, minScale, maxScale);
    }
}
