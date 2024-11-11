using UnityEngine;

public class TerrainTextureGenerator : MonoBehaviour
{
    [Header("Texture Settings")]
    public int textureSize = 512; 
    public Color grassColor = new Color(0.4f, 0.6f, 0.2f); 
    public Color dirtColor = new Color(0.6f, 0.5f, 0.2f);  

    [Header("Noise Settings")]
    public float noiseScale = 20f;      
    public float patchiness = 0.5f;     

    void Start()
    {
        Texture2D warZoneTexture = GenerateWarZoneTexture();
        ApplyTexture(warZoneTexture);
    }

    Texture2D GenerateWarZoneTexture()
    {
        Texture2D texture = new Texture2D(textureSize, textureSize);

        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                
                float noise = Mathf.PerlinNoise(x / noiseScale, y / noiseScale);

                
                Color pixelColor = Color.Lerp(grassColor, dirtColor, Mathf.Clamp01(noise + patchiness));

                texture.SetPixel(x, y, pixelColor);
            }
        }

        texture.Apply();
        return texture;
    }

    void ApplyTexture(Texture2D texture)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.mainTexture = texture;
        }
    }

    //void Update ()
    //{
    //    exture2D warZoneTexture = GenerateWarZoneTexture();
    //    ApplyTexture(warZoneTexture);
    //}
}
