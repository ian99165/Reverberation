using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float crouchSpeed = 2.5f;
    private CharacterController controller;

    [Header("Camera Settings")]
    public Transform playerCamera;
    public float rotationSpeed = 100f;
    private float xRotation = 0f;
    private Vector2 inputLook;
    private Vector2 inputMove;
    private bool isRunning = false;
    private const float lookThreshold = 0.01f;
    [SerializeField] private GameObject _mainCamera;

    [Header("Interaction Settings")]
    public float interactionDistance = 3f;

    [Header("滑鼠設定")]
    public GameObject mouseState;
    public Image cursor; // 準心圖案
    public Sprite defaultCursor; // 預設準心圖案
    public Sprite interactableCursor;
    public GameObject menubook;

    [Header("機關")]
    public GameObject Lv1;
    public GameObject Lv2;
    
    // Gravity Settings
    private PlayerControls controls;
    public bool _lock;
    public bool _menu;
    public bool _talk;
    public bool _item;
    public bool _view;
    public bool _interactLock;

    private Vector3 velocity; // 角色速度
    private float gravity = -9.81f; // 重力加速度
    private bool isGrounded; // 是否在地面
    
    private InteractionController _interactionController;
    private MouseState _mousestate;
    
    public Inventory inventory;
    private GameObject currentPickedObject;
    
    public void SetInteractLock(bool state)
    {
        _interactLock = state;
    }

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => inputMove = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => inputMove = Vector2.zero;
        controls.Player.Look.performed += ctx => inputLook = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => inputLook = Vector2.zero;
        controls.Player.Run.performed += ctx => isRunning = !isRunning;
        controls.Player.View.performed += ctx => View();
        
        _interactionController = _mainCamera.GetComponent<InteractionController> ();
        _mousestate = mouseState.GetComponent<MouseState>();
        
        
    }

    private void Start()
    {
        _menu = false;
        _talk = false;
        _lock = false;
        _item = false;
        _view = false;
    
        controller = GetComponent<CharacterController>();
    
        menubook.SetActive(false);

        inventory = menubook.GetComponent<Inventory>();
    }


    private void Update()
    {
        Move();
        RotateCamera();
        ApplyGravity();
        UpdateCursor();
    }

    private void Move()
    {
        if (!_menu)
        {
            if (!_talk)
            {
                Vector3 move = transform.right * inputMove.x + transform.forward * inputMove.y;
                float currentSpeed = isRunning ? runSpeed : moveSpeed;

                controller.Move(move * currentSpeed * Time.deltaTime);
            }
        }
    }

    private void RotateCamera()
    {
        if (!_menu)
        {
            if (!_talk)
            {
                if (Mathf.Abs(inputLook.x) > lookThreshold || Mathf.Abs(inputLook.y) > lookThreshold)
                {
                    float mouseX = inputLook.x * rotationSpeed * Time.deltaTime;
                    float mouseY = inputLook.y * rotationSpeed * Time.deltaTime;

                    transform.Rotate(Vector3.up * mouseX);

                    xRotation -= mouseY;
                    xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                    playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                }
            }
        }
    }

    private void ApplyGravity()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
    
    public void OnInteraction(InputValue value)
    { 
        if (!_menu)
        {
            if (!_talk)
            {
                Ray ray = new Ray(playerCamera.position, playerCamera.forward);
                Debug.DrawRay(playerCamera.position, playerCamera.forward * interactionDistance, Color.red, 1f);

                if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
                {
                    switch (hit.collider.tag)
                    {
                        case "Item":
                            SoundManager.Instance.PlaySound(SoundManager.Instance.pickUp);
                            _interactionController.IsPickup = !_interactionController.IsPickup;
                            if (_item) { _item = false; return; }
                            _item = true;
                            currentPickedObject = hit.collider.gameObject; // 記錄當前拾取物件
                            break;
                        case "NPC":
                            var npcInteractionScript = hit.collider.GetComponent<NPCInteraction>();
                            if (npcInteractionScript != null)
                            {
                                npcInteractionScript.Interact_NPC();
                            }
                            break;
                        case "Devices":
                            var devicesInteractionScript = hit.collider.GetComponent<DevicesInteraction>();
                            if (devicesInteractionScript != null)
                            {
                                devicesInteractionScript.Interact_Devices();
                            }
                            break;
                        case "EleButton":
                            var eleButton = hit.collider.GetComponent<EleInteraction>();
                            if (eleButton != null)
                            {
                                eleButton.Interact_Devices();
                            }
                            break;
                        case "lock":
                            GameObject targetObj = null;
                            switch (hit.collider.gameObject.name)
                            {
                                case "Lv1":
                                    targetObj = Lv1;
                                    break;
                                case "Lv2":
                                    targetObj = Lv2;
                                    break;
                            }

                            if (targetObj != null)
                            {
                                targetObj.SetActive(true);
                                CantMove();
                                _mousestate.MouseMode_II();
                                SoundManager.Instance.PlaySound(SoundManager.Instance.window);
                            }
                            break;
                        case "Reverberation": 
                            switch (hit.collider.gameObject.name)
                            {
                                case "APT_1":
                                case "APT_2":
                                case "APT_3":
                                    Reverberation.Instance.SetAPTState(hit.collider.gameObject.name, true);
                                    Reverberation.Instance.fragments();
                                    break;
                            }
                            Destroy(hit.collider.gameObject);
                            Debug.Log("記憶碎片");
                            break;

                        case "Key":
                            //inventory.AddItem("Key");
                            Destroy(hit.collider.gameObject);
                            var reverberation= hit.collider.GetComponent<Reverberation>();
                            if (reverberation != null)
                            {
                                reverberation.fragments();
                            }
                            SoundManager.Instance.PlaySound(SoundManager.Instance.pickUp);
                            break;
                        case "Clockwork":
                            //inventory.AddItem("Clockwork");
                            Destroy(hit.collider.gameObject);
                            var npcInteraction= Lv2.GetComponent<NPCInteraction>();
                            npcInteraction.switchState();
                            SoundManager.Instance.PlaySound(SoundManager.Instance.pickUp);
                            break;
                        case "Pages":
                            //inventory.AddItem("Pages");
                            Destroy(hit.collider.gameObject);
                            SoundManager.Instance.PlaySound(SoundManager.Instance.pickUp);
                            break;
                        case "SavePoint":
                            break;
                    }
                }
            }
        }
    }

    public void OnMenu(InputValue value)
    {
        if(_interactLock) return;
        SoundManager.Instance.PlaySound(SoundManager.Instance.window);
        if (!_lock)
        {
            if (!_talk)
            {
                if (!_menu)
                {
                    _menu = true;
                    _mousestate.MouseMode_II();
                    menubook.SetActive(true);
                    return;
                }
                else
                {
                    _menu = false;
                    _mousestate.MouseMode_I();
                    menubook.SetActive(false);
                    return;
                }
            }
        }

        if (_lock)
        {
            CanMove();
            _mousestate.MouseMode_I();
            if (Lv1 != null) Lv1.SetActive(false);

            GameObject lockObject = GameObject.Find("lock");
            if (lockObject != null)
            {
                var rotatable = lockObject.GetComponent<RotatableObject>();
                if (rotatable != null)
                {
                    rotatable.enabled = true;
                }
            }
        }
        
        if (_view)
        {
            _mousestate.MouseMode_I();
            _item = true;
            _view = false;
            CanMove();

            // **禁用當前拾取物件的 RotatableObject 腳本**
            if (currentPickedObject != null)
            {
                var rotatable = currentPickedObject.GetComponent<RotatableObject>();
                if (rotatable != null)
                {
                    rotatable.enabled = false;
                }
            }
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void UpdateCursor()
    {
        if (!_view)
        {
            if (!_item)
            {
                if (!_menu)
                {
                    if (!_talk)
                    {
                        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

                        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
                        {
                            // 防止未定義的 Tag 報錯
                            if (IsInteractableTag(hit.collider.tag))
                            {
                                Debug.Log("可互動");
                                cursor.sprite = interactableCursor; // 更改為可互動圖案
                            }
                            else
                            {
                                cursor.sprite = defaultCursor; // 恢復為預設圖案
                            }
                        }
                        else
                        {
                            cursor.sprite = defaultCursor; // 恢復為預設圖案
                        }
                    }
                }
            }
        }
    }
    
    private void View()
    {
        if (_view)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.window);
            _mousestate.MouseMode_I();
            _item = true;
            _view = false;
            CanMove();

            // **禁用當前拾取物件的 RotatableObject 腳本**
            if (currentPickedObject != null)
            {
                var rotatable = currentPickedObject.GetComponent<RotatableObject>();
                if (rotatable != null)
                {
                    rotatable.enabled = false;
                }
            }
            return;
        }
        if(!_menu)
        {
            if (_item)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.window);
                _mousestate.MouseMode_II();
                _item = false;
                _view = true;
                CantMove();

                // 啟用當前拾取物件的 RotatableObject 腳本
                if (currentPickedObject != null)
                {
                    var rotatable = currentPickedObject.GetComponent<RotatableObject>();
                    if (rotatable != null)
                    {
                        rotatable.enabled = true;
                    }
                }
            }
        }
    }

    public void CantMove()
    {
        _menu = true;
        _lock = true;
        _talk = true;
    }

    public void CanMove()
    {
        _menu = false;
        _lock = false;
        _talk = false;
    }

    // 判斷是否為可互動的 Tag
    private bool IsInteractableTag(string tag)
    {
        return tag == "Item" || tag == "NPC" ||
               tag == "Devices" || tag == "Key" ||
               tag == "Door" || tag == "SavePoint"|| 
               tag == "Clockwork"|| tag == "Pages"|| tag == "Reverberation" ||
               tag == "lock" || tag == "EleButton";
               ;
    }

    public void TalkingSet()
    {
        if (!_talk)
        {
            _talk = true;
            return;
        }
        else
        {
            _talk = false;
        }
    }
}
