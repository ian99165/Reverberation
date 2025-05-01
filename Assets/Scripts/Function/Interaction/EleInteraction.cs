using System.Collections;
using UnityEngine;

public class EleInteraction : MonoBehaviour
{
    public string Name = "p";
    public string ObjectName;
    private bool _can_move = true;
    private bool _open;
    
    public GameObject EleDoorR;
    public GameObject EleDoorL;

    [Header("移動秒數")]
    public float movementDuration = 0.5f;

    [Header("停留秒數")]
    public float stayDuration = 0.5f;

    [Header("移動速度")]
    public float speed = 0.01f;
    
    public InteractionHandler interactionHandler;

    public void Interact_Devices()
    {
        if (!_can_move) return; // 如果正在執行動作，不允許再次觸發

        switch (Name)
        {
            case "r":
                StartCoroutine(Move_Button(Vector3.right));
                break;
            case "p":
                StartCoroutine(Move_Button(Vector3.forward));
                break;
            case "u":
                StartCoroutine(Move_Button(Vector3.up));
                break;
        }
    }

    private IEnumerator Move_Button(Vector3 direction)
    {
        _can_move = false;
        _open = true;
        openEle();
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + direction * speed * movementDuration;

        yield return StartCoroutine(Move(startPosition, targetPosition));

        yield return new WaitForSeconds(stayDuration);

        _open = false;
        if (ObjectName == "door") openEle();
        yield return StartCoroutine(Move(targetPosition, startPosition));

        _can_move = true;

        // 延遲 15 秒後改變門的狀態
        StartCoroutine(ChangeDoorStateAfterDelay(15f));
    }

    private IEnumerator Move(Vector3 start, Vector3 end)
    {
        float elapsedTime = 0f;

        while (elapsedTime < movementDuration)
        {
            transform.position = Vector3.Lerp(start, end, elapsedTime / movementDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
    }

    public void openEle()
    {
        switch (ObjectName)
        {
            case "open":
                if (EleDoorL != null)
                {
                    var eleButtonR = EleDoorL.GetComponent<EleInteraction>();
                    if (eleButtonR != null) eleButtonR.Interact_Devices();
                }

                if (EleDoorR != null)
                {
                    var eleButtonL = EleDoorR.GetComponent<EleInteraction>();
                    if (eleButtonL != null) eleButtonL.Interact_Devices();
                }
                SoundManager.Instance.PlaySound(SoundManager.Instance.eleButton);
                break;
            case "door":
                if (_open) SoundManager.Instance.PlaySound(SoundManager.Instance.eleDoorOpened);
                if (!_open) SoundManager.Instance.PlaySound(SoundManager.Instance.eleDoorClosed);
                break;
        }
    }

    private IEnumerator ChangeDoorStateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (interactionHandler != null)
        {
            interactionHandler.SetDoorClosed(!_open);
        }
    }
}
