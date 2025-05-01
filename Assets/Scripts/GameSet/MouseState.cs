using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseState : MonoBehaviour
{
    public int _mouse_state = 0 ;
    public GameObject CanvaMode_I;
    public GameObject CanvaMode_II;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void MouseMode_I()
    {
        _mouse_state = 0;
        Mode_state();
        CanvaMode_I.SetActive(true);
        CanvaMode_II.SetActive(false);
    }

    public void MouseMode_II()
    {
        _mouse_state = 1;
        Mode_state();
        CanvaMode_I.SetActive(false);
        CanvaMode_II.SetActive(true);
    }

    public void Mode_state()
    {
        if (_mouse_state == 0) //探索模式
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (_mouse_state == 1) //鼠鍵模式
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
