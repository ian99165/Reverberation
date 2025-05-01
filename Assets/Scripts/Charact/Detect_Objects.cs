using UnityEngine;

public class Detect_Objects : MonoBehaviour
{
    [Header("移動機率設定")]
    [Range(0f, 1f)] public float teleportChance = 0.1f; // 物件移動機率

    [Header("偵測圖層")]
    public LayerMask detectionLayer; // 偵測專屬圖層

    [Header("角色相機")]
    public Camera playerCamera; // 玩家攝像機

    [Header("擴大範圍")]
    public float detectionRadius = 50f; // 偵測範圍半徑

    private void Update()
    {
        CheckForObjectsInView();
    }

    private void CheckForObjectsInView()
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerCamera);

        Collider[] colliders = Physics.OverlapSphere(playerCamera.transform.position, detectionRadius, detectionLayer);
        foreach (var collider in colliders)
        {
            if (GeometryUtility.TestPlanesAABB(frustumPlanes, collider.bounds))
            {

                Object_Transfer transferScript = collider.GetComponent<Object_Transfer>();
                if (transferScript != null)
                {
                    transferScript.OnEnterView(); // 記錄進入視錐範圍
                }
            }
            else
            {
                Object_Transfer transferScript = collider.GetComponent<Object_Transfer>();
                if (transferScript != null)
                {
                    transferScript.OnExitView(teleportChance);
                }
            }
        }
    }
}