using System;
using UnityEngine;

public class RaycastGradient : MonoBehaviour
{
    public Renderer planeRenderer;
    public Gradient heatGradient;
    private Texture2D texture;

    public float minX, maxX, minZ, maxZ;
    public int textureResolution = 512;
    public int brushSize = 5;
    public float heatAmount = 0.5f;
    public float coolAmount = 0.01f;

    private void Start()
    {
        texture = new Texture2D(textureResolution, textureResolution);
        planeRenderer.material.mainTexture = texture;

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, heatGradient.Evaluate(0));
            }
        }

        texture.Apply();
    }

    internal void UpdateHeatTexture(float pointX, float pointZ)
    {
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                float heat = texture.GetPixel(x, y).r;
                heat = Mathf.Max(heat - coolAmount, 0);
                texture.SetPixel(x, y, heatGradient.Evaluate(heat));
            }
        }

        int texX = (int)(Mathf.InverseLerp(minX, maxX, pointX) * texture.width);
        int texZ = (int)(Mathf.InverseLerp(minZ, maxZ, pointZ) * texture.height);

        for (int x = texX - brushSize; x <= texX + brushSize; x++)
        {
            for (int z = texZ - brushSize; z <= texZ + brushSize; z++)
            {
                if (x >= 0 && x < texture.width && z >= 0 && z < texture.height) 
                {
                    double dist = Math.Sqrt(Math.Pow(texX - x, 2) + Math.Pow(texZ - z, 2));
                    double scaledHeatAmount = heatAmount * (1 - dist / (brushSize + 1));
                    float heat = Mathf.Min(texture.GetPixel(x, z).r + Mathf.Max((float)scaledHeatAmount, 0), 1);

                    texture.SetPixel(x, z, heatGradient.Evaluate(heat));
                }
            }
        }

        texture.Apply();
    }
}
