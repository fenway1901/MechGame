using UnityEngine;
using UnityEngine.AI;

public class MechMovementAgent : MonoBehaviour
{
    [SerializeField] Transform visual2D; // the 2D sprite root; if null uses this.transform
    [SerializeField] float visualZ = 0f; // Z for your 2D world

    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!visual2D) visual2D = transform;

        // Disable agent's rotation/position updates. We drive the visual.
        agent.updateUpAxis = true;          // built-in NavMesh expects Y-up
        agent.updateRotation = false;
        agent.updatePosition = true;        // let agent simulate; we mirror to XY each frame
    }

    void LateUpdate()
    {
        // Agent moves on XZ at y?0. Mirror to XY at z=visualZ.
        Vector3 p = agent.nextPosition;                 // or agent.transform.position
        visual2D.position = new Vector3(p.x, p.z, visualZ);
    }

    // Helper for external callers: set destination using XY coords
    public void SetDestinationXY(Vector2 xy)
    {
        Vector3 navTarget = new Vector3(xy.x, 0f, xy.y);
        if (NavMesh.SamplePosition(navTarget, out var hit, 0.6f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(navTarget);
    }
}
