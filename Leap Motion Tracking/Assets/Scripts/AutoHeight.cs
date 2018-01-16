using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHeight : MonoBehaviour {

    public GameObject mainCameraRig;

    private static GameObject mainCameraStatic;

    void Start()
    {
        AutoHeight.mainCameraStatic = mainCameraRig;
    }

    public void OnContact()
    {
        Vector3 pos = mainCameraRig.transform.position;
        mainCameraRig.transform.position = new Vector3(pos.x, pos.y + 0.005f, pos.z);

    }

    public static void OnHandContact()
    {
        Vector3 pos = mainCameraStatic.transform.position;
        mainCameraStatic.transform.position = new Vector3(pos.x, pos.y + 0.005f, pos.z);
    }

   /* private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Fingerbone"))
        {
            Vector3 pos = mainCameraRig.transform.position;
            mainCameraRig.transform.position = new Vector3(pos.x, pos.y + 0.005f, pos.z);
        }
    }
    */
}
