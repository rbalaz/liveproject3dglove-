using UnityEngine;
using UnityEngine.UI;

public class ImageBehaviour : MonoBehaviour {
    void Start()
    {
        string objectName = gameObject.transform.name;
        string[] parts = objectName.Split(' ');
        string imageNumber = parts[1].Substring(1, parts[1].Length - 2);

        string numberString = imageNumber.PadLeft(6, '0');
        string path = @"image-" + numberString;
        Texture2D texture = (Texture2D)Resources.Load(path, typeof(Texture2D));
        for (int i = 0; i < texture.height; i++)
        {
            for (int j = 0; j < texture.width; j++)
            {
                Color color = texture.GetPixel(i, j);
                Color newColor;
                if (color.grayscale < 15)
                    newColor = new Color(color.r, color.g, color.b, 0);
                else
                    newColor = new Color(color.r, color.g, color.b, 1);
                texture.SetPixel(i, j, newColor);
            }
        }
        RawImage image = gameObject.GetComponent<RawImage>();
        image.texture = texture;

        int parsedNumber = int.Parse(imageNumber);
        Vector3 position = gameObject.transform.position;
        position.x += (parsedNumber - 1) * 0.002f;
        gameObject.transform.position = position;
    }
}


