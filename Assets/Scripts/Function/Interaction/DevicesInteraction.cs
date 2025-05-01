using System.Collections;
using UnityEngine;

public class DevicesInteraction : MonoBehaviour
{
    // 物件狀態變數
    private bool _canMove = true; // 是否可移動
    private bool _isOpen = false; // 物件是否開啟
    public bool _isLocked; // 是否鎖定

    [Header("物件名稱")]
    public string Name = "Name";  // 物件類型
    public string ObjectName;     // 物件的名稱（用於音效）

    [Header("控制參數")]
    public float movementDuration = 3f;  // 移動持續時間
    public float speed = 1f;             // 移動速度
    public float rotationSpeed = 45f;    // 旋轉速度（每秒角度）

    private void Start()
    {
        _canMove = true;
        _isOpen = false;
    }

    /// <summary>
    /// 主要互動函數
    /// </summary>
    public void Interact_Devices()
    {
        PlayLockAudio();
        if (_isLocked) return;

        switch (Name)
        {
            case "M_u": StartCoroutine(ToggleMove(Vector3.up, Vector3.down)); break;
            case "M_p": StartCoroutine(ToggleMove(Vector3.forward, Vector3.back)); break;
            case "M_r": StartCoroutine(ToggleMove(Vector3.right, Vector3.left)); break;
            case "R_u": StartCoroutine(ToggleRotate(Vector3.right)); break;
            case "R_r": StartCoroutine(ToggleRotate(Vector3.up)); break;
            default: Debug.Log("未定義的互動行為"); break;
        }
    }

    /// <summary>
    /// 切換移動狀態（開 → 關）
    /// </summary>
    private IEnumerator ToggleMove(Vector3 openDirection, Vector3 closeDirection)
    {
        if (!_canMove) yield break; // 加入這行防止重複點擊問題

        if (_isOpen)
        {
            yield return Move(closeDirection);
            _isOpen = false;
        }
        else
        {
            yield return Move(openDirection);
            _isOpen = true;
        }
    }



    /// <summary>
    /// 通用的移動函數
    /// </summary>
    private IEnumerator Move(Vector3 direction)
    {
        if (!_canMove) yield break;
        PlayAudio();
        
        _canMove = false;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + direction * speed * movementDuration;

        float elapsedTime = 0f;
        while (elapsedTime < movementDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / movementDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        _canMove = true;
    }

    /// <summary>
    /// 切換旋轉狀態（開 → 關）
    /// </summary>
    private IEnumerator ToggleRotate(Vector3 rotationAxis)
    {
        if (!_canMove) yield break; // 防止重複點擊問題

        float rotationAngle = _isOpen ? -rotationSpeed * movementDuration : rotationSpeed * movementDuration;
        yield return Rotate(rotationAxis, rotationAngle);
        _isOpen = !_isOpen;
    }



    /// <summary>
    /// 通用的旋轉函數
    /// </summary>
    private IEnumerator Rotate(Vector3 axis, float angle)
    {
        if (!_canMove) yield break;

        _canMove = false;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(axis * angle);

        float elapsedTime = 0f;
        while (elapsedTime < movementDuration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / movementDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;

        _canMove = true;
    }

    /// <summary>
    /// 解鎖物件，使其可互動
    /// </summary>
    public void Unlock()
    {
        _isLocked = false;
    }

    /// <summary>
    /// 播放對應音效
    /// </summary>
    private void PlayAudio()
    {
        if (SoundManager.Instance == null) return;

        switch (ObjectName)
        {
            case "button":
                SoundManager.Instance.PlaySound(SoundManager.Instance.eleButton);
                break;

            case "Door":
                if (_isLocked)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.lockDoor);
                }
                else
                {
                    SoundManager.Instance.PlaySound(_isOpen ? SoundManager.Instance.closeDoor : SoundManager.Instance.openDoor);
                }
                break;

            case "Cabinet":
                if (_isLocked)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.lockCabinet);
                }
                else
                {
                    SoundManager.Instance.PlaySound(_isOpen ? SoundManager.Instance.closeCabinet : SoundManager.Instance.openCabinet);
                }
                break;
            case "CabinetMove":
                SoundManager.Instance.PlaySound(SoundManager.Instance.moveCabinet);
                break;
            case "CabinetDrawer":
                SoundManager.Instance.PlaySound(_isOpen ? SoundManager.Instance.closeCabinetDrawer : SoundManager.Instance.openCabinetDrawer);
                break;

            case "Drawer":
                SoundManager.Instance.PlaySound(_isOpen ? SoundManager.Instance.closeDrawer : SoundManager.Instance.openDrawer);
                break;
        }
    }
    private void PlayLockAudio()
    {
        if (SoundManager.Instance == null) return;

        switch (ObjectName)
        {
            case "Door":
                if (_isLocked)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.lockDoor);
                }
                break;
            case "Cabinet":
                if (_isLocked)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.lockCabinet);
                }
                break;
        }
    }
}
