using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Axes (world)")]
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;
    [SerializeField] private bool followZ = true;

    [Header("Offset")]
    [SerializeField] private Vector3 offset = Vector3.zero;

    [Header("Smoothing")]
    [Tooltip("0 = snap instantly. Higher = smoother.")]
    [Min(0f)]
    [SerializeField] private float smoothTime = 0.15f;

    private Vector3 _velocity;

    public void SetTarget(Transform newTarget) => target = newTarget;

    private void LateUpdate()
    {
        Tick(Time.deltaTime);
    }

    private void Tick(float dt)
    {
        if (target == null) return;

        Vector3 current = transform.position;
        Vector3 t = target.position;

        Vector3 desired = current;

        if (followX) desired.x = t.x;
        if (followY) desired.y = t.y;
        if (followZ) desired.z = t.z;

        desired += offset;

        if (smoothTime <= 0f)
        {
            transform.position = desired;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(current, desired, ref _velocity, smoothTime, Mathf.Infinity, dt);
        }
    }
}
