using UnityEngine;
using UnityEngine.AI;

public class BugAI : MonoBehaviour
{
    public enum MonsterState
    {
        Patrolling,
        Chasing
    }

    private bool _is_player;
    public MonsterState currentState = MonsterState.Patrolling;
    public Transform player;
    public float detectionRadius = 5f;
    public float moveSpeed = 0.5f;
    public float chasingSpeed = 1f;
    public float minPauseTime = 1f;
    public float maxPauseTime = 3f;
    public float patrolRadius = 10f;
    public float minPatrolChangeTime = 5f;
    public float maxPatrolChangeTime = 10f;

    private NavMeshAgent agent;
    private float timeToChangePatrolPoint;
    public bool isHostile = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.autoBraking = false;
        SetRandomDestination();
        SetNextPatrolTime();
    }

    void Update()
    {
        if (!isHostile && currentState != MonsterState.Patrolling)
        {
            SwitchToPatrolling();
        }

        if (_is_player)
        {
            agent.isStopped = true;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (isHostile && distanceToPlayer <= detectionRadius && currentState != MonsterState.Chasing)
        {
            SwitchToChasing();
        }
        else if (distanceToPlayer > detectionRadius && currentState != MonsterState.Patrolling)
        {
            SwitchToPatrolling();
        }

        switch (currentState)
        {
            case MonsterState.Patrolling:
                Patrol();
                break;
            case MonsterState.Chasing:
                ChasePlayer();
                break;
        }
    }

    void Patrol()
    {
        if (Time.time >= timeToChangePatrolPoint)
        {
            SetRandomDestination();
            SetNextPatrolTime();
        }
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);
        agent.speed = chasingSpeed;
    }

    void SetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void SetNextPatrolTime()
    {
        timeToChangePatrolPoint = Time.time + Random.Range(minPatrolChangeTime, maxPatrolChangeTime);
    }

    public void SwitchToChasing()
    {
        currentState = MonsterState.Chasing;
        agent.speed = chasingSpeed;
    }

    public void SwitchToPatrolling()
    {
        currentState = MonsterState.Patrolling;
        agent.speed = moveSpeed;
        SetRandomDestination();
        SetNextPatrolTime();
    }

    public void EnableHostile()
    {
        isHostile = true;
    }

    public void DisableHostile()
    {
        isHostile = false;
        SwitchToPatrolling();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _is_player = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _is_player = false;
        }
    }
}
