using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;

public class CollisionReport : MonoBehaviour
{
    List<Collision> collidingObjects = new List<Collision>();
    /*
        // Use this for initialization
        void Start ()
        {
        }
        */
    void Update()
    {
        foreach (Collision collider in collidingObjects)
        {
            calculateRateOfIntersection(collider);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        collidingObjects.Add(other);
        string hand = other.transform.parent.parent.name;
        string finger = other.transform.parent.name;
        string bone = other.transform.name;
        string fingerPart = "";
        switch (bone)
        {
            case "bone3":
                fingerPart = "Tip";
                break;
            case "bone2":
                fingerPart = "Middle";
                break;
            case "bone1":
                fingerPart = "Base";
                break;
            default:
                fingerPart = "Palm";
                break;
        }
        //Debug.Log("Collision detected: " + hand + "-" + finger + "-" + fingerPart + ".");
    }


    void OnCollisionExit(Collision other)
    {
        string hand = other.transform.parent.parent.name;
        string finger = other.transform.parent.name;
        string bone = other.transform.name;
        int position = -1;
        for(int i = 0; i < collidingObjects.Count; i++)
        {
            if (collidingObjects[i].transform.name == bone)
            {
                if (collidingObjects[i].transform.parent.name == finger)
                {
                    if (collidingObjects[i].transform.parent.parent.name == hand)
                    {
                        position = i;
                    }
                }
            }
        }
        if (position > -1)
            collidingObjects.RemoveAt(position);
    }

    void calculateRateOfIntersection(Collision other)
    {
        string predecessor = identifyPredecessor(other);
        string finger = other.transform.parent.name;
        string hand = other.transform.parent.parent.name;
        if (predecessor == "bone2")
        {
            Vector3 bone3 = other.transform.position;
            Vector3 bone2 = other.transform.parent.GetChild(1).transform.position;
            Vector3 contact = other.contacts[0].point;
            float boneDistance = getDistanceBetweenVectors(bone3, bone2);
            float contactDistance = getDistanceBetweenVectors(bone3, contact);
            if (intersectionHappened(other, bone3, bone2, contact, boneDistance) == true)
            {
                if (boneDistance > contactDistance)
                {
                    Debug.Log("Hand: " + hand + " Finger: " + finger + " Bone: " + other.transform.name + " Intersection rate: " + 100 * (contactDistance / boneDistance) + "%.");
                }
                else
                {
                    Debug.Log("Hand: " + hand + " Finger: " + finger + " Bone: " + other.transform.name + " Intersection rate: 100%");
                }
            }
        }
        else if (predecessor == "bone1")
        {
            Vector3 bone2 = other.transform.position;
            Vector3 bone1 = other.transform.parent.GetChild(0).transform.position;
            Vector3 contact = other.contacts[0].point;
            float boneDistance = getDistanceBetweenVectors(bone2, bone1);
            float contactDistance = getDistanceBetweenVectors(bone2, contact);
            if (intersectionHappened(other, bone2, bone1, contact, boneDistance) == true)
            {
                if (boneDistance > contactDistance)
                {
                    Debug.Log("Hand: " + hand + " Finger: " + finger + " Bone: " + other.transform.name + " Intersection rate: " + 100 * (contactDistance / boneDistance) + "%.");
                }
                else
                {
                    Debug.Log("Hand: " + hand + " Finger: " + finger + " Bone: " + other.transform.name + " Intersection rate: 100%");
                }
            }
        }
    }

    void logAllBonePositions(Collision other)
    {
        Vector3 bone1 = other.transform.parent.GetChild(0).transform.position;
        Vector3 bone2 = other.transform.parent.GetChild(1).transform.position;
        Vector3 bone3 = other.transform.parent.GetChild(2).transform.position;

        Debug.Log("Bone 1: X:" + bone1.x + " Y: " + bone1.y + " Z: " + bone1.z);
        Debug.Log("Bone 2: X:" + bone2.x + " Y: " + bone2.y + " Z: " + bone2.z);
        Debug.Log("Bone 3: X:" + bone3.x + " Y: " + bone3.y + " Z: " + bone3.z);
    }

    string identifyPredecessor(Collision other)
    {
        if (other.transform.name == "bone3")
            return "bone2";
        if (other.transform.name == "bone2")
            return "bone1";
        return null;
    }

    bool intersectionHappened(Collision other, Vector3 farBone, Vector3 closeBone, Vector3 contact, float boneDistance)
    {
        float farContactDistance = getDistanceBetweenVectors(farBone, contact);
        float closeContactDistance = getDistanceBetweenVectors(closeBone, contact);

        return farContactDistance < boneDistance && closeContactDistance < boneDistance;
    }

    float getDistanceBetweenVectors(Vector3 first, Vector3 second)
    {
        return Mathf.Sqrt(Mathf.Pow(first.x - second.x, 2) + Mathf.Pow(first.y - second.y, 2) + Mathf.Pow(first.z - second.z, 2));
    }
}
