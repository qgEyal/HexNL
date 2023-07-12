/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    public float size = 1.0f;
    public int width = 10;
    public int height = 10;

    public Texture2D voronoiTexture;

    private Mesh hexMesh;
    private Vector3[] vertices;
    private int[] triangles;

    void Awake()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        hexMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = hexMesh;

        // Create vertices and triangles for hexagons
        List<Vector3> vertexList = new List<Vector3>();
        List<int> triangleList = new List<int>();

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                float x = i * size * 1.5f;
                float z = (j + (i % 2) * 0.5f) * size * Mathf.Sqrt(3.0f);

                Vector3 center = new Vector3(x, 0.0f, z);
                vertexList.Add(center);
                for (int k = 0; k < 6; k++)
                {
                    float angleRad = (60 * k) * Mathf.PI / 180.0f;
                    Vector3 vertex = new Vector3(center.x + size * Mathf.Cos(angleRad), 0.0f, center.z + size * Mathf.Sin(angleRad));
                    vertexList.Add(vertex);

                    int v0 = vertexList.Count - 2;
                    int v1 = vertexList.Count - 1;
                    int v2 = ((k == 5) ? vertexList.Count - 6 : v1 + 1);
                    int v3 = ((k == 0) ? vertexList.Count - 5 : v0 - 1);

                    triangleList.Add(v0);
                    triangleList.Add(v1);
                    triangleList.Add(v2);

                    triangleList.Add(v0);
                    triangleList.Add(v2);
                    triangleList.Add(v3);
                }
            }
        }

        vertices = vertexList.ToArray();
        triangles = triangleList.ToArray();

        hexMesh.Clear();
        hexMesh.vertices = vertices;
        hexMesh.triangles = triangles;
        hexMesh.RecalculateNormals();

        // Create Voronoi diagram
        if (voronoiTexture == null)
        {
            voronoiTexture = new Texture2D(256, 256);
        }

        Color[] colors = new Color[256 * 256];
        Voronoi voronoi = new Voronoi();
        SiteList sites = new SiteList();
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];
            Vector2 point = new Vector2(vertex.x, vertex.z);
            points.Add(point);
            sites.Add(new Site(point.x, point.y));
        }

        Diagram diagram = voronoi.ComputeDiagram(sites);

        for (int y = 0; y < 256; y++)
        {
            for (int x = 0; x < 256; x++)
            {
                Vector2 point = new Vector2(x / 256.0f * width * size * 1.5f, y / 256.0f * Mathf.Sqrt(3.0f) * size);
                          int index = diagram.GetSiteNumber(point.x, point.y);
                if (index == -1)
                {
                    colors[y * 256 + x] = Color.white;
                }
                else
                {
                    colors[y * 256 + x] = Color.HSVToRGB((float)index / vertices.Length, 1.0f, 1.0f);
                }
            }
        }

        voronoiTexture.SetPixels(colors);
        voronoiTexture.Apply();

        // Apply Voronoi diagram as texture
        Material material = new Material(Shader.Find("Sprites/Default"));
        material.SetTexture("_MainTex", voronoiTexture);

        GetComponent<MeshRenderer>().material = material;
    }
}

*/

