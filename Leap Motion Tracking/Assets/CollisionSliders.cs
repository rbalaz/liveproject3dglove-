using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Hand
{
    public Slider sliderThumb;
    public Slider sliderIndex;
    public Slider sliderMiddle;
    public Slider sliderRing;
    public Slider sliderPinky;

    public Dictionary<string, Slider> slidersDict; // Contains all sliders
    public void FillDictionary()
    {
        slidersDict = new Dictionary<string, Slider>()
        {
            {"Thumb", sliderThumb},
            {"Index", sliderIndex},
            {"Middle", sliderMiddle},
            {"Ring", sliderRing},
            {"Pinky", sliderPinky}
        };
    }

    public void CollisionStay(Collision other)
    {
        foreach (KeyValuePair<string, Slider> row in slidersDict)
        {
            if (other.transform.name.Contains(row.Key))
            {
                row.Value.value = 1;
            }
        }
    }

    public void CollisionExit(Collision other)
    {
        foreach (KeyValuePair<string, Slider> row in slidersDict)
        {
            if (other.transform.name.Contains(row.Key))
            {
                row.Value.value = 0;
            }
        }
    }
}

public class CollisionSliders : MonoBehaviour
{

    public Hand leftHand;
    public Hand rightHand;

    void Start()
    {
        leftHand.FillDictionary();
        rightHand.FillDictionary();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.name.Contains("Contact Fingerbone"))
        {
            //Debug.Log(collision.transform.name);
            string hand = collision.transform.parent.name;
            if (hand.Contains("Left"))
            {
                leftHand.CollisionStay(collision);
            }
            else
            {
                rightHand.CollisionStay(collision);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.name.Contains("Contact Fingerbone"))
        {
            string hand = collision.transform.parent.name;
            if (hand.Contains("Left"))
            {
                leftHand.CollisionExit(collision);
            }
            else
            {
                rightHand.CollisionExit(collision);
            }
        }
    }
}