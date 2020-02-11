using UnityEngine;

namespace Generator.Test
{
    public class MapDisplay : MonoBehaviour
    {
        public Renderer textureRenderer;

        public void DrawColorMap(Color[] colorMap, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            /*Color[] colourMap = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                }
            }*/

            texture.SetPixels(colorMap);
            texture.Apply();

            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(width, 1, height);
        }
    }
}