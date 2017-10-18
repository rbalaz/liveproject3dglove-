using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour {

    public Text sliderText;

	// Use this for initialization
	void Start () {

	}

    public void UpdateSliderText(float value)
    {
        sliderText.text = value.ToString("000");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
