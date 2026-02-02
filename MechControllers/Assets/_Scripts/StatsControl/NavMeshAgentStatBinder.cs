using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(StatsComponent))]
public class NavMeshAgentStatBinder : MonoBehaviour
{
    [SerializeField] private StatType moveSpeedStat = StatType.Mech_Speed;

    private NavMeshAgent _agent;
    private StatsComponent _stats;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _stats = GetComponent<StatsComponent>();
    }

    private void Start()
    {
        Apply();
    }

    private void OnEnable()
    {
        _stats.OnStatChanged += Apply;
    }

    private void OnDisable()
    {
        _stats.OnStatChanged -= Apply;
    }

    private void Apply()
    {
        Debug.Log(name + " is applying");
        float speed = _stats.Get(moveSpeedStat);
        _agent.speed = Mathf.Max(0f, speed);
    }
}
