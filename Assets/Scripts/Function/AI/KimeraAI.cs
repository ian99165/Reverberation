using UnityEngine;
using UnityEngine.AI; // 使用 NavMeshAgent

public class KimeraAI : MonoBehaviour
{
    public float viewAngle = 60f;  // 扇形視野角度
    public float viewDistance = 10f;  // 偵測距離
    public float normalSpeed = 2f;  // 正常移動速度
    public float chaseSpeed = 5f;  // 追逐時的加速速度
    public LayerMask playerLayer;  // 玩家圖層
    public LayerMask obstacleLayer; // 障礙物圖層

    private Transform player;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed; // 預設為正常速度
    }

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, viewDistance, playerLayer);

        bool playerDetected = false;

        foreach (Collider col in colliders)
        {
            player = col.transform;

            // 計算敵人到玩家的方向向量
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // 計算敵人面向方向與玩家方向的夾角
            float angle = Vector3.Angle(transform.forward, directionToPlayer);

            if (angle < viewAngle / 2)
            {
                // 射線檢測，確保視線沒有被遮擋
                if (!Physics.Raycast(transform.position, directionToPlayer, viewDistance, obstacleLayer))
                {
                    playerDetected = true;
                    agent.speed = chaseSpeed; // 加速追擊玩家
                    agent.SetDestination(player.position); // 追蹤玩家
                    Debug.Log("玩家進入視野，敵人加速！");
                    break;
                }
            }
        }

        // 如果沒有偵測到玩家，回復正常速度
        if (!playerDetected)
        {
            agent.speed = normalSpeed;
        }
    }

    // 可視化扇形範圍（在 Scene 檢視中顯示）
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);
    }
}
