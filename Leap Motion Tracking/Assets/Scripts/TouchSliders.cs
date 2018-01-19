using Leap;
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
        TouchDetector.OnTouchValueChange += ValueChanged;

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

    public void ValueChanged(Finger.FingerType fingerType, bool isLeftHand, float force)
    {
        if (isLeftHand)
        {
            leftSliders[(int)fingerType].value = force;
        }
        else
        {
            rightSliders[(int)fingerType].value = force;
        }
    }
}

