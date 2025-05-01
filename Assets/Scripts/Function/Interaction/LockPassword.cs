using UnityEngine;

public class LockPassword : MonoBehaviour
{
    [Header("解鎖物件")]
    public GameObject Object1;
    public GameObject Object2;

    [Header("密碼")]
    public int one;
    public int two;
    public int three;
    public int four;
    public int five;

    public int _one;
    public int _two;
    public int _three;
    public int _four;
    public int _five;

    private DevicesInteraction devicesInteraction;
    private SwitchState switchState;

    public int minValue = 0;
    public int maxValue = 9;
    
    public MouseState _mousestate;
    public FirstPersonController _firstPersonController;

    public int One
    {
        get { return _one; }
        set { _one = Mathf.Clamp(value, minValue, maxValue); }
    }

    public int Two
    {
        get { return _two; }
        set { _two = Mathf.Clamp(value, minValue, maxValue); }
    }

    public int Three
    {
        get { return _three; }
        set { _three = Mathf.Clamp(value, minValue, maxValue); }
    }

    public int Four
    {
        get { return _four; }
        set { _four = Mathf.Clamp(value, minValue, maxValue); }
    }

    public int Five
    {
        get { return _five; }
        set { _five = Mathf.Clamp(value, minValue, maxValue); }
    }

    public void Start()
    {
        _one = 0;
        _two = 0;
        _three = 0;
        _four = 0;
        _five = 0;

        if (Object1 != null)
        {
            devicesInteraction = Object1.GetComponent<DevicesInteraction>();
            if (devicesInteraction == null)
            {
                Debug.LogWarning("DevicesInteraction 腳本不存在於指定的物件上！");
            }
        }
        if (Object2 != null)
        {
            switchState = Object2.GetComponent<SwitchState>();
            if (devicesInteraction == null)
            {
                Debug.LogWarning("DevicesInteraction 腳本不存在於指定的物件上！");
            }
        }
        else
        {
            Debug.LogWarning("Object2 尚未設定！");
        }
    }

    public void inspection()
    {
        bool conditionMet = false;
        conditionMet = (_one == one && _two == two && three == _three && four == _four && _five == five);

        if (conditionMet && devicesInteraction != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.lockFall);
            devicesInteraction.Unlock();
            switchState.switchState();
            _mousestate.MouseMode_I();
            _firstPersonController.CanMove();
            Destroy(gameObject);
        }
    }
}
