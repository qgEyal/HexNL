
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexGrid : MonoBehaviour
{
    public int gridSize = 10; // number of hexagons in each row/column
    public float hexSize = 1f; // size of each hexagon
    public float hexHeight = 0.75f; // height of each hexagon
    public float noiseScale = 0.1f; // scale of the noise used to generate Voronoi diagram

    private Mesh hexMesh;
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;
    private Color[] colors;

    private void Awake()
    {
        GenerateHexMesh();
        GenerateVoronoiDiagram();
    }

    private void GenerateHexMesh()
    {
        hexMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = hexMesh;

        int numHexes = gridSize * gridSize;
        int numVertices = numHexes * 7;
        int numTriangles = numHexes * 18;

        vertices = new Vector3[numVertices];
        uv = new Vector2[numVertices];
        triangles = new int[numTriangles];
        colors = new Color[numVertices];

        int v = 0;
        int t = 0;

        for (int q = -gridSize + 1; q < gridSize; q++)
        {
            int r1 = Mathf.Max(-gridSize + 1, -q - gridSize + 1);
            int r2 = Mathf.Min(gridSize - 1, -q + gridSize - 1);

            for (int r = r1; r <= r2; r++)
            {
                float x = hexSize * Mathf.Sqrt(3f) * (q + r / 2f);
                float z = hexSize * 3f / 2f * r;
                float y = Random.Range(0f, hexHeight);

                Vector3 center = new Vector3(x, y, z);

                for (int i = 0; i < 6; i++)
                {
                    vertices[v] = center + HexCorner(i);
                    uv[v] = HexUV(i);
                    colors[v] = Color.white;
                    v++;
                }

                triangles[t] = v - 1;
                triangles[t + 1] = v - 4;
                triangles[t + 2] = v - 5;
                triangles[t + 3] = v - 1;
                triangles[t + 4] = v - 5;
                triangles[t + 5] = v - 2;
                triangles[t + 6] = v - 1;
                triangles[t + 7] = v - 2;
                triangles[t + 8] = v - 3;
                triangles[t + 9] = v - 1;
                triangles[t + 10] = v - 3;
                triangles[t + 11] = v - 6;

                t += 12;
            }
        }

        hexMesh.vertices = vertices;
        hexMesh.uv = uv;
        hexMesh.triangles = triangles;
        hexMesh.colors = colors;
    }

    private Vector3 HexCorner(int i)
    {
        float angle = 60f * i;
        return new Vector3(hexSize * Mathf.Cos(Mathf.Deg2Rad * angle), 0f, hexSize * Mathf.Sin(Mathf.Deg2Rad * angle));
    }
        private Vector2 HexUV(int i)
    {
        float angle = 60f * i;
        return new Vector2(hexSize * Mathf.Cos(Mathf.Deg2Rad * angle), hexSize * Mathf.Sin(Mathf.Deg2Rad * angle));
    }

    private void GenerateVoronoiDiagram()
    {
        for (int i = 0; i < vertices.Length; i += 6)
        {
            Vector3 center = (vertices[i] + vertices[i + 1] + vertices[i + 2] + vertices[i + 3] + vertices[i + 4] + vertices[i + 5]) / 6f;
            Vector3 noiseOffset = new Vector3(Random.Range(0f, 1000f), Random.Range(0f, 1000f), Random.Range(0f, 1000f));

            for (int j = i; j < i + 6; j++)
            {
                Vector2 hexUV = uv[j];
                Vector2 noiseUV = new Vector2(hexUV.x + center.x * noiseScale, hexUV.y + center.z * noiseScale);
                float noise = Mathf.PerlinNoise(noiseUV.x + noiseOffset.x, noiseUV.y + noiseOffset.z);

                colors[j] = new Color(noise, noise, noise, 1f);
            }
        }

        hexMesh.colors = colors;

        // Subdivide the hexagons based on the Voronoi diagram
        for (int i = 0; i < vertices.Length; i += 6)
        {
            Vector3 center = (vertices[i] + vertices[i + 1] + vertices[i + 2] + vertices[i + 3] + vertices[i + 4] + vertices[i + 5]) / 6f;
            Vector3 noiseOffset = new Vector3(Random.Range(0f, 1000f), Random.Range(0f, 1000f), Random.Range(0f, 1000f));

            for (int j = i; j < i + 6; j++)
                {
                    Vector2 hexUV = uv[j];
                    Vector2 noiseUV = new Vector2(hexUV.x + center.x * noiseScale, hexUV.y + center.z * noiseScale);
                    float noise = Mathf.PerlinNoise(noiseUV.x + noiseOffset.x, noiseUV.y + noiseOffset.z);

                    if (noise > 0.5f)
                    {
                        // Subdivide the hexagon into three smaller triangles
                        int triangleStartIndex = j - j % 3;
                        int triangleEndIndex = triangleStartIndex + 2;

                        Vector3 v1 = vertices[triangleStartIndex];
                        Vector3 v2 = vertices[triangleStartIndex + 1];
                        Vector3 v3 = vertices[triangleStartIndex + 2];

                        Vector2 uv1 = uv[triangleStartIndex];
                        Vector2 uv2 = uv[triangleStartIndex + 1];
                        Vector2 uv3 = uv[triangleStartIndex + 2];

                        Color color = colors[triangleStartIndex];

                        int newVertexIndex = vertices.Length;
                        int newUVIndex = uv.Length;
                        int newColorIndex = colors.Length;

                        Array.Resize(ref vertices, newVertexIndex + 3);
                        Array.Resize(ref uv, newUVIndex + 3);
                        Array.Resize(ref colors, newColorIndex + 3);

                        Vector3 v4 = (v1 + v2) / 2f;
                        Vector3 v5 = (v2 + v3) / 2f;
                        Vector3 v6 = (v3 + v1                ) / 2f;

                    Vector2 uv4 = (uv1 + uv2) / 2f;
                    Vector2 uv5 = (uv2 + uv3) / 2f;
                    Vector2 uv6 = (uv3 + uv1) / 2f;

                    vertices[newVertexIndex] = v4;
                    vertices[newVertexIndex + 1] = v5;
                    vertices[newVertexIndex + 2] = v6;

                    uv[newUVIndex] = uv4;
                    uv[newUVIndex + 1] = uv5;
                    uv[newUVIndex + 2] = uv6;

                    colors[newColorIndex] = color;
                    colors[newColorIndex + 1] = color;
                    colors[newColorIndex + 2] = color;

                    triangles.Add(j);
                    triangles.Add(newVertexIndex);
                    triangles.Add(newVertexIndex + 2);

                    triangles.Add(newVertexIndex);
                    triangles.Add(j + 1);
                    triangles.Add(newVertexIndex + 1);

                    triangles.Add(newVertexIndex + 1);
                    triangles.Add(j + 2);
                    triangles.Add(newVertexIndex + 2);
                }
            }
        }

        hexMesh.vertices = vertices;
        hexMesh.uv = uv;
        hexMesh.colors = colors;
        hexMesh.triangles = triangles.ToArray();
    }
}


  */ 
