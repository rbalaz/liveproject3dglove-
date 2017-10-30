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
            {"thumb", sliderThumb},
            {"index", sliderIndex},
            {"middle", sliderMiddle},
            {"ring", sliderRing},
            {"pinky", sliderPinky}
        };
    }

    public void CollisionStay(Collision other)
    {
        if (other.transform.name.Contains("bone"))
        {
            int childIndex = -1;
            if (other.transform.name == "bone3")
                childIndex = 1;
            else if (other.transform.name == "bone2")
                childIndex = 0;
            else
                return;

            // Calculate intersection rate
            Vector3 farBone = other.transform.position;
            Vector3 closeBone = other.transform.parent.GetChild(childIndex).transform.position;
            int rate = calculateRateOfIntersection(farBone, closeBone, other);
            if (rate == -1)
                return;

            // Set slider value
            string finger = other.transform.parent.name;
            if (childIndex == 1)
                slidersDict[finger].value = rate;
            else
                slidersDict[finger].value = 100 + rate;
        }
    }

    public void CollisionExit(Collision other)
    {
        if (other.transform.name == "bone3")
        {
            string finger = other.transform.parent.name;
            slidersDict[finger].value = 0;
        }
            
        //Debug.Log(finger + ", " + other.transform.name + ", exit");
    }

    int calculateRateOfIntersection(Vector3 farBone, Vector3 closeBone, Collision other)
    {
        Vector3 contact = other.contacts[0].point;
        float boneDistance = Vector3.Distance(farBone, closeBone);
        float contactDistance = Vector3.Distance(farBone, contact);
        if (intersectionHappened(farBone, closeBone, contact, boneDistance) == true)
        {
            float intersectionRate = (boneDistance > contactDistance) ? 100 * (contactDistance / boneDistance) : 100;
            //Debug.Log("Bone: " + other.transform.name + " Intersection rate: " + intersectionRate + "%.");
            return Mathf.RoundToInt(intersectionRate);
        }
        return -1;
    }

    bool intersectionHappened(Vector3 farBone, Vector3 closeBone, Vector3 contact, float boneDistance)
    {
        float farContactDistance = Vector3.Distance(farBone, contact);
        float closeContactDistance = Vector3.Distance(closeBone, contact);

        return farContactDistance < boneDistance && closeContactDistance < boneDistance;
    }

}

public class CollisionSliders : MonoBehaviour {

    public Hand leftHand;
    public Hand rightHand;

    void Start () {
        leftHand.FillDictionary();
        rightHand.FillDictionary();
    }

    private void OnCollisionStay(Collision collision)
    {        
        if (collision.transform.name.Contains("bone"))
        {
            string hand = collision.transform.parent.parent.name;
            if (hand == "RigidRoundHand_L")
            {
                leftHand.CollisionStay(collision);
            }
            else if (hand == "RigidRoundHand_R")
            {
                rightHand.CollisionStay(collision);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.name.Contains("bone"))
        {
            string hand = collision.transform.parent.parent.name;
            if (hand == "RigidRoundHand_L")
            {
                leftHand.CollisionExit(collision);
            }
            else if (hand == "RigidRoundHand_R")
            {
                rightHand.CollisionExit(collision);
            }
        }
    }
}

/*
void logAllBonePositions(Collision other)
{
    Vector3 bone1 = other.transform.parent.GetChild(0).transform.position;
    Vector3 bone2 = other.transform.parent.GetChild(1).transform.position;
    Vector3 bone3 = other.transform.parent.GetChild(2).transform.position;

    Debug.Log("Bone 1: X:" + bone1.x + " Y: " + bone1.y + " Z: " + bone1.z);
    Debug.Log("Bone 2: X:" + bone2.x + " Y: " + bone2.y + " Z: " + bone2.z);
    Debug.Log("Bone 3: X:" + bone3.x + " Y: " + bone3.y + " Z: " + bone3.z);
}
*/
