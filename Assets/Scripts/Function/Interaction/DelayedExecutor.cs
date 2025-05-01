using System.Collections;
using UnityEngine;

public class DelayedExecutor : MonoBehaviour
{
    public void ExecuteWithDelay(float delay, System.Action action)
    {
        StartCoroutine(DelayCoroutine(delay, action));
    }

    private IEnumerator DelayCoroutine(float delay, System.Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}