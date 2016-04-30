using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using LibNoise.Unity;

[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer), typeof(MeshFilter))]
public class ProceduralCubeFace : MonoBehaviour
{
    public enum CubeFace { Front, Back, Left, Right, Top, Bottom };
    public CubeFace face = CubeFace.Front;
    public ProceduralCubeBody parentBody;
    public Vector3[][] vertices;

    private Mesh mesh;
    private Vector3[] verticesExpanded;
    private int[] triangles;
    private Color32[] vertexColors;
    private Biome biome;
    private float actualRadius;

    private void OnDrawGizmos()
    {
        return;
        if (vertices == null)
        {
            return;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            for (int j = 0; j < vertices[i].Length; j++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(vertices[i][j], 0.1f);
            }
        }
    }

    public void Prepare()
    {
        LibNoise.Unity.Generator.Perlin perlin = new LibNoise.Unity.Generator.Perlin(1f, 1f, 1f, 4, parentBody.parentBody.biome.terrainGenData.seed, 
            QualityMode.Medium);
        actualRadius = Mathf.Lerp(parentBody.parentBody.biome.terrainGenData.lowRadius, parentBody.parentBody.biome.terrainGenData.highRadius,
            (float)perlin.GetValue(transform.position.x, transform.position.y, transform.position.z));
        gameObject.AddComponent<MeshFilter>();
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Sphere Side " + face;

        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
        biome = parentBody.parentBody.biome;
    }

    public void GenerateSphere()
    {
        CreateVertices();
    }

    public void GenerateTerrain()
    {
        SetVertexNoiseHeights();
    }

    public void GenerateTrianglesAndColors()
    {
        int shared1 = 2;                                                                            //vertex locations with only one triangle (2 corners)
        int shared2 = 2;                                                                            //vertex locations with 2 triangles (2 other corners)
        int shared3 = (biome.terrainGenData.gridSize - 1) * 4;                                      //vertex locations with 3 triangles (edges)
        int shared6 = (biome.terrainGenData.gridSize - 1) * (biome.terrainGenData.gridSize - 1);    //vertex locations with 6 triangles (inner vertices)
        verticesExpanded = new Vector3[shared1 + shared2 * 2 + shared3 * 3 + shared6 * 6];
        for (int i = 0, v = 0; i < vertices.Length; i++)
        {
            for (int j = 0; j < vertices[i].Length; j++)
            {
                verticesExpanded[v++] = vertices[i][j];
            }
        }
        vertexColors = new Color32[verticesExpanded.Length];

        CreateTriangles();
    }

    public void Finish()
    {
        CreateColliders();
        mesh.vertices = verticesExpanded;
        mesh.triangles = triangles;
        mesh.colors32 = vertexColors;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private void SetVertexNormalized(int vertexIndex, int repeats, int x, int y, int z)
    {
        Vector3 v = new Vector3(x, y, z) * 2f / biome.terrainGenData.gridSize - Vector3.one;
        float x2 = v.x * v.x;
        float y2 = v.y * v.y;
        float z2 = v.z * v.z;
        Vector3 s;
        s.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
        s.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
        s.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);

        for (int j = 0; j < repeats; j++)
        {
            vertices[vertexIndex][j] = s.normalized * actualRadius;   //TODO: CHANGE TO GENERATED RADIUS
        }
    }

    private void SetVertexNoisy(int vertexIndex, int repeats) 
    {
        for (int i = 0; i < repeats; i++)
        {
            //print("" + heightOffsets[i]);
            vertices[vertexIndex][i] = vertices[vertexIndex][i] + (vertices[vertexIndex][i] - Vector3.zero).normalized *
                parentBody.GetHeightMap()[(int)face][vertexIndex] * biome.terrainGenData.meshHeightMultiplier 
                * biome.meshHeightCurve.Evaluate(parentBody.GetHeightMap()[(int)face][vertexIndex]);
        }
    }

    private void SetVertexNoiseHeights()
    {
        for (int r = 0, i = 0; r <= biome.terrainGenData.gridSize; r++)
        {
            for (int c = 0; c <= biome.terrainGenData.gridSize; c++)
            {
                int duplicateVerts = ((r == 0 && c == 0) || (r == biome.terrainGenData.gridSize && c == biome.terrainGenData.gridSize)) ? 1 :
                    ((r == 0 && c == biome.terrainGenData.gridSize) || (r == biome.terrainGenData.gridSize && c == 0)) ? 2 :
                    (r == 0 || r == biome.terrainGenData.gridSize || (c == 0 || c == biome.terrainGenData.gridSize)) ? 3 : 6;
                int pos = r * (biome.terrainGenData.gridSize + 1) + c;

                SetVertexNoisy(pos, duplicateVerts);
            }
        }
    }

    private void CreateVertices()
    {
        vertices = new Vector3[(biome.terrainGenData.gridSize + 1) * (biome.terrainGenData.gridSize + 1)][];
        for (int r = 0; r <= biome.terrainGenData.gridSize; r++)
        {
            for (int c = 0; c <= biome.terrainGenData.gridSize; c++)
            {
                int duplicateVerts = ((r == 0 && c == 0) || (r == biome.terrainGenData.gridSize && c == biome.terrainGenData.gridSize)) ? 1 :
                    ((r == 0 && c == biome.terrainGenData.gridSize) || (r == biome.terrainGenData.gridSize && c == 0)) ? 2 :
                    (r == 0 || r == biome.terrainGenData.gridSize || (c == 0 || c == biome.terrainGenData.gridSize)) ? 3 : 6;
                int pos = r * (biome.terrainGenData.gridSize + 1) + c;
                vertices[pos] = new Vector3[duplicateVerts];

                switch (face)
                {
                    case CubeFace.Front:
                        SetVertexNormalized(pos, duplicateVerts, c, r, biome.terrainGenData.gridSize);
                        break;
                    case CubeFace.Back:
                        SetVertexNormalized(pos, duplicateVerts, biome.terrainGenData.gridSize - c, biome.terrainGenData.gridSize - r, 0);
                        break;
                    case CubeFace.Left:
                        SetVertexNormalized(pos, duplicateVerts, 0, r, c);
                        break;
                    case CubeFace.Right:
                        SetVertexNormalized(pos, duplicateVerts, biome.terrainGenData.gridSize, r, c);
                        break;
                    case CubeFace.Top:
                        SetVertexNormalized(pos, duplicateVerts, c, biome.terrainGenData.gridSize, r);
                        break;
                    case CubeFace.Bottom:
                        SetVertexNormalized(pos, duplicateVerts, biome.terrainGenData.gridSize - c, 0, biome.terrainGenData.gridSize - r);
                        break;
                }
            }
        }
    }

    private void SetTriangleColor(int heightIndex, int a, int b, int c)
    {
        if (parentBody.GetHeightMap() == null)
        {
            vertexColors[a] = biome.regions[0].lowColor;
            vertexColors[b] = biome.regions[0].lowColor;
            vertexColors[c] = biome.regions[0].lowColor;
            return;
        }

        for (int i = 0; i < biome.regions.Length; i++)
        {
            float height = parentBody.GetHeightMap()[(int)face][heightIndex];
            if (height <= biome.regions[i].highHeight)
            {
                vertexColors[a] = Color32.Lerp(biome.regions[i].lowColor, biome.regions[i].highColor, 
                    Mathf.InverseLerp(biome.regions[i].lowHeight, biome.regions[i].highHeight, height));
                vertexColors[b] = Color32.Lerp(biome.regions[i].lowColor, biome.regions[i].highColor,
                    Mathf.InverseLerp(biome.regions[i].lowHeight, biome.regions[i].highHeight, height));
                vertexColors[c] = Color32.Lerp(biome.regions[i].lowColor, biome.regions[i].highColor,
                    Mathf.InverseLerp(biome.regions[i].lowHeight, biome.regions[i].highHeight, height));
                break;
            }
        }
    }

    private int GetTrueVertexIndex(int i)
    {
        int index = 0;
        for (int v = 0; v < i; v++)
        {
            index += vertices[v].Length;
        }

        return index;
    }

    //Set triangles for 1 quad (indexed at i with vertices specified by vxx)
    private int SetQuad(int[] triangles, int[] vertexCounts, int i, int v00, int v10, int v01, int v11)
    {
        if (i == 0)
            print("vertex indices" + GetTrueVertexIndex(v00) + " " + GetTrueVertexIndex(v01) + " " + GetTrueVertexIndex(v10));

        if (face == CubeFace.Back || face == CubeFace.Right || face == CubeFace.Top)
        {
            triangles[i] = GetTrueVertexIndex(v00) + vertexCounts[v00]++;
            triangles[i + 1] = GetTrueVertexIndex(v01) + vertexCounts[v01]++;
            triangles[i + 4] = GetTrueVertexIndex(v01) + vertexCounts[v01]++;
            triangles[i + 2] = GetTrueVertexIndex(v10) + vertexCounts[v10]++;
            triangles[i + 3] = GetTrueVertexIndex(v10) + vertexCounts[v10]++;
            triangles[i + 5] = GetTrueVertexIndex(v11) + vertexCounts[v11]++;
        }
        else
        {
            triangles[i] = GetTrueVertexIndex(v00) + vertexCounts[v00]++;
            triangles[i + 1] = GetTrueVertexIndex(v10) + vertexCounts[v10]++;
            triangles[i + 4] = GetTrueVertexIndex(v11) + vertexCounts[v11]++;
            triangles[i + 2] = GetTrueVertexIndex(v01) + vertexCounts[v01]++;
            triangles[i + 3] = GetTrueVertexIndex(v10) + vertexCounts[v10]++;
            triangles[i + 5] = GetTrueVertexIndex(v01) + vertexCounts[v01]++;
        }

        SetTriangleColor(v00, triangles[i], triangles[i + 1], triangles[i + 2]);
        SetTriangleColor(v10, triangles[i + 3], triangles[i + 4], triangles[i + 5]);
        return i + 6;
    }

    //BUGGY WHEN biome.terrainGenData.gridSize OR biome.terrainGenData.gridSize = 1
    private void CreateTriangles()
    {
        triangles = new int[biome.terrainGenData.gridSize * biome.terrainGenData.gridSize * 6];
        int[] vertexTriangleCounts = new int[vertices.Length];
        for (int i = 0; i < vertexTriangleCounts.Length; i++) { vertexTriangleCounts[i] = 0; }
        int t = 0;

        //loop through every quad in the mesh
        for (int r = 0; r < biome.terrainGenData.gridSize; r++)
        {
            for (int c = 0; c < biome.terrainGenData.gridSize; c++)
            {
                int v = r * (biome.terrainGenData.gridSize + 1) + c;
                t = SetQuad(triangles, vertexTriangleCounts, t, v, v + 1, v + biome.terrainGenData.gridSize + 1, v + biome.terrainGenData.gridSize + 2);
            }
        }
    }


    public void CreateColliders()
    {
        gameObject.AddComponent<MeshCollider>();
    }
}