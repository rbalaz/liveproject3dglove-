using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TouchFingerType
{
    ThumbDistal = 0,
    IndexDistal = 1,
    MidleDistal = 2,
    RingDistal  = 3,
    PinkyDistal = 4,
    Other
}



public class TouchFinger
{
    public TouchFingerType fingerType;
    public bool touching = false;
}

public class TouchDetection : MonoBehaviour {

    public delegate void TouchValueChanged(TouchFingerType fingerType, bool isLeftHand, bool touching);
    public static event TouchValueChanged OnTouchValueChange;

    public static TouchFinger[] touchFingersLeft;
    public static TouchFinger[] touchFingersRight;

    private static TouchDetection _instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (_instance == null)
        {
            _instance = new GameObject("TouchDetection").AddComponent<TouchDetection>();
            DontDestroyOnLoad(_instance.gameObject);
            touchFingersLeft = new TouchFinger[] {
                new TouchFinger { fingerType = TouchFingerType.ThumbDistal },
                new TouchFinger { fingerType = TouchFingerType.IndexDistal },
                new TouchFinger { fingerType = TouchFingerType.MidleDistal },
                new TouchFinger { fingerType = TouchFingerType.RingDistal },
                new TouchFinger { fingerType = TouchFingerType.PinkyDistal }
            };
            touchFingersRight = new TouchFinger[] {
                new TouchFinger { fingerType = TouchFingerType.ThumbDistal },
                new TouchFinger { fingerType = TouchFingerType.IndexDistal },
                new TouchFinger { fingerType = TouchFingerType.MidleDistal },
                new TouchFinger { fingerType = TouchFingerType.RingDistal },
                new TouchFinger { fingerType = TouchFingerType.PinkyDistal }
            };
        }
    }

    public static void UpdateFinger(TouchFingerType fingerType, bool isLeftHand, bool touching)
    {
        var hand = isLeftHand ? touchFingersLeft : touchFingersRight;

        // If array contains element with such index
        if ((int)fingerType < hand.Length)
        {
            // Value has changed
            if (hand[(int)fingerType].touching != touching)
            {
                hand[(int)fingerType].touching = touching;
                OnTouchValueChange(fingerType, isLeftHand, touching);
            }
        }
        else
        {
            Debug.LogError("UpdateFinger Error: fingerType=" + fingerType + ", isLeftHand=" + isLeftHand + ", touching=" + touching);
        }
    }
}
