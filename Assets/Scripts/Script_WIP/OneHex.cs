using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*

Explanation:

The `GenerateMesh()` function creates a hexagonal mesh with the specified radius and number of subdivisions per side. 
The mesh is generated using a series of vertices and triangles, where each hexagon is made up of seven vertices and six triangles. 
The `subdivisions` parameter determines how many times each edge of the hexagon is subdivided.

The `GenerateVoronoiTexture()` function generates a Voronoi diagram using the same hexagonal grid as the mesh. The diagram is generated 
by placing random points within each hexagon, and assigning a color to each point. The color is then assigned to each pixel in the 
Voronoi diagram based on its distance to the closest point.

The Voronoi diagram is then applied as a texture to the hexagonal mesh using the `_MainTex` property of the mesh's material. 
The mesh is subdivided again to match the pattern of the Voronoi

*/
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class OneHex : MonoBehaviour
{
    public int radius = 5;  // Hexagon radius in number of hexagons
    public int subdivisions = 1;  // Number of subdivisions for each hexagon side

    private MeshFilter meshFilter;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        GenerateMesh();
        GenerateVoronoiTexture();
    }

    void GenerateMesh()
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Generate hexagon vertices
        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);

            for (int r = r1; r <= r2; r++)
            {
                float x = q * 1.5f;
                float z = r * Mathf.Sqrt(3) + q * Mathf.Sqrt(3) / 2;
                vertices.Add(new Vector3(x, 0, z));
            }
        }

        // Generate hexagon triangles
        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);

            for (int r = r1; r <= r2; r++)
            {
                int index = (q + radius) * (2 * radius + 1) + r + radius;
                if (r < r2 && q < radius)
                {
                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2 * radius + 2);
                }
                if (r < r2 && q > -radius)
                {
                    triangles.Add(index);
                    triangles.Add(index + 2 * radius + 2);
                    triangles.Add(index + 2 * radius + 1);
                }
                if (q == radius && r == r2)
                {
                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2 * radius + 2);
                }
                if (q == -radius && r == r1)
                {
                    triangles.Add(index);
                    triangles.Add(index + 2 * radius + 1);
                    triangles.Add(index + 2 * radius + 2);
                }
            }
        }

        // Subdivide hexagon edges
        for (int s = 0; s < subdivisions; s++)
        {
            int numVertices = vertices.Count;

            for (int i = 0; i < numVertices; i++)
            {
                Vector3 v1 = vertices[i];
                Vector3 v2 = vertices[(i + 1) % numVertices];
                Vector3 v3 = vertices[(i + 2) % numVertices];

                Vector3 newV1 = Vector3.Lerp(v1, v2, 0.5f);
                Vector3 newV2 = Vector3.Lerp(v2, v3, 0.5f);

                vertices.Add(newV1);
                vertices.Add(newV2);

                if (i < numVertices - 1)
                {
                    triangles.Add(i * 2);
                    triangles.Add(i * 2 + 1);
                    triangles.Add(i * 2 + 2);
                    triangles.Add(i * 2 + 1);
                    triangles.Add(i * 2 + 3);
                    triangles.Add(i * 2 + 2);

                    triangles.Add(i * 2);
                    triangles.Add(i * 2 + 2);
                    triangles.Add((i + numVertices) * 2);

                    triangles.Add((i + numVertices) * 2);
                    triangles.Add(i * 2 + 2);
                    triangles.Add((i + numVertices) * 2 + 2);
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }

    void GenerateVoronoiTexture()
    {
        Texture2D voronoiTexture = new Texture2D(512, 512);
        Color[] colors = new Color[voronoiTexture.width * voronoiTexture.height];

        // Generate Voronoi diagram
        List<Vector2> points = new List<Vector2>();

        for (int q = -radius - subdivisions; q <= radius + subdivisions; q++)
        {
            int r1 = Mathf.Max(-radius - subdivisions, -q - radius - subdivisions);
            int r2 = Mathf.Min(radius + subdivisions, -q + radius + subdivisions);

            for (int r = r1; r <= r2; r++)
            {
                float x = q * 1.5f;
                float z = r * Mathf.Sqrt(3) + q * Mathf.Sqrt(3) / 2;
                points.Add(new Vector2(x, z));
            }
        }

        for (int x = 0; x < voronoiTexture.width; x++)
        {
            for (int y = 0; y < voronoiTexture.height; y++)
            {
                float minDistance = Mathf.Infinity;
                int minIndex = -1;

                for (int i = 0; i < points.Count; i++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), points[i]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minIndex = i;
                    }
                }

                colors[y * voronoiTexture.width + x] = Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1);

                if (minIndex >= 0)
                {
                    colors[y * voronoiTexture.width + x] = colors[minIndex];
                }
            }
        }

        voronoiTexture.SetPixels(colors);
        voronoiTexture.Apply();

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.SetTexture("_MainTex", voronoiTexture);
    }
}

