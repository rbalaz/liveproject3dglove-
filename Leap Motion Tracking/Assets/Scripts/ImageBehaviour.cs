using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageBehaviour : MonoBehaviour {
    private float lastChange;
    int currentImageNumber;

    void Start()
    {
        Debug.Log("Debug 0");
        lastChange = Time.time;
        Debug.Log("Debug 0.1");
        currentImageNumber = 0;
        Debug.Log("Debug 0.2");
    }

    void Update()
    {
        if (Time.time - lastChange > 1)
        {
            currentImageNumber++;
            string numberString = currentImageNumber.ToString().PadLeft(6, '0');
            string path = @"image-" + numberString;
            Texture2D texture = (Texture2D)Resources.Load(path, typeof(Texture2D));
            RawImage image = gameObject.GetComponent<RawImage>();
            image.texture = texture;
            lastChange = Time.time;
            if (currentImageNumber == 361)
                currentImageNumber = 0;
        }
    }

    // Under development, currently freezing Unity on play
    //void Update()
    //{
    //    Debug.Log("Debug 0.3");
    //    Debug.Log(Time.time - lastChange);
    //    if (Time.time - lastChange > 0.5)
    //    {
    //        Debug.Log("Debug 0.4");
    //        currentImageNumber++;
    //        Debug.Log("Debug 0.5");
    //        string numberString = currentImageNumber.ToString().PadLeft(6, '0');
    //        Debug.Log("Debug 0.6");
    //        string path = @"image-" + numberString;
    //        Debug.Log("Debug 1");
    //        Texture2D texture = (Texture2D)Resources.Load(path, typeof(Texture2D));
    //        Debug.Log("Debug 2");
    //        for (int i = 0; i < texture.height; i++)
    //        {
    //            Debug.Log("Debug 3");
    //            for (int j = 0; j < texture.width; j++)
    //            {
    //                Color color = texture.GetPixel(i, j);
    //                Color newColor;
    //                if (color.grayscale < 15)
    //                    newColor = new Color(color.r, color.g, color.b, 0);
    //                else
    //                    newColor = color;
    //                texture.SetPixel(i, j, newColor);
    //            }
    //            Debug.Log("Debug 4");
    //        }

    //        Debug.Log("Debug 5");
    //        RawImage image = gameObject.GetComponent<RawImage>();
    //        image.texture = texture;
    //        lastChange = Time.time;
    //        if (currentImageNumber == 361)
    //            currentImageNumber = 0;
    //    }
    //}
}


