using System.Collections;
using UnityEngine;

public class LockInteraction : MonoBehaviour
{
    public string Name = "Name";
    public string Number = "Number";
    public LockPassword lockPassword;
    
    private bool _can_move;

    private bool _isInteracting = false;
    
    public FirstPersonController _playerController;
    
    [Header("秒數")]
    public float movementDuration = 3f;
    [Header("移動速度")]
    public float speed = 1f;
    [Header("旋轉速度")]
    public float rotationSpeed = 45f;

    private void Start()
    {
        _can_move = true;
        
        _playerController = FindObjectOfType<FirstPersonController>();
    }

    public void Interact_Devices()
    {
        if (_isInteracting) return;
        SoundManager.Instance.PlaySound(SoundManager.Instance.lockTurn);
        
        if (_playerController != null)
        {
            _playerController.SetInteractLock(true); // 鎖住互動
        }
        
        StartCoroutine(PerformInteraction());
    }

    private IEnumerator PerformInteraction()
    {
        _isInteracting = true;

        switch (Name)
        {
            case "M_u":
                SwitchPass();
                yield return StartCoroutine(Move_Up());
                break;
            case "M_p":
                SwitchPass();
                yield return StartCoroutine(Move_Forward());
                break;
            case "M_r":
                SwitchPass();
                yield return StartCoroutine(Move_Right());
                break;
            case "R_u":
                SwitchPass();
                yield return StartCoroutine(Rotate_Up());
                break;
            case "R_r":
                SwitchPass();
                yield return StartCoroutine(Rotate_Right());
                break;
            default:
                Debug.Log("No action executed.");
                yield break;
        }

        if (lockPassword != null)
        {
            lockPassword.inspection();
        }

        _isInteracting = false;

        if (_playerController != null)
        {
            _playerController.SetInteractLock(false); // 解鎖互動
        }
    }

    private IEnumerator Move(Vector3 direction)
    {
        if (_can_move)
        {
            _can_move = false;
            float elapsedTime = 0f;
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + direction * speed * movementDuration;

            while (elapsedTime < movementDuration)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / movementDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            _can_move = true;
        }
    }

    public IEnumerator Move_Up() { yield return StartCoroutine(Move(Vector3.up)); }
    public IEnumerator Move_Right() { yield return StartCoroutine(Move(Vector3.right)); }
    public IEnumerator Move_Forward() { yield return StartCoroutine(Move(Vector3.forward)); }

    // **旋轉函數**
    private IEnumerator Rotate(Vector3 rotationAxis, float angle)
    {
        if (_can_move)
        {
            _can_move = false;
            float elapsedTime = 0f;
            Quaternion startRotation = transform.rotation;
            Quaternion targetRotation = startRotation * Quaternion.Euler(rotationAxis * angle);

            while (elapsedTime < movementDuration)
            {
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / movementDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.rotation = targetRotation;
            _can_move = true;
        }
    }

    public IEnumerator Rotate_Right() { yield return StartCoroutine(Rotate(Vector3.up, 36f)); }
    public IEnumerator Rotate_Up() { yield return StartCoroutine(Rotate(Vector3.right, -rotationSpeed * movementDuration)); }
    
    void SwitchPass()
    {
        if (lockPassword != null)
        {
            switch (Number)
            {
                case "1":
                    lockPassword.One = IncrementWithReset(lockPassword.One, lockPassword.maxValue);
                    break;
                case "2":
                    lockPassword.Two = IncrementWithReset(lockPassword.Two, lockPassword.maxValue);
                    break;
                case "3":
                    lockPassword.Three = IncrementWithReset(lockPassword.Three, lockPassword.maxValue);
                    break;
                case "4":
                    lockPassword.Four = IncrementWithReset(lockPassword.Four, lockPassword.maxValue);
                    break;
                case "5":
                    lockPassword.Five = IncrementWithReset(lockPassword.Five, lockPassword.maxValue);
                    break;
            }
        }
    }

    // 遞增並檢查是否達到最大值，若達到則重置為 0
    private int IncrementWithReset(int value, int maxValue)
    {
        value++;
        if (value > maxValue)
        {
            value = 0;
        }
        return value;
    }
}
