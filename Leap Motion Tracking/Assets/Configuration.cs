using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Configuration : MonoBehaviour {

    public GameObject mainCameraRig;
    public GameObject stage;

    private float defaultCameraHeight;

    void Start()
    {
        defaultCameraHeight = mainCameraRig.transform.position.y;
    }

    public void SetHeight(float value)
    {
        Vector3 pos = mainCameraRig.transform.position;
        mainCameraRig.transform.position = new Vector3(pos.x, defaultCameraHeight + value, pos.z);
    }

    public void GravityOn()
    {
        SetKinematic(false);
    }

    public void GravityOff()
    {
        SetKinematic(true);
    }

    private void SetKinematic(bool state)
    {
        Transform[] allStageChildren = stage.GetComponentsInChildren<Transform>();
        foreach (Transform child in allStageChildren)
        {
            Debug.Log(child.gameObject.name + ", " + transform);
            Rigidbody childRb = child.GetComponent<Rigidbody>();
            if (childRb != null) childRb.isKinematic = state;
        }
    }
}
