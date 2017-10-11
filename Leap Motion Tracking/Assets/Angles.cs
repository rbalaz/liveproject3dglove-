using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angles : MonoBehaviour {

    public class FingerStatus
    {
        public string objectName;
        public float x;
        public float y;
        public float z;

        public FingerStatus(string objectName, float x, float y, float z)
        {
            this.objectName = objectName;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    private FingerStatus[] fingerStatus;

    void Start()
    {
        fingerStatus = new FingerStatus[5];
        fingerStatus[0] = new FingerStatus("thumb", 0, 0, 0);
        fingerStatus[1] = new FingerStatus("index", 0, 0, 0);
        fingerStatus[2] = new FingerStatus("middle", 0, 0, 0);
        fingerStatus[3] = new FingerStatus("ring", 0, 0, 0);
        fingerStatus[4] = new FingerStatus("pinky", 0, 0, 0);
    }

    void Update ()
    {
        //Log of bone part
        Vector3 boneAngles = new Vector3();
        boneAngles.x = gameObject.transform.GetChild(0).transform.rotation.x;
        boneAngles.y = gameObject.transform.GetChild(0).transform.rotation.y;
        boneAngles.z = gameObject.transform.GetChild(0).transform.rotation.z;
        Debug.Log(gameObject.transform.GetChild(0).name + "- X: " + boneAngles.x + " Y: " + boneAngles.y + " Z: " + boneAngles.z + ".");

        //Log of finger rotation values
        var parent = gameObject.transform.parent;
        int childCount = parent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Vector3 angles = new Vector3();
            angles.x = parent.GetChild(i).transform.rotation.x;
            angles.y = parent.GetChild(i).transform.rotation.y;
            angles.z = parent.GetChild(i).transform.rotation.z;
            string name = parent.GetChild(i).name;
            string[] fingers = { "pinky", "thumb", "ring", "index", "middle" };
            for (int j = 0; j < fingers.Length; j++)
            {
                if (fingers[j] == name && shouldUpdate(name, angles))
                {
                    Debug.Log(name);
                    Debug.Log("X: " + angles.x + " Y: " + angles.y + " Z: " + angles.z + ".");
                    break;
                }
            }
        }
	}

    private bool shouldUpdate(string name, Vector3 angles)
    {
        for (int i = 0; i < 5; i++)
        {
            if (name == fingerStatus[i].objectName)
            {
                if (angles.x != fingerStatus[i].x)
                {
                    fingerStatus[i].x = angles.x;
                    fingerStatus[i].y = angles.y;
                    fingerStatus[i].z = angles.z;
                    return true;
                }
                else if (angles.y != fingerStatus[i].y)
                {
                    fingerStatus[i].y = angles.y;
                    fingerStatus[i].z = angles.z;
                    return true;
                }
                else if (angles.z != fingerStatus[i].z)
                {
                    fingerStatus[i].z = angles.z;
                    return true;
                }
                else
                    return false;
            }
        }
        return false;
    }
}
