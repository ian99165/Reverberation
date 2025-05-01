using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Reverberation : MonoBehaviour
{
    public static Reverberation Instance { get; private set; } // 單例模式

    public GameObject playerPrefs;
    public GameObject Videoplayer;
    private DelayedExecutor executor;

    private Dictionary<string, bool> APTStates = new Dictionary<string, bool>
    {
        { "APT_1", false },
        { "APT_2", false },
        { "APT_3", false }
    };

    void Start()
    {
        if (playerPrefs == null)
        {
            Debug.LogError("playerPrefs 未正確設置，請確保已在 Inspector 中賦值");
        }

        executor = FindObjectOfType<DelayedExecutor>();
        if (executor == null)
        {
            Debug.LogWarning("DelayedExecutor 未找到，請確認場景中是否存在該腳本的物件");
        }

        LoadData();
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void fragments()
    {
        foreach (var key in APTStates.Keys)
        {
            Debug.Log($"{key}: {APTStates[key]}");
        }
        
        SaveData(); // 存檔

        if (APTStates["APT_1"] && APTStates["APT_2"] && APTStates["APT_3"] && !playerPrefs.activeSelf)
        {
            playerPrefs.SetActive(true);
            Videoplayer.SetActive(true);

            if (executor != null)
            {
                executor.ExecuteWithDelay(69f, () => DeactivateObjects());
            }
            else
            {
                Debug.LogWarning("DelayedExecutor 未找到");
            }
        }
    }
    
    void DeactivateObjects()
    {
        playerPrefs.SetActive(false);
        Videoplayer.SetActive(false);
    }

    public void SetAPTState(string aptName, bool state)
    {
        if (APTStates.ContainsKey(aptName))
        {
            APTStates[aptName] = state;
            SaveData();
        }
        else
        {
            Debug.LogWarning($"無效的 APT 名稱: {aptName}");
        }
    }

    public void SaveData()
    {
        foreach (var key in APTStates.Keys)
        {
            PlayerPrefs.SetInt(key, APTStates[key] ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        var keys = APTStates.Keys.ToList(); // 避免 `foreach` 修改集合
        foreach (var key in keys)
        {
            APTStates[key] = PlayerPrefs.GetInt(key, 0) == 1;
        }
    }
    
    public void ResetAPTStates()
    {
        // 重置 APT 狀態
        APTStates["APT_1"] = false;
        APTStates["APT_2"] = false;
        APTStates["APT_3"] = false;
        SaveData(); // 儲存重置後的狀態
    }

}
