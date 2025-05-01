using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotatableObject : MonoBehaviour
{
    public float rotationSpeed = 300f;
    public float mouseSpeed = 100f;
    public InputActionReference moveAction;
    private Vector3 lastMousePosition;

    void Update()
    {
        if (gameObject.CompareTag("Item"))
        {
            RotateWithMouse();
            RotateWithStick();
        }
    }

    public void RotateWithMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotationX = delta.x * mouseSpeed * Time.deltaTime;
            float rotationY = delta.y * mouseSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up, -rotationX, Space.World);
            transform.Rotate(Vector3.right, rotationY, Space.World);

            lastMousePosition = Input.mousePosition;
        }
    }

    public void RotateWithStick()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        float rotationX = input.x * rotationSpeed * Time.deltaTime;
        float rotationY = input.y * rotationSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up, rotationX, Space.World);
        transform.Rotate(Vector3.right, -rotationY, Space.World);
    }
}
