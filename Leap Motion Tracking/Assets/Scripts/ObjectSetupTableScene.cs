using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SimpleObjectList
{
    public GameObject gameObject;
    public Transform SpawnPosition;
}

public class ObjectSetupTableScene : MonoBehaviour
{

    public GameObject dicom;
    public GameObject patient;
    public GameObject heart;
    public GameObject MRI;
    public GameObject bones;
    public GameObject bonesImage;
    public List<SimpleObjectList> simpleObjects;

    private List<GameObject> simpleObjectsOnScene = new List<GameObject>();

    private List<Transform> patientParts = new List<Transform>();
    private List<Transform> MRIParts = new List<Transform>();
    private List<Transform> heartParts = new List<Transform>();
    private List<Transform> bonesParts = new List<Transform>();

    void Start()
    {
        var patientPartsTmp = new List<Transform>(patient.GetComponentsInChildren<Transform>());
        var MRIPartsTmp = new List<Transform>(MRI.GetComponentsInChildren<Transform>());
        var heartPartsTmp = new List<Transform>(heart.GetComponentsInChildren<Transform>());
        var bonesPartsTmp = new List<Transform>(bones.GetComponentsInChildren<Transform>());

        foreach (var part in patientPartsTmp)
        {
            if (part.parent != patient.transform.parent)
                patientParts.Add(part);
        }
        foreach (var part in MRIPartsTmp)
        {
            if (part.parent != MRI.transform.parent)
                MRIParts.Add(part);
        }
        foreach (var part in heartPartsTmp)
        {
            if (part.parent != heart.transform.parent)
                heartParts.Add(part);
        }
        foreach (var part in bonesPartsTmp)
        {
            if (part.parent != bones.transform.parent)
                bonesParts.Add(part);
        }

        // Instantly disappear at start
        patient.SetActive(false);
        MRI.SetActive(false);

        SwitchPatient(false);
        SwitchMRI(false);
        SwitchDicom(false);
        SwitchHeart(false);
        SwitchSimpleObjects(false);
        //SwitchBones(false);
    }

    public void SwitchDicom(bool state)
    {
        dicom.SetActive(state);
    }

    public void SwitchBones(bool state)
    {
        StartCoroutine(SwitchBonesCoroutine(state));
        bonesImage.SetActive(state);
    }

    private IEnumerator SwitchBonesCoroutine(bool state)
    {
        if (state == true)
            bones.SetActive(state);
        foreach (var part in bonesParts)
        {
            part.gameObject.SetActive(state);
            yield return null;
        }
        if (state == false)
            bones.SetActive(state);
    }

    public void SwitchHeart(bool state)
    {
        StartCoroutine(SwitchHeartCoroutine(state));
    }

    private IEnumerator SwitchHeartCoroutine(bool state)
    {
        if (state == true)
            heart.SetActive(state);
        foreach (var part in heartParts)
        {
            part.gameObject.SetActive(state);
            yield return null;
        }
        if (state == false)
            heart.SetActive(state);
    }

    public void SwitchPatient(bool state)
    {
        StartCoroutine(SwitchPatientCoroutine(state));
    }

    private IEnumerator SwitchPatientCoroutine(bool state)
    {
        if (state == true)
            patient.SetActive(state);
        foreach (var part in patientParts)
        {
            part.gameObject.SetActive(state);
            yield return null;
        }
        if (state == false)
            patient.SetActive(state);
    }

    public void SwitchMRI(bool state)
    {
        StartCoroutine(SwitchMRICoroutine(state));
    }

    private IEnumerator SwitchMRICoroutine(bool state)
    {
        if (state == true)
            MRI.SetActive(state);
        foreach (var part in MRIParts)
        {
            part.gameObject.SetActive(state);
            yield return null;
        }
        if (state == false)
            MRI.SetActive(state);
    }

    public void SwitchSimpleObjects(bool state)
    {
        StartCoroutine(SwitchSimpleObjectsCoroutine(state));
    }

    private IEnumerator SwitchSimpleObjectsCoroutine(bool state)
    {
        if (state == true)
            foreach (var obj in simpleObjects)
            {
                GameObject newObj = Instantiate(obj.gameObject, obj.SpawnPosition.parent);
                newObj.transform.position = obj.SpawnPosition.transform.position;
                if (Configuration.IsGravityOn)
                {
                    newObj.GetComponent<Rigidbody>().isKinematic = false;
                }
                simpleObjectsOnScene.Add(newObj);
                yield return null;
            }
        else
        {
            foreach (var obj in simpleObjectsOnScene)
            {
                Destroy(obj);
                yield return null;
            }
            simpleObjectsOnScene = new List<GameObject>();
        }

    }

}
