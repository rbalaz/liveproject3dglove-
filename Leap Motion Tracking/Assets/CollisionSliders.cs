using System;
using System.Collections;
using System.Collections.Generic;
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

public class Finger
{
    public GameObject obj;
    public float distance;
    public Slider slider;

    public Finger(GameObject f, float d, Sliders sl)
    {
        obj = f;
        distance = d;
        FindSlider(sl);
    }

    private void FindSlider(Sliders sl)
    {
        if (obj.name.Contains("Thumb")) slider = sl.sliderThumb;
        if (obj.name.Contains("Index")) slider = sl.sliderIndex;
        if (obj.name.Contains("Middle")) slider = sl.sliderMiddle;
        if (obj.name.Contains("Ring")) slider = sl.sliderRing;
        if (obj.name.Contains("Pinky")) slider = sl.sliderPinky;
    }
}

public class CollisionSliders : MonoBehaviour
{

    public Sliders leftHandSliders;
    public Sliders rightHandSliders;

    private List<Finger> leftHandFingers;
    private List<Finger> rightHandFingers;

    private int BONES_COUNT = 5;
    private string BASE_PATH_LEFT = "/LMHeadMountedRig/Interaction Manager/Left Interaction Hand Contact Bones/Contact Fingerbone ";
    private string BASE_PATH_RIGHT = "/LMHeadMountedRig/Interaction Manager/Right Interaction Hand Contact Bones/Contact Fingerbone ";
    private string[] BONE_NAMES = new string[] { "(Thumb-Intermediate)", "(Index-Intermediate)", "(Middle-Intermediate)", "(Ring-Intermediate)", "(Pinky-Intermediate)" };

    void Start()
    {
        leftHandFingers = new List<Finger>();
        rightHandFingers = new List<Finger>();
    }

    private void tryFindBones(string basePath)
    {
        foreach (string bone in BONE_NAMES)
        {
            GameObject tmp = GameObject.Find(basePath + bone);
            if (tmp != null)
            {
                if (basePath == BASE_PATH_LEFT)
                    leftHandFingers.Add(new Finger(tmp, float.PositiveInfinity, leftHandSliders));
                else
                    rightHandFingers.Add(new Finger(tmp, float.PositiveInfinity, rightHandSliders));
            }
        }

    }

    private void Update()
    {
        // !!!!!!  UpdateSliders mozno nie cez update ale cez OnHover() event

        if (leftHandFingers.Count < BONES_COUNT)
            tryFindBones(BASE_PATH_LEFT);
        else
            UpdateSliders(leftHandFingers);

        if (rightHandFingers.Count < BONES_COUNT)
            tryFindBones(BASE_PATH_RIGHT);
        else
            UpdateSliders(rightHandFingers);
    }

    private void UpdateSliders(List<Finger> fingers)
    {
        foreach (Finger finger in fingers)
        {
            /*
            finger.distance = Vector3.Distance(transform.position, finger.obj.transform.position);
            if (finger.distance < 0.3) finger.slider.value = 1;
            else finger.slider.value = 0;
            */

            // !!!!! este treba skusit ine moznosti hladania vzdialenosti (Linecast je pomaly, a tiez hitInfo.distance < ???)

            RaycastHit hitInfo;
            if (Physics.Linecast(transform.position, finger.obj.transform.position, out hitInfo))
            { 
                if (hitInfo.distance < (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z)/5)
                    finger.slider.value = 1;
                else
                    finger.slider.value = 0;
            }



        }
    }

    /*
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
    */
}