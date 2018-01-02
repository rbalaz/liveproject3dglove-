using Leap.Unity.Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuPanelControl : MonoBehaviour {

    public GameObject menuCategoryText;
    public GameObject serverInfoAnimator;
    public GameObject sceneSettingsAnimator;

    private enum CurrentItem
    {
        ServerInfo,
        SceneSettings,   
        None
    }
    private CurrentItem currentItem;

    private string textBasic = ". . . . . . . . . . . . . . . . . . . . . . .";
    private string textServer = "Server Info";
    private string textSceneSettings = "Scene Settings";

    void Start()
    {
        currentItem = CurrentItem.None;
        menuCategoryText.GetComponent<TextMesh>().text = textBasic;

        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            child.gameObject.tag = "Controls";
        }
    }

    public void SelectServerInfoMenu()
    {
        if (currentItem == CurrentItem.ServerInfo)
        {
            menuCategoryText.GetComponent<TextMesh>().text = textBasic;
            serverInfoAnimator.GetComponent<TransformTweenBehaviour>().PlayBackward();
            currentItem = CurrentItem.None;
        }
        else
        {
            menuCategoryText.GetComponent<TextMesh>().text = textServer;
            sceneSettingsAnimator.GetComponent<TransformTweenBehaviour>().PlayBackward();
            serverInfoAnimator.GetComponent<TransformTweenBehaviour>().PlayForwardAfterDelay(0.4f);
            currentItem = CurrentItem.ServerInfo;
        }
    }

    public void SelectSceneSettingsMenu()
    {
        if (currentItem == CurrentItem.SceneSettings)
        {
            menuCategoryText.GetComponent<TextMesh>().text = textBasic;
            sceneSettingsAnimator.GetComponent<TransformTweenBehaviour>().PlayBackward();
            currentItem = CurrentItem.None;
        }
        else
        {
            menuCategoryText.GetComponent<TextMesh>().text = textSceneSettings;
            serverInfoAnimator.GetComponent<TransformTweenBehaviour>().PlayBackward();
            sceneSettingsAnimator.GetComponent<TransformTweenBehaviour>().PlayForwardAfterDelay(0.4f);
            currentItem = CurrentItem.SceneSettings;
        }
    }
	
	
}
