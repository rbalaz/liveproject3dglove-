using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DicomImageScript : MonoBehaviour
{
    private RawImage dicomImage;
	private Vector3 startLocation;

    // 195 posun
    // 361 snimkov

    // Use this for initialization
    void Start()
    {
        dicomImage = gameObject.GetComponent<RawImage>();
		startLocation = dicomImage.transform.localPosition;
        setStatus(1);
    }

    public void setStatus(float value)
    {
        if (dicomImage == null)
        {
            dicomImage = gameObject.GetComponent<RawImage>();
			startLocation = dicomImage.transform.localPosition;
        }

        int intValue = (int)value;
        string imageNumber = intValue + "";

        string numberString = imageNumber.PadLeft(6, '0');
        string path = @"image-" + numberString;
        Texture2D texture = (Texture2D)Resources.Load(path, typeof(Texture2D));

        dicomImage.texture = texture;
        float xPositionChange = (value - 1) * 195 / 361;
        Vector3 newPosition = new Vector3(startLocation.x + xPositionChange, startLocation.y, startLocation.z);
        dicomImage.transform.localPosition = newPosition;
    }
}
