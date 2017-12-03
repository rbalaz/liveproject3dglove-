using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageBehaviour : MonoBehaviour {
    private float lastChange;
    int currentImageNumber;

    void Start()
    {
        lastChange = Time.time;
        currentImageNumber = 0;
    }

    void Update () {
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

}


