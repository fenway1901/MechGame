using UnityEngine;
using UnityEngine.AI;


[DefaultExecutionOrder(50)]
[RequireComponent(typeof(NavMeshAgent))]
public class MechMovementAgent : MonoBehaviour
{
    [SerializeField] Transform visual2D;   // your 2D sprite root
    [SerializeField] float navPlaneY = 0f; // Y-height of your baked NavMesh

    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!visual2D) visual2D = transform;

        agent.updateRotation = false;
        agent.updatePosition = true;   // let agent simulate
        // Built-in NavMesh is Y-up. Keep that. We mirror positions below.
    }

    void LateUpdate()
    {
        // Agent moves on XZ at ~Y = navPlaneY. Mirror to XY.
        Vector3 p = agent.nextPosition;
    }

    // Call this with XY world coords
    public void SetDestinationXY(Vector2 xy)
    {
        Vector3 navTarget = new Vector3(xy.x, navPlaneY, xy.y);
        if (NavMesh.SamplePosition(navTarget, out var hit, 0.6f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(navTarget);
    }
}
