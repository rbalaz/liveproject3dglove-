using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;

public class CollisionReport : MonoBehaviour
{
    /*
	// Use this for initialization
	void Start () {
		
	}	
	void Update ()
    {

	}
    */
    void OnCollisionEnter(Collision other)
    {
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
        Debug.Log("Collision detected: " + hand + "-" + finger + "-" + fingerPart + ".");
    }
}
