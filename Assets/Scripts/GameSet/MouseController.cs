using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;


public class MouseController : MonoBehaviour
{
    [Header("滑鼠控制")]
    public RectTransform virtualCursor; // UI 虛擬滑鼠
    public Canvas canvas; // Canvas
    public float cursorSpeed = 1000f; // 手柄移動速度

    private Vector2 cursorPos; // 虛擬滑鼠座標
    private PlayerControls controls; // 輸入系統
    private InputMode currentMode = InputMode._joy_mod; // 預設為手柄模式

    private enum InputMode { _key_mod, _joy_mod } // 輸入模式（鍵鼠/手柄）

    public GameObject CameraToHome;
    public GameObject CameraToStart;

    [Header("之後要改zzz")]
    public GameObject 隱藏打開1;
    public GameObject 隱藏打開2;
    private void Awake()
    {
        controls = new PlayerControls();
        controls.Enable();

        // 監聽鍵盤/滑鼠輸入，切換模式
        controls.UI.anyKey.performed += _ => SetMode(InputMode._key_mod);
        controls.UI.anyJoy.performed += _ => SetMode(InputMode._joy_mod);

        // 初始化虛擬滑鼠位置
        cursorPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    private void Update()
    {
        if (currentMode == InputMode._joy_mod)
        {
            // 手柄控制虛擬滑鼠
            Vector2 moveInput = controls.Player.AsMouse.ReadValue<Vector2>();
            cursorPos += moveInput * cursorSpeed * Time.deltaTime;
            cursorPos = new Vector2(
                Mathf.Clamp(cursorPos.x, 0, Screen.width),
                Mathf.Clamp(cursorPos.y, 0, Screen.height)
            );
        }
        else
        {
            // 讓 UI 滑鼠跟隨真實滑鼠
            cursorPos = Input.mousePosition;
        }

        UpdateCursorPos();

        // 偵測按鍵點擊（滑鼠左鍵 或 手柄點擊鍵）
        if (controls.Player.Click.triggered || Mouse.current.leftButton.wasPressedThisFrame)
        {
            SimulateMouseClick();
        }
    }

    private void UpdateCursorPos()
    {
        if (canvas && virtualCursor)
        {
            Vector2 localCursorPos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                cursorPos,
                canvas.worldCamera,
                out localCursorPos))
            {
                virtualCursor.anchoredPosition = localCursorPos;
            }
        }
    }

    private void SimulateMouseClick()
    {
        // 確保 Camera.main 不為 null
        if (Camera.main == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        // 確保 cursorPos 被初始化
        if (cursorPos == null)
        {
            Debug.LogError("cursorPos is not initialized!");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(cursorPos);
    
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log(hit.collider.gameObject.tag);
            Debug.Log(hit.collider.gameObject.name);

            if (hit.collider.CompareTag("UI_Menu"))
            {
                switch (hit.collider.name)
                {
                    case "Button_Back":
                        隱藏打開1.SetActive(false);
                        隱藏打開2.SetActive(false);
                        break;
                    case "Button_Exit_Q":
                        隱藏打開1.SetActive(true);
                        隱藏打開2.SetActive(true);
                        break;
                    case "Button_Start":
                        Button_Start();
                        break;
                    case "Button_Exit":
                        Button_Exit();
                        break;
                    case "Button_Reset":
                        SceneManager.LoadScene("S0");
                        break;
                    default:
                        Debug.Log("無");
                        break;
                }
            }
            else if (hit.collider.CompareTag("UI_Item"))
            {
                Debug.Log("拾取道具");
                // inventory.DropItem(hit.collider.gameObject); // 物品處理
            }
            else if (hit.collider.CompareTag("UI_Object"))
            {
                Debug.Log("觸發互動");
                var lockInteraction = hit.collider.GetComponent<LockInteraction>();
                if (lockInteraction != null)
                {
                    lockInteraction.Interact_Devices();
                }
            }
        }
    }


    private void SetMode(InputMode mode)
    {
        if (currentMode != mode)
        {
            currentMode = mode;
        }
    }
    
    private void Button_Start()
    {
        Reverberation.Instance.ResetAPTStates();
        CameraToHome.SetActive(false);
        StartCoroutine(WaitAndDisableHomeCamera());
    }

    private IEnumerator WaitAndDisableHomeCamera()
    {
        yield return new WaitForSeconds(2.1f);

        CameraToStart.SetActive(false);
        SceneManager.LoadScene("S1");
    }

    private void Button_Exit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
