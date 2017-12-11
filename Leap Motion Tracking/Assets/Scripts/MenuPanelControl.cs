using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuPanelControl : MonoBehaviour {

    public GameObject menuCategoryText;
    public GameObject serverInfoDisplay;
    public GameObject sceneSettingsDisplay;

    void Start()
    {
        menuCategoryText.GetComponent<TextMesh>().text = "Please select category";
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            child.gameObject.tag = "Controls";
        }
    }


    public void SelectServerInfoMenu()
    {
        menuCategoryText.GetComponent<TextMesh>().text = "Server Info";
    }

    public void SelectSceneSettingsMenu()
    {
        menuCategoryText.GetComponent<TextMesh>().text = "Scene Settings";
    }
	
	
}
