using Leap.Unity.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Sliders
{
    public Slider sliderThumb;
    public Slider sliderIndex;
    public Slider sliderMiddle;
    public Slider sliderRing;
    public Slider sliderPinky;
}

public class TouchSliders : MonoBehaviour
{
    [Tooltip("Sliders for left hand fingers")]
    public Sliders leftHandSliders;
    [Tooltip("Sliders for right hand fingers")]
    public Sliders rightHandSliders;

    private Slider[] leftSliders;
    private Slider[] rightSliders;

    public void Start()
    {
        TouchDetection.OnTouchValueChange += ValueChanged;

        leftSliders = new Slider[] {
            leftHandSliders.sliderThumb,
            leftHandSliders.sliderIndex,
            leftHandSliders.sliderMiddle,
            leftHandSliders.sliderRing,
            leftHandSliders.sliderPinky,
        };
        rightSliders = new Slider[] {
            rightHandSliders.sliderThumb,
            rightHandSliders.sliderIndex,
            rightHandSliders.sliderMiddle,
            rightHandSliders.sliderRing,
            rightHandSliders.sliderPinky,
        };
    }

    public void ValueChanged(TouchFingerType fingerType, bool isLeftHand, bool touching)
    {
        if (isLeftHand)
        {
            leftSliders[(int)fingerType].value = touching ? 100 : 0;
        }
        else
        {
            rightSliders[(int)fingerType].value = touching ? 100 : 0;
        }
    }
}

