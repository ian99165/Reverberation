using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    public Transform target;
    public float floatSpeed = 1f;
    public float floatAmplitude = 0.2f;

    private Vector3 offset;

    void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;

            float newY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

            transform.position = new Vector3(targetPosition.x, targetPosition.y + newY, targetPosition.z);
        }
    }
}