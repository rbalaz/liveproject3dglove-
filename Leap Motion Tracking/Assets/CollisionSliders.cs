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

    // Assign slider to finger
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
    [Tooltip("Distance threshold to decide whether finger is touching the object")]
    public float touchDistance;
    [Tooltip("Sliders for left hand fingers")]
    public Sliders leftHandSliders;
    [Tooltip("Sliders for right hand fingers")]
    public Sliders rightHandSliders;

    // Lists of left and right hand fingers
    private List<Finger> leftHandFingers;
    private List<Finger> rightHandFingers;

    // Collider of the object the script is attached to
    private Collider myCollider;

    // Constants used for finding fingers in the scene
    private int BONES_COUNT = 5;
    private string BASE_PATH_LEFT = "/LMHeadMountedRig/Interaction Manager/Left Interaction Hand Contact Bones/Contact Fingerbone ";
    private string BASE_PATH_RIGHT = "/LMHeadMountedRig/Interaction Manager/Right Interaction Hand Contact Bones/Contact Fingerbone ";
    private string[] BONE_NAMES = new string[] { "(Thumb-Distal)", "(Index-Distal)", "(Middle-Distal)", "(Ring-Distal)", "(Pinky-Distal)" };


    void Start()
    {
        // Initialize lists
        leftHandFingers = new List<Finger>();
        rightHandFingers = new List<Finger>();
        // Store collider (performance reasons)
        myCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (leftHandFingers.Count != BONES_COUNT) TryFindFingers(BASE_PATH_LEFT);
        if (rightHandFingers.Count != BONES_COUNT) TryFindFingers(BASE_PATH_RIGHT);
    }

    public void UpdateOnHover() // Called during HoverStay() and HoverEnd() events by InteractionBehavior script
    {
        UpdateSliders(leftHandFingers);
        UpdateSliders(rightHandFingers);
    }

    private void TryFindFingers(string basePath)
    {
        // Try to find distal bone of each finger
        foreach (string bone in BONE_NAMES)
        {
            GameObject tmp = GameObject.Find(basePath + bone);
            // If object was found, store it in the list
            if (tmp != null)
            {
                if (basePath == BASE_PATH_LEFT)
                    leftHandFingers.Add(new Finger(tmp, float.PositiveInfinity, leftHandSliders));
                else
                    rightHandFingers.Add(new Finger(tmp, float.PositiveInfinity, rightHandSliders));
            }
        }

    }

    private void UpdateSliders(List<Finger> fingers)
    {
        // If the list is empty, do nothing (hand was not yet initialized)
        if (fingers.Count != BONES_COUNT) return;


        foreach (Finger finger in fingers)
        {
            // Cast ray from finger to the center of the object
            RaycastHit[] hits;
            Vector3 origin = finger.obj.transform.position;
            hits = Physics.RaycastAll(origin, transform.position - origin, 100.0F);

            foreach (RaycastHit hitInfo in hits)
            {
                // Debug - draw line in editor
                if (finger.slider == rightHandSliders.sliderIndex) Debug.DrawRay(origin, transform.position - origin, Color.red);

                // If the ray hit the object's collider, compare distance with threshold and update sliders
                if (hitInfo.collider == myCollider)
                {
                    if (hitInfo.distance < touchDistance)
                        finger.slider.value = ((touchDistance - hitInfo.distance) * 120) / touchDistance;
                    else
                        finger.slider.value = 0;
                }
            }
        }
    }
}