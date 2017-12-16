using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Configuration : MonoBehaviour {

    public GameObject mainCameraRig;
    public GameObject stage;

    private float defaultCameraHeight;
	
	public GameObject loader;

    void Start()
    {
        defaultCameraHeight = mainCameraRig.transform.position.y;
		loader.SetActive(false);
    }
	
	void Update()
    {
		if(Input.GetKeyDown("space"))
		{
			ResetScene();
		}
		if(Input.GetKeyDown("escape"))
		{
			Application.Quit();
		}

    }

    public void SetHeight(float value)
    {
        Vector3 pos = mainCameraRig.transform.position;
        mainCameraRig.transform.position = new Vector3(pos.x, defaultCameraHeight + (value-0.5f)/3, pos.z);
    }

    public void GravityOn()
    {
        StartCoroutine(SetKinematic(false));
    }

    public void GravityOff()
    {
		StartCoroutine(SetKinematic(true));
    }

    private IEnumerator SetKinematic(bool state)
    {
        Transform[] allStageChildren = stage.GetComponentsInChildren<Transform>();
		int i = 0;
        foreach (Transform child in allStageChildren)
        {
			
            //Debug.Log(child.gameObject.name + ", " + transform);
            if (! child.CompareTag("Controls"))
            {
                Rigidbody childRb = child.GetComponent<Rigidbody>();
                if (childRb != null) childRb.isKinematic = state;
            }
			i++;
			if (i%10 == 0)
				yield return null;
        }
    }
	
	public void ResetScene()
	{
		
		StartCoroutine(AsyncLoader());
		//SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		
	}
	
	public IEnumerator AsyncLoader(){
		loader.SetActive(true);
		yield return null;
		AsyncOperation asyncLoader = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
		while (!asyncLoader.isDone) {
            //LoadingBar.fillAmount = async.progress / 0.9f; //Async progress returns always 0 here    
            Debug.Log(asyncLoader.progress);
            //textPourcentage.text = LoadingBar.fillAmount + "%"; //I have always 0% because the fillAmount is always 0
            yield return null;
 
        }
		loader.SetActive(false);
	}
}
