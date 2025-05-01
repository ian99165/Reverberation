using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchState : MonoBehaviour
{
    public string Name;

    public void switchState()
    {
        switch (Name)
        {
            case "1":
                SetKinematic(); // 直接對當前物件執行
                break;
        }
    }

    void SetKinematic()
    {
        Rigidbody rb = GetComponent<Rigidbody>(); // 取得當前物件的 Rigidbody
        if (rb != null)
        {
            rb.isKinematic = false; // 設定 Is Kinematic
        }
    }
}