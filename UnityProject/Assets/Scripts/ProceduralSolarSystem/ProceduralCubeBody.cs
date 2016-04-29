using UnityEngine;
using System.Threading;

public class ProceduralCubeBody : MonoBehaviour
{
    public CelestialBody parentBody;

    private float[][] heightMap = null;
    private Vector3[][] faceVertices = null;
    private GameObject[] faces = null;
    private ProceduralCubeFace[] faceComponents = null;

    private void Awake()
    {
        Generate();
    }

    public void Prepare()
    {
        if (faces != null)
        {
            for (int i = 0; i < faces.Length; i++)
            {
                if (faces[i] != null)
                {
                    DestroyImmediate(faces[i]);
                }
            }
        }
        faceComponents = new ProceduralCubeFace[6];
        faces = new GameObject[6];
        Biome biome = parentBody.biome;

        bool shouldGenerateTerrain = !(biome.terrainGenData.lacunarity == 0f || biome.terrainGenData.persistance == 0f || 
            biome.terrainGenData.meshHeightMultiplier == 0f || biome.terrainGenData.octaves == 0);
        if (shouldGenerateTerrain)
        {
            faceVertices = new Vector3[6][];
        }

        for (int i = 0; i < faces.Length; i++)
        {
            faces[i] = new GameObject();
            faces[i].AddComponent<ProceduralCubeFace>();
            faceComponents[i] = faces[i].GetComponent<ProceduralCubeFace>();
            faces[i].transform.parent = transform;
            faces[i].transform.localPosition = Vector3.zero;
            faces[i].GetComponent<MeshRenderer>().material = biome.material;
            faces[i].layer = 8; //Ground layer
            faces[i].name = (ProceduralCubeFace.CubeFace)i + " Face";

            faceComponents[i].parentBody = this;
            faceComponents[i].face = (ProceduralCubeFace.CubeFace)i;
            faceComponents[i].Prepare();
        }
    }

    public void Generate()
    {
        Biome biome = parentBody.biome;
        bool shouldGenerateTerrain = !(biome.terrainGenData.lacunarity == 0f || biome.terrainGenData.persistance == 0f ||
            biome.terrainGenData.meshHeightMultiplier == 0f || biome.terrainGenData.octaves == 0);

        for (int i = 0; i < faces.Length; i++)
        {
            faceComponents[i].GenerateSphere();

            if (shouldGenerateTerrain)
            {
                faceVertices[i] = new Vector3[faceComponents[i].vertices.Length];
                for (int v = 0; v < faceVertices[i].Length; v++)
                {
                    faceVertices[i][v] = faceComponents[i].vertices[v][0];
                }
            }
        }

        heightMap = null;
        if (shouldGenerateTerrain)
        {
            heightMap = PerlinNoise3D.GenerateVertexHeightMap(faceVertices, biome.terrainGenData.seed, biome.terrainGenData.noiseScale, biome.terrainGenData.octaves, 
                biome.terrainGenData.persistance, biome.terrainGenData.lacunarity, biome.terrainGenData.offset);
        }
        for (int i = 0; i < faceComponents.Length; i++)
        {
            if (shouldGenerateTerrain)
            {
                faceComponents[i].GenerateTerrain();
            }
            faceComponents[i].GenerateTrianglesAndColors();
        }
    }

    public void Finish()
    {
        for (int i = 0; i < faceComponents.Length; i++)
        {
            faceComponents[i].Finish();
        }
        heightMap = null;
        faceVertices = null;
    }

    public float[][] GetHeightMap()
    {
        return heightMap;
    }
}
