using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuPanelControl : MonoBehaviour {

    public GameObject menuCategoryText;
    public GameObject serverInfoDisplay;
    public GameObject sceneSettingsDisplay;

    // Use this for initialization
    void Start()
    {
        menuCategoryText.GetComponent<TextMesh>().text = "Please select category";
    }

    // Update is called once per frame
    void Update()
    {

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
