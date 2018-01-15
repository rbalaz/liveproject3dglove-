using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageBehaviour : MonoBehaviour {
    private Transform[] imageComponents;
    private bool updateFinished;

    private void Start()
    {
        imageComponents = gameObject.GetComponentsInChildren<Transform>();
        flushChildren();
        updateFinished = false;
    }

    private void flushChildren()
    {
        List<Transform> images = new List<Transform>();
        for (int i = 0; i < imageComponents.Length; i++)
        {
            if (imageComponents[i].parent.name == "AbdomenModel" && imageComponents[i].name.Contains("RawImage"))
                images.Add(imageComponents[i]);
        }

        imageComponents = images.ToArray();
    }

    void Update()
    {
        if (!updateFinished)
        {
            StartCoroutine(loadImages());
            updateFinished = true;
        }
    }

    private IEnumerator loadImages()
    {
        for (int l = 0; l < imageComponents.Length; l++)
        {
            string objectName = imageComponents[l].transform.name;
            string[] parts = objectName.Split(' ');
            string imageNumber = parts[1].Substring(1, parts[1].Length - 2);

            string numberString = imageNumber.PadLeft(6, '0');
            string path = @"image-" + numberString;
            //string asset = "Assets/Resources/" + path + ".jpg";
            //Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(asset, typeof(Texture2D));
            Texture2D texture = (Texture2D)Resources.Load(path, typeof(Texture2D));
            //for (int i = 0; i < texture.height; i++)
            //{
            //    for (int j = 0; j < texture.width; j++)
            //    {
            //        Color color = texture.GetPixel(i, j);
            //        Color newColor;
            //        if (color.grayscale < 15)
            //            newColor = new Color(color.r, color.g, color.b, 0);
            //        else
            //            newColor = new Color(color.r, color.g, color.b, 1);
            //        texture.SetPixel(i, j, newColor);
            //    }
            //}
            RawImage image = imageComponents[l].GetComponent<RawImage>();
            image.texture = texture;

            int parsedNumber = int.Parse(imageNumber);
            Vector3 position = imageComponents[l].transform.position;
            position.x += (parsedNumber - 1) * 0.002f;
            imageComponents[l].transform.position = position;

            yield return null;
        }
    }
}


