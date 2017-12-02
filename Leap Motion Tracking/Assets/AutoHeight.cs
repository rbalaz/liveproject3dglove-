using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHeight : MonoBehaviour {

    public GameObject mainCameraRig;

    public void OnContact()
    {
        Vector3 pos = mainCameraRig.transform.position;
        mainCameraRig.transform.position = new Vector3(pos.x, pos.y + 0.005f, pos.z);

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
