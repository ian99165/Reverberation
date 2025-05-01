using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractionHandler : MonoBehaviour
{
    public bool isDoorClosed = false;
    public MouseState _mousestate;

    public bool isPlayerInRange = false;
    public bool S1toS2 = false;

    public Collider playerCollider;

    void Update()
    {
        if (!S1toS2)
        {
            if (isDoorClosed && isPlayerInRange)
            {
                S1toS2 = true;
                ExecuteAction();
            }
        }
    }

    public void SetDoorClosed(bool isClosed)
    {
        isDoorClosed = isClosed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == playerCollider)
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other == playerCollider)
        {
            isPlayerInRange = false;
        }
    }

    private void ExecuteAction()
    {
        Debug.Log("門已關閉，玩家在範圍內，執行指令");
        _mousestate.MouseMode_II();
        SceneManager.LoadScene("S2");
    }
}