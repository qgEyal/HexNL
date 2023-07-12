
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMesher : MonoBehaviour {

    public float radius = 1f; // Radius of each hexagon
    public int numRings = 3; // Number of hexagon rings
    public int subDivisions = 1; // Number of subdivisions for the Voronoi diagram

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh hexMesh;

    void Start () {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        GenerateMesh();
    }

    void GenerateMesh () {
        hexMesh = new Mesh();

        // Generate vertices
        List<Vector3> vertices = new List<Vector3>();
        for (int ring = 0; ring <= numRings; ring++) {
            float y = ring * Mathf.Sqrt(3f) * radius;
            int numHexagons = 6 * (ring + 1);
            for (int i = 0; i < numHexagons; i++) {
                float angle = i * Mathf.PI / 3f;
                float x = Mathf.Cos(angle) * (ring + 1) * radius * 1.5f;
                float z = Mathf.Sin(angle) * (ring + 1) * radius * 2f;
                if (ring % 2 == 1) {
                    z += radius * Mathf.Sqrt(3f);
                }
                vertices.Add(new Vector3(x, y, z));
            }
        }
        hexMesh.vertices = vertices.ToArray();

        // Generate triangles
        List<int> triangles = new List<int>();
        for (int ring = 0; ring < numRings; ring++) {
            int ringStart = ring * 6 * (ring + 1);
            int nextRingStart = (ring + 1) * 6 * (ring + 2);
            for (int i = 0; i < 6 * (ring + 1); i++) {
                int j = i + 6 * (ring + 1);
                if (i % (ring + 1) != ring) {
                    triangles.Add(ringStart + i);
                    triangles.Add(nextRingStart + i + 1);
                    triangles.Add(nextRingStart + i);
                }
                if (i % (ring + 1) != 0) {
                    triangles.Add(ringStart + i);
                    triangles.Add(ringStart + i + 1);
                    triangles.Add(nextRingStart + i + 1);
                }
            }
        }
        hexMesh.triangles = triangles.ToArray();

        // Generate UVs
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < vertices.Count; i++) {
            uvs.Add(new Vector2(vertices[i].x, vertices[i].z));
        }
        hexMesh.uv = uvs.ToArray();

        meshFilter.mesh = hexMesh;

        // Generate Voronoi diagram
        Color[] colors = new Color[vertices.Count];
        List<Vector2> sites = new List<Vector2>();
        for (int i = 0; i < vertices.Count; i++) {
            sites.Add(new Vector2(vertices[i].x, vertices[i].z));
        }
        Voronoi voronoi = new Voronoi(sites);
        List<List<Vector2>> regions = voronoi.Regions();
        for (int i = 0; i < regions.Count; i++) {
            Color randomColor = new Color(Random.value, Random.value, Random.value, 1f);
            List<Vector2> region = regions[i];
            for (int j = 0; j < region.Count; j++) {
            int index = vertices.FindIndex(v => v.x == region[j].x && v.z == region[j].y);
            colors[index] = randomColor;
            }
        }
            // Apply Voronoi diagram as 2D texture
    Texture2D texture = new Texture2D(512, 512);
    texture.filterMode = FilterMode.Point;
    for (int i = 0; i < vertices.Count; i++) {
        texture.SetPixel((int) (vertices[i].x + 256), (int) (vertices[i].z + 256), colors[i]);
    }
    texture.Apply();
    meshRenderer.material.mainTexture = texture;

    // Subdivide mesh to match Voronoi diagram pattern
    for (int i = 0; i < subDivisions; i++) {
        SubdivideMesh();
        }
    }

    void SubdivideMesh () {
        // Create a new list of vertices and triangles
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();

        // Subdivide each triangle
        for (int i = 0; i < hexMesh.triangles.Length; i += 3) {
            // Get the vertices of the current triangle
            Vector3 v1 = hexMesh.vertices[hexMesh.triangles[i]];
            Vector3 v2 = hexMesh.vertices[hexMesh.triangles[i + 1]];
            Vector3 v3 = hexMesh.vertices[hexMesh.triangles[i + 2]];

            // Calculate the midpoints of each edge
            Vector3 v12 = (v1 + v2) / 2f;
            Vector3 v23 = (v2 + v3) / 2f;
            Vector3 v31 = (v3 + v1) / 2f;

            // Add the new vertices to the list
            newVertices.Add(v1);
            newVertices.Add(v12);
            newVertices.Add(v31);
            newVertices.Add(v2);
            newVertices.Add(v23);
            newVertices.Add(v12);
            newVertices.Add(v3);
            newVertices.Add(v31);
            newVertices.Add(v23);
            newVertices.Add(v12);
            newVertices.Add(v23);
            newVertices.Add(v31);

            // Add the new triangles to the list
            int startIndex = newVertices.Count - 12;
            newTriangles.Add(startIndex);
            newTriangles.Add(startIndex + 1);
            newTriangles.Add(startIndex + 2);
            newTriangles.Add(startIndex + 3);
            newTriangles.Add(startIndex + 4);
            newTriangles.Add(startIndex + 5);
            newTriangles.Add(startIndex + 6);
            newTriangles.Add(startIndex + 7);
            newTriangles.Add(startIndex + 8);
            newTriangles.Add(startIndex + 9);
            newTriangles.Add(startIndex + 10);
            newTriangles.Add(startIndex + 11);
        }

        // Update the mesh with the new vertices and triangles
        hexMesh.vertices = newVertices.ToArray();
        hexMesh.triangles = newTriangles.ToArray();
        hexMesh.RecalculateNormals();
        hexMesh.RecalculateBounds();

        // Update the UVs to match the new vertices
        List<Vector2> newUVs = new List<Vector2>();
        for (int i = 0; i < newVertices.Count; i++) {
            newUVs.Add(new Vector2(newVertices[i].x / size + 0.5f, newVertices[i].z / size + 0.5f));
        }
        hexMesh.uv = newUVs.ToArray();
    }
}


*/