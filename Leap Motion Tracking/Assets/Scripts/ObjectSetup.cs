using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSetup : MonoBehaviour {

    public GameObject stage;
    public Canvas canvas;
    private Transform[] children;
    private Transform[] canvasChildren;

    private void Start()
    {
        children = stage.GetComponentsInChildren<Transform>();
        canvasChildren = canvas.GetComponentsInChildren<Transform>();
    }

    public void ActivateSimpleSkeleton()
    {
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name.Contains("Skeleton"))
            {
                GameObject gameobj = children[i].gameObject;
                gameobj.SetActive(true);
            }
        }
    }

    public void DeactivateSimpleSkeleton()
    {
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name.Contains("Skeleton"))
            {
                GameObject gameobj = children[i].gameObject;
                gameobj.SetActive(false);
            }
        }
    }

    public void ActivateDicom()
    {
        for (int i = 0; i < canvasChildren.Length; i++)
        {
            if (canvasChildren[i].name.Contains("RawImage"))
            {
                GameObject gameobj = canvasChildren[i].gameObject;
                gameobj.SetActive(true);
            }
        }
    }

    public void DeactivateDicom()
    {
        for (int i = 0; i < canvasChildren.Length; i++)
        {
            if (canvasChildren[i].name.Contains("RawImage"))
            {
                GameObject gameobj = canvasChildren[i].gameObject;
                gameobj.SetActive(false);
            }
        }
    }

    public void ActivatePatient()
    {
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name.Contains("Table"))
            {
                GameObject gameobj = children[i].gameObject;
                gameobj.SetActive(true);
            }
        }
    }

    public void DeactivatePatient()
    {
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name.Contains("Table"))
            {
                GameObject gameobj = children[i].gameObject;
                gameobj.SetActive(false);
            }
            if (children[i].name.Contains("Scheletro50"))
            {
                GameObject gameobj = children[i].gameObject;
                gameobj.SetActive(false);
            }
        }
    }

    public void ActivateSimpleObjects()
    {
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name.Contains("Cube"))
            {
                GameObject gameobj = children[i].gameObject;
                gameobj.SetActive(true);
            }
            if (children[i].name.Contains("Sphere"))
            {
                GameObject gameobj = children[i].gameObject;
                gameobj.SetActive(true);
            }
            if (children[i].name.Contains("Heart"))
            {
                GameObject gameobj = children[i].gameObject;
                gameobj.SetActive(true);
            }
        }
    }

    public void DeactivateSimpleObjects()
    {
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name.Contains("Cube"))
            {
                GameObject gameobj = children[i].gameObject;
                gameobj.SetActive(false);
            }
            if (children[i].name.Contains("Sphere"))
            {
                GameObject gameobj = children[i].gameObject;
                gameobj.SetActive(false);
            }
            if (children[i].name.Contains("Heart"))
            {
                GameObject gameobj = children[i].gameObject;
                gameobj.SetActive(false);
            }
        }
    }

    public void ActivateMRI()
    {
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name.Contains("MRI Siemens"))
            {
                GameObject gameobj = children[i].gameObject;
                gameobj.SetActive(true);
            }
        }
    }

    public void DeactivateMRI()
    {
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name.Contains("MRI Siemens"))
            {
                GameObject gameobj = children[i].gameObject;
                gameobj.SetActive(false);
            }
        }
    }

}
