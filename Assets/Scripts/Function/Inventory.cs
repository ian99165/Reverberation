using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("")] 
    public Transform itemsContainer; // 物品欄容器
    public Transform player;
    public Transform target;
    public float dropDistance = 1.5f; 
    
    private List<GameObject> itemList = new List<GameObject>(); // 存儲物品的列表
    private Vector3 startPosition = new Vector3(0, 0.13f, 0); // 物品欄初始位置
    private Vector3 lastItemPosition; // 記錄上一個物品的位置
    private float itemSpacing = 0.07f; // 物品之間的水平間距
    private float rowSpacing = -0.06f; // 物品之間的垂直間距
    private int maxColumns = 3; // 每列最多 3 個圖標
    private int currentColumn = 0; // 當前行內的物品數量

    
    [Header("道具欄")]
    public GameObject Key;
    public GameObject Clockwork;
    public GameObject Pages;

    [Header("生成道具")] 
    public GameObject Key3D;
    public GameObject Clockwork3D;
    public GameObject Pages3D;


    // 物品名稱與對應圖示的映射
    private Dictionary<string, GameObject> itemIcons;
    private Dictionary<string, GameObject> itemPrefabs; // 3D 物品對應表

    void Start()
    {
        if (itemsContainer == null)
        {
            Debug.LogError("itemsContainer 未設置！請在 Inspector 中綁定物品欄容器。");
        }

        lastItemPosition = startPosition;

        itemIcons = new Dictionary<string, GameObject>
        {
            { "Key", Key },
            { "Clockwork", Clockwork },
            { "Pages", Pages }
        };

        itemPrefabs = new Dictionary<string, GameObject>
        {
            { "Key", Key3D },
            { "Clockwork", Clockwork3D },
            { "Pages", Pages3D }
        };

        // 確保字典內的物品不為 null
        foreach (var key in itemIcons.Keys)
        {
            if (itemIcons[key] == null)
            {
                Debug.LogError($"物品 {key} 未正確綁定 UI 圖標！");
            }
        }

        foreach (var key in itemPrefabs.Keys)
        {
            if (itemPrefabs[key] == null)
            {
                Debug.LogError($"物品 {key} 未正確綁定 3D 物件！");
            }
        }
    }

    public void AddItem(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogWarning("itemName 不能為空！");
            return;
        }

        if (!itemIcons.ContainsKey(itemName))
        {
            Debug.LogWarning($"找不到 {itemName} 對應的 UI 物品圖示！");
            return;
        }

        GameObject prefab = itemIcons[itemName];
        if (prefab == null)
        {
            Debug.LogError($"物品 {itemName} 的圖示 Prefab 未設定，請檢查 Inspector！");
            return;
        }

        if (itemsContainer == null)
        {
            Debug.LogError("itemsContainer 為空，請在 Inspector 綁定！");
            return;
        }

        // 創建對應的 UI 物品圖示
        GameObject newItem = Instantiate(prefab, itemsContainer);
        newItem.transform.localPosition = lastItemPosition;

        // 插入物品到列表
        itemList.Insert(0, newItem);

        // 更新物品欄顯示
        UpdateInventoryPositions();
    }


    public void DropItem(GameObject selectedItem)
    {
        Debug.Log($"準備丟棄物品: {selectedItem.name}");

        if (!selectedItem)
        {
            Debug.LogWarning("選中的物品為空，無法丟棄");
            return;
        }

        // 取得物品名稱
        string itemName = selectedItem.name.Replace("(Clone)", "").Trim();

        // 確保有對應的 3D 預製物
        if (!itemPrefabs.ContainsKey(itemName))
        {
            Debug.LogWarning($"找不到對應的 3D 物品預製物: {itemName}");
            return;
        }

        // 確保 Target 物件已經指定
        if (target == null)
        {
            Debug.LogWarning("請在 Inspector 中手動指定 Target 物件");
            return;
        }

        // 直接刪除當前物品
        itemList.Remove(selectedItem);
        Destroy(selectedItem);
        Debug.Log("已刪除物品");

        // 生成對應的 3D 物品於 Target 位置
        Vector3 dropPosition = target.position;
        Instantiate(itemPrefabs[itemName], dropPosition, Quaternion.identity);
        Debug.Log($"成功丟棄 {itemName}，生成於 {dropPosition}");

        // 重新更新物品欄位置
        UpdateInventoryPositions();
    }

    private void UpdateInventoryPositions()
    {
        // 清空原來的位置
        lastItemPosition = startPosition;
        currentColumn = 0;

        // 遍歷物品列表，重新計算每個物品的位置
        foreach (var item in itemList)
        {
            item.transform.localPosition = lastItemPosition;

            // 更新行內物品數量
            currentColumn++;

            if (currentColumn >= maxColumns)
            {
                lastItemPosition = new Vector3(startPosition.x, lastItemPosition.y + rowSpacing, startPosition.z);
                currentColumn = 0;
            }
            else
            {
                lastItemPosition += new Vector3(itemSpacing, 0, 0);
            }
        }
    }


    public void ClearInventory()
    {
        foreach (var item in itemList)
        {
            Destroy(item);
        }
        itemList.Clear();
        lastItemPosition = startPosition;
        currentColumn = 0;
    }
}