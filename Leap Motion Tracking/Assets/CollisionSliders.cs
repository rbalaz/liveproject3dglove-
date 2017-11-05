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
    public float touchDistance;
    public Sliders leftHandSliders;
    public Sliders rightHandSliders;

    private List<Finger> leftHandFingers;
    private List<Finger> rightHandFingers;

    private Collider myCollider;

    private int BONES_COUNT = 5;
    private string BASE_PATH_LEFT = "/LMHeadMountedRig/Interaction Manager/Left Interaction Hand Contact Bones/Contact Fingerbone ";
    private string BASE_PATH_RIGHT = "/LMHeadMountedRig/Interaction Manager/Right Interaction Hand Contact Bones/Contact Fingerbone ";
    private string[] BONE_NAMES = new string[] { "(Thumb-Distal)", "(Index-Distal)", "(Middle-Distal)", "(Ring-Distal)", "(Pinky-Distal)" };


    void Start()
    {
        leftHandFingers = new List<Finger>();
        rightHandFingers = new List<Finger>();
        myCollider = GetComponent<Collider>();
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
        if (leftHandFingers.Count < BONES_COUNT) tryFindBones(BASE_PATH_LEFT);        
        if (rightHandFingers.Count < BONES_COUNT) tryFindBones(BASE_PATH_RIGHT);
    }

    public void HoverStay()
    {
        UpdateSliders(leftHandFingers);
        UpdateSliders(rightHandFingers);
    }

    private void UpdateSliders(List<Finger> fingers)
    {
        if (fingers.Count != BONES_COUNT) return;

        foreach (Finger finger in fingers)
        {
            /*
            finger.distance = Vector3.Distance(transform.position, finger.obj.transform.position);
            if (finger.distance < 0.3) finger.slider.value = 1;
            else finger.slider.value = 0;
            */
            

            RaycastHit[] hits;
            Vector3 origin = finger.obj.transform.position;
            hits = Physics.RaycastAll(origin, transform.position-origin, 100.0F);

            foreach (RaycastHit hitInfo in hits)
            {
                if (finger.slider == rightHandSliders.sliderIndex) Debug.DrawRay(origin, transform.position - origin, Color.red, 1.0f);

                if (hitInfo.collider == myCollider)
                { 
                    if (hitInfo.distance < touchDistance)
                        finger.slider.value = ((touchDistance-hitInfo.distance)*120)/ touchDistance;
                    else
                        finger.slider.value = 0;
                }
            }
            /*
            RaycastHit hitInfo;
            if (Physics.Linecast(finger.obj.transform.position, , out hitInfo))
            {
                //if (finger.slider == leftHandSliders.sliderIndex) Debug.Log(hitInfo.distance);
                if (hitInfo.collider == myCollider && hitInfo.distance < touchDistance)
                    finger.slider.value = 1;
                else
                    finger.slider.value = 0;
            }
            */


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