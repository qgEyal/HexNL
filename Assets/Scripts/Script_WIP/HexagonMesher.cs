/*
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class HexagonMesher : MonoBehaviour
{
    public int numColumns = 10; // number of hexagons in a column
    public int numRows = 10; // number of hexagons in a row
    public float hexagonRadius = 1.0f; // radius of a hexagon
    public Material material; // material for the hexagon mesh

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh hexagonMesh;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uvs;

    void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        hexagonMesh = new Mesh();
        meshFilter.mesh = hexagonMesh;
        meshRenderer.material = material;

        GenerateHexagonMesh();
        ApplyVoronoiTexture();
    }

    void GenerateHexagonMesh()
    {
        List<Vector3> verticesList = new List<Vector3>();
        List<int> trianglesList = new List<int>();
        List<Vector2> uvsList = new List<Vector2>();

        float xStep = hexagonRadius * Mathf.Sqrt(3.0f);
        float yStep = hexagonRadius * 1.5f;

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                float x = col * xStep;
                float y = row * yStep;

                if (col % 2 == 1)
                {
                    y += hexagonRadius * 0.75f;
                }

                Vector3 center = new Vector3(x, 0.0f, y);

                for (int i = 0; i < 6; i++)
                {
                    Vector3 vertex = center + hexagonRadius * HexagonCorner(i);
                    verticesList.Add(vertex);
                }

                int baseIndex = row * numColumns + col;
                for (int i = 0; i < 6; i++)
                {
                    trianglesList.Add(baseIndex * 6 + i);
                    trianglesList.Add(baseIndex * 6 + (i + 1) % 6);
                    trianglesList.Add(baseIndex * 6 + 6 + i);

                    trianglesList.Add(baseIndex * 6 + (i + 1) % 6);
                    trianglesList.Add(baseIndex * 6 + 6 + (i + 1) % 6);
                    trianglesList.Add(baseIndex * 6 + 6 + i);
                }

                uvsList.AddRange(new Vector2[]{
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f + 0.5f * Mathf.Cos(Mathf.PI / 3.0f), 0.5f + 0.5f * Mathf.Sin(Mathf.PI / 3.0f)),
                    new Vector2(0.5f + 0.5f * Mathf.Cos(2.0f * Mathf.PI / 3.0f), 0.5f + 0.5f * Mathf.Sin(2.0f * Mathf.PI / 3.0f)),
                    new Vector2(0.5f - 0.5f * Mathf.Cos(2.0f * Mathf.PI / 3.0f), 0.5f + 0.5f * Mathf.Sin(2.0f * Mathf.PI / 3.0f)),
                    new Vector2(0.5f - 0.5f * Mathf.Cos(Mathf.PI / 3.0f), 0.5f + 0.5f * Mathf.Sin(Mathf.PI / 3.0f)),
                    new Vector2(0.5f - 0.5f * Mathf.Cos(2.0f * Mathf.PI / 3.0f), 0.5f + 0.5f * Mathf.Sin(2.0f * Mathf.PI / 3.0f)),
                    });
            }
        }
            vertices = verticesList.ToArray();
        triangles = trianglesList.ToArray();
        uvs = uvsList.ToArray();

        hexagonMesh.vertices = vertices;
        hexagonMesh.triangles = triangles;
        hexagonMesh.uv = uvs;
    }

    void ApplyVoronoiTexture()
    {
        Texture2D texture = new Texture2D(numColumns, numRows);

        Vector2[] points = GenerateRandomPoints(numColumns * numRows, new Vector2(0, 0), new Vector2(numColumns, numRows));
        int[] voronoiDiagram = GenerateVoronoiDiagram(points, numColumns, numRows);
        Color[] colors = new Color[numColumns * numRows];

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                int index = row * numColumns + col;
                int siteIndex = voronoiDiagram[index];

                Color color = Random.ColorHSV();
                if (siteIndex >= 0)
                {
                    color = colors[siteIndex];
                }
                colors[index] = color;
            }
        }

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                int index = row * numColumns + col;
                texture.SetPixel(col, row, colors[index]);
            }
        }

        texture.Apply();

        meshRenderer.material.mainTexture = texture;

        // Subdivide the hexagonal mesh to match the Voronoi diagram pattern
        SubdivideMesh(voronoiDiagram);
    }

    void SubdivideMesh(int[] voronoiDiagram)
    {
        List<Vector3> newVerticesList = new List<Vector3>();
        List<int> newTrianglesList = new List<int>();
        List<Vector2> newUvsList = new List<Vector2>();

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                int index = row * numColumns + col;
                int siteIndex = voronoiDiagram[index];

                if (siteIndex >= 0)
                {
                    Vector2 site = HexagonCenter(col, row);
                    Vector2[] corners = new Vector2[6];
                    for (int i = 0; i < 6; i++)
                    {
                        corners[i] = HexagonCorner(i) + site;
                    }

                    int baseIndex = newVerticesList.Count;
                    for (int i = 0; i < 6; i++)
                    {
                        newVerticesList.Add(corners[i]);
                        newUvsList.Add(uvs[i]);
                    }

                    newTrianglesList.AddRange(new int[]{
                        baseIndex, baseIndex + 2, baseIndex + 1,
                        baseIndex, baseIndex + 3, baseIndex + 2,
                        baseIndex, baseIndex + 4, baseIndex + 3,
                        baseIndex, baseIndex + 5, baseIndex + 4
                        });
                    Color color = meshRenderer.material.mainTexture.GetPixel(col, row);
                    for (int i = 0; i < 6; i++)
                    {
                        newUvsList.Add(uvs[i] + new Vector2(col, row));
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        newUvsList[baseIndex + i] = HexagonCorner(i);
                    }
                }
            }
        }

        hexagonMesh.vertices = newVerticesList.ToArray();
        hexagonMesh.triangles = newTrianglesList.ToArray();
        hexagonMesh.uv = newUvsList.ToArray();
    }

    Vector2[] GenerateRandomPoints(int numPoints, Vector2 minBounds, Vector2 maxBounds)
    {
        Vector2[] points = new Vector2[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            float x = UnityEngine.Random.Range(minBounds.x, maxBounds.x);
            float y = UnityEngine.Random.Range(minBounds.y, maxBounds.y);
            points[i] = new Vector2(x, y);
        }
        return points;
    }

    int[] GenerateVoronoiDiagram(Vector2[] points, int numColumns, int numRows)
    {
        int[] diagram = new int[numColumns * numRows];
        float minDist;
        int closestSite;

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                minDist = Mathf.Infinity;
                closestSite = -1;

                for (int i = 0; i < points.Length; i++)
                {
                    float dist = Vector2.Distance(new Vector2(col, row), points[i]);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestSite = i;
                    }
                }

                diagram[row * numColumns + col] = closestSite;
            }
        }

        return diagram;
    }

    Vector2 HexagonCenter(int col, int row)
    {
        float x = col * (1.5f * hexagonRadius);
        float y = row * (2.0f * hexagonRadius - hexagonSide);
        if (col % 2 == 1)
        {
            y += hexagonRadius - hexagonSide / 2.0f;
        }
        return new Vector2(x, y);
    }

    Vector2 HexagonCorner(int index)
    {
        float angle_deg = 60.0f * index;
        float angle_rad = Mathf.PI / 180.0f * angle_deg;
        return new Vector2(hexagonRadius * Mathf.Cos(angle_rad), hexagonRadius * Mathf.Sin(angle_rad));
    }
}

                    

*/