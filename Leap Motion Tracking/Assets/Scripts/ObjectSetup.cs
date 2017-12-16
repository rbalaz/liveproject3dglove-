using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSetup : MonoBehaviour {

    public GameObject stage;
    public Canvas canvas;
    private Transform[] stageChildren;
    private Transform[] canvasChildren;
    private Transform[] scheletroBones;
    private Transform[] skeletonBones;

    private void Start()
    {
        stageChildren = stage.GetComponentsInChildren<Transform>();
        canvasChildren = canvas.GetComponentsInChildren<Transform>();
        skeletonBones = stage.GetComponentsInChildren<Transform>();
        FlushChildren();
		DeactivateDicom();
		DeactivateMRI();
		DeactivatePatient();
		DeactivateSimpleObjects();
		DeactivateSimpleSkeleton();
    }

    private void FlushChildren()
    {
        List<Transform> stageChildrenList = new List<Transform>();
        List<Transform> scheletroBonesList = new List<Transform>();
        List<Transform> skeletonBonesList = new List<Transform>();
        for (int i = 0; i < stageChildren.Length; i++)
        {
            if (stageChildren[i].parent != null)
                if (stageChildren[i].parent.name == "Stage")
                    stageChildrenList.Add(stageChildren[i]);
                else if (stageChildren[i].parent.name.Contains("Scheletro"))
                    scheletroBonesList.Add(stageChildren[i]);
                else if (stageChildren[i].parent.name.Contains("Skeleton"))
                    skeletonBonesList.Add(stageChildren[i]);

        }

        List<Transform> canvasChildrenList = new List<Transform>();
        for (int i = 0; i < canvasChildren.Length; i++)
        {
            if (canvasChildren[i].parent != null)
                if (canvasChildren[i].parent.name == "Canvas")
                    canvasChildrenList.Add(canvasChildren[i]);
        }
        
        canvasChildren = canvasChildrenList.ToArray();
	    stageChildren = stageChildrenList.ToArray();
        scheletroBones = scheletroBonesList.ToArray();
        skeletonBones = skeletonBonesList.ToArray();
    }

    public void ActivateSimpleSkeleton()
    {
        StartCoroutine(SetSimpleBones(true));
    }

    private IEnumerator SetSimpleBones(bool status)
    {
        if (!status)
            SetSimpleSkeleton(status);
        for (int i = 0; i < skeletonBones.Length; i++)
        {
            GameObject gameobj = skeletonBones[i].gameObject;
            gameobj.SetActive(status);

            yield return null;
        }
        if (status)
            SetSimpleSkeleton(status);
    }

    private void SetSimpleSkeleton(bool status)
    {
        for (int i = 0; i < stageChildren.Length; i++)
        {
            if (stageChildren[i].name.Contains("Skeleton"))
            {
                GameObject gameobj = stageChildren[i].gameObject;
                gameobj.SetActive(status);
            }
        }
    }

    public void DeactivateSimpleSkeleton()
    {
        StartCoroutine(SetSimpleBones(false));

        for (int i = 0; i < stageChildren.Length; i++)
        {
            if (stageChildren[i].name.Contains("Skeleton"))
            {
                GameObject gameobj = stageChildren[i].gameObject;
                gameobj.SetActive(false);
            }
        }
    }

    public void ActivateDicom()
    {
        for (int i = 0; i < stageChildren.Length; i++)
        {
            if (stageChildren[i].name.Contains("AbdomenModel"))
            {
                GameObject gameobj = stageChildren[i].gameObject;
                gameobj.SetActive(true);
            }
        }
    }

    public void DeactivateDicom()
    {
        for (int i = 0; i < stageChildren.Length; i++)
        {
            if (stageChildren[i].name.Contains("AbdomenModel"))
            {
                GameObject gameobj = stageChildren[i].gameObject;
                gameobj.SetActive(false);
            }
        }
    }

    public void ActivatePatient()
    {
        StartCoroutine(SetScheletroBones(true));
    }

    private IEnumerator SetScheletroBones(bool status)
    {
        if (!status)
            SetTable(status);
        for (int i = 0; i < scheletroBones.Length; i++)
        {
            GameObject gameobj = scheletroBones[i].gameObject;
            gameobj.SetActive(status);

            yield return null;
        }
        if (status)
            SetTable(status);
    }

    private void SetTable(bool status)
    {
        for (int i = 0; i < stageChildren.Length; i++)
        {
            if (stageChildren[i].name.Contains("Table"))
            {
                GameObject gameobj = stageChildren[i].gameObject;
                gameobj.SetActive(status);
            }
        }
    }

    public void DeactivatePatient()
    {
        StartCoroutine(SetScheletroBones(false));
    }

    public void ActivateSimpleObjects()
    {
        for (int i = 0; i < stageChildren.Length; i++)
        {
            if (stageChildren[i].name.Contains("Cube"))
            {
                GameObject gameobj = stageChildren[i].gameObject;
                gameobj.SetActive(true);
            }
            if (stageChildren[i].name.Contains("Sphere"))
            {
                GameObject gameobj = stageChildren[i].gameObject;
                gameobj.SetActive(true);
            }
            if (stageChildren[i].name.Contains("Heart"))
            {
                GameObject gameobj = stageChildren[i].gameObject;
                gameobj.SetActive(true);
            }
        }
    }

    public void DeactivateSimpleObjects()
    {
        for (int i = 0; i < stageChildren.Length; i++)
        {
            if (stageChildren[i].name.Contains("Cube"))
            {
                GameObject gameobj = stageChildren[i].gameObject;
                gameobj.SetActive(false);
            }
            if (stageChildren[i].name.Contains("Sphere"))
            {
                GameObject gameobj = stageChildren[i].gameObject;
                gameobj.SetActive(false);
            }
            if (stageChildren[i].name.Contains("Heart"))
            {
                GameObject gameobj = stageChildren[i].gameObject;
                gameobj.SetActive(false);
            }
        }
    }

    public void ActivateMRI()
    {
        for (int i = 0; i < stageChildren.Length; i++)
        {
            if (stageChildren[i].name.Contains("MRI Siemens"))
            {
                GameObject gameobj = stageChildren[i].gameObject;
                gameobj.SetActive(true);
            }
        }
    }

    public void DeactivateMRI()
    {
        for (int i = 0; i < stageChildren.Length; i++)
        {
            if (stageChildren[i].name.Contains("MRI Siemens"))
            {
                GameObject gameobj = stageChildren[i].gameObject;
                gameobj.SetActive(false);
            }
        }
    }

}
