using UnityEngine;
using LibNoise.Unity;

public static class PerlinNoise3D
{
    public static float[][] GenerateVertexHeightMap(Vector3[][] vertices, int seed, float scale,
        int octaves, float persistance, float lacunarity, Vector3 offset)
   {
        LibNoise.Unity.Generator.Perlin perlin = new LibNoise.Unity.Generator.Perlin(1f, lacunarity, persistance, octaves, seed, QualityMode.Medium);
        float[][] heightMap = new float[vertices.Length][];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int f = 0; f < heightMap.Length; f++)
        {
            heightMap[f] = new float[vertices[f].Length];

            for (int v = 0; v < heightMap[f].Length; v++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = vertices[f][v].x / scale + offset.x;
                    float sampleY = vertices[f][v].y / scale + offset.y;
                    float sampleZ = vertices[f][v].z / scale + offset.z;

                    float perlinValue = (float)perlin.GetValue(sampleX, sampleY, sampleZ);
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                heightMap[f][v] = noiseHeight;
            }
        }

        for (int f = 0; f < vertices.Length; f++)
        {
            for (int v = 0; v < heightMap[f].Length; v++)
            {
                heightMap[f][v] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, heightMap[f][v]);
            }
        }
        return heightMap;
    }
}
