using UnityEngine;

public class Object_Transfer : MonoBehaviour
{
    [Header("移動範圍設定")]
    public Vector3 teleportRange = new Vector3(0.1f, 0f, 0.1f); // 隨機移動範圍

    [Header("隱藏功能設定")]
    [Range(0f, 1f)] public float hideChance = 0.05f; // 隱藏機率
    private bool isHidden = false; // 紀錄物件是否隱藏

    private MeshRenderer meshRenderer; // 用於控制物件可見性
    private Collider objectCollider; // 碰撞器，用於保留偵測功能
    private bool isInView = false; // 紀錄物件是否在視線內

    public Camera playerCamera; // 玩家相機，用於視線檢測

    void Awake()
    {
        // 初始化 MeshRenderer 和 Collider
        meshRenderer = GetComponent<MeshRenderer>();
        objectCollider = GetComponent<Collider>();

        if (meshRenderer == null)
        {
            //Debug.LogWarning($"物件 {name} 缺少 MeshRenderer，無法進行隱藏操作！");
        }

        if (playerCamera == null)
        {
            //Debug.LogError($"物件 {name} 未指定玩家相機！");
        }
    }

    // 當物件進入視線
    public void OnEnterView()
    {
        if (!isInView)
        {
            isInView = true;

            if (isHidden)
            {
                // 如果物件被隱藏，進入視線時顯示物件
                ShowObject();
            }

            //Debug.Log($"物件 {name} 進入視線內，不進行移動或隱藏");
        }
    }

    // 當物件離開視線
    public void OnExitView(float teleportChance)
    {
        if (isInView)
        {
            isInView = false;

            //Debug.Log($"物件 {name} 離開視線，嘗試進行隨機行為...");

            // 隨機決定是否進行隱藏或移動
            TryTeleport(teleportChance);
            TryHide();
        }
    }

    // 嘗試移動物件
    private void TryTeleport(float chance)
    {
        if (Random.value < chance)
        {
            Vector3 newPosition;
            int maxAttempts = 10; // 最大嘗試次數
            bool validPosition = false;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // 計算隨機位置
                Vector3 randomOffset = new Vector3(
                    Random.Range(-teleportRange.x, teleportRange.x),
                    Random.Range(-teleportRange.y, teleportRange.y),
                    Random.Range(-teleportRange.z, teleportRange.z)
                );

                newPosition = transform.position + randomOffset;

                // 檢查新位置是否在玩家視線內
                if (!IsInPlayerView(newPosition))
                {
                    //Debug.Log($"物件 {name} 移動成功！新位置為：{newPosition}");
                    transform.position = newPosition;
                    validPosition = true;
                    break;
                }
                else
                {
                    //Debug.Log($"物件 {name} 嘗試的新位置 {newPosition} 在玩家視線內，重新計算...");
                }
            }

            if (!validPosition)
            {
                //Debug.LogWarning($"物件 {name} 無法找到適合的移動位置，移動取消");
            }
        }
        else
        {
            //Debug.Log($"物件 {name} 移動未觸發");
        }
    }

    // 嘗試隱藏物件
    private void TryHide()
    {
        if (!isHidden && Random.value < hideChance)
        {
            //Debug.Log($"物件 {name} 被隱藏！");
            HideObject();
        }
        else
        {
            //Debug.Log($"物件 {name} 隱藏未觸發");
        }
    }

    // 隱藏物件
    private void HideObject()
    {
        isHidden = true;

        // 禁用 MeshRenderer 以隱藏物件，並保留碰撞器功能
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
    }

    // 顯示物件
    private void ShowObject()
    {
        isHidden = false;

        // 啟用 MeshRenderer 以顯示物件
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }

        //Debug.Log($"物件 {name} 已重新顯示");
    }

    // 檢查位置是否在玩家視線內
    private bool IsInPlayerView(Vector3 position)
    {
        Vector3 viewportPoint = playerCamera.WorldToViewportPoint(position);

        // 檢查位置是否在視錐範圍內
        return viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
    }
}
