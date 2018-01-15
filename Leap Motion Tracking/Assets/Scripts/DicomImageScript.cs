using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DicomImageScript : MonoBehaviour
{
    private RawImage dicomImage;
    private int status;

    // 195 posun
    // 361 snimkov

    // Use this for initialization
    void Start()
    {
        dicomImage = gameObject.GetComponent<RawImage>();
        status = 1;
        setStatus(1);
    }

    public void setStatus(float value)
    {
        if (dicomImage == null)
        {
            dicomImage = gameObject.GetComponent<RawImage>();
        }

        Vector3 startPosition = dicomImage.transform.localPosition;
        int intValue = (int)value;
        string imageNumber = intValue + "";

        string numberString = imageNumber.PadLeft(6, '0');
        string path = @"image-" + numberString;
        Texture2D texture = (Texture2D)Resources.Load(path, typeof(Texture2D));

        dicomImage.texture = texture;
        float xPositionChange = (value - status) * 195 / 361;
        Vector3 newPosition = new Vector3(startPosition.x + xPositionChange, startPosition.y, startPosition.z);
        dicomImage.transform.localPosition = newPosition;
        status = intValue;
    }
}
