using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSetupTableScene : MonoBehaviour {

    public GameObject dicom;
    public GameObject patient;
    public GameObject MRI;
    public List<GameObject> simpleObjects;
    
    private List<Transform> patientParts = new List<Transform>();
    private List<Transform> MRIParts = new List<Transform>();

    void Start()
    {
        var patientPartsTmp = new List<Transform>(patient.GetComponentsInChildren<Transform>());
        var MRIPartsTmp = new List<Transform>(MRI.GetComponentsInChildren<Transform>());
        foreach(var part in patientPartsTmp)
        {
            if (part.parent != patient.transform.parent)
                patientParts.Add(part);
        }
        foreach (var part in MRIPartsTmp)
        {
            if (part.parent != MRI.transform.parent)
                MRIParts.Add(part);
        }

        // Instantly disappear at start
        patient.SetActive(false);
        MRI.SetActive(false);

        SwitchPatient(false);
        SwitchMRI(false);
        SwitchDicom(false);
        SwitchSimpleObjects(false);
    }

    public void SwitchDicom(bool state)
    {
        dicom.SetActive(state);
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
        if(state == true)
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
        foreach (var obj in simpleObjects)
        {
            obj.SetActive(state);
            yield return null;
        }
    }

}
