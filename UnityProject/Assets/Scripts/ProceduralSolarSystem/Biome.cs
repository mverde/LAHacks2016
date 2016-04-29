using UnityEngine;

[System.Serializable]
public class Biome
{
    public int lowDistFromParent, highDistFromParent;
    public bool water;
    public AnimationCurve meshHeightCurve;
    public Material material;

    public TerrainData[] regions;
    public TerrainGenData terrainGenData;
}

[System.Serializable]
public struct TerrainData
{
    public string name;
    public float lowHeight;
    public float highHeight;
    public Color32 lowColor;
    public Color32 highColor;
}

[System.Serializable]
public struct TerrainGenData
{
    // Cube Sphere generation
    public int gridSize;
    public float lowRadius, highRadius;

    // Procedural Noise Generation
    [Range(0, 6)]
    public int levelOfDetail;
    public float noiseScale;
    public bool autoUpdate;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    // Modifiers
    public int seed;
    public Vector3 offset;
    public float meshHeightMultiplier;
}
