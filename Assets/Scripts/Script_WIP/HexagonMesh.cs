using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*

In this script, the `Start()` function creates a hexagonal mesh using the `CreateHexagonMesh()` function, generates a voronoi diagram 
with random colors using the `GenerateVoronoiTexture()` function, and applies the voronoi texture to the hexagonal meshes as a 2D texture. 
The `CreateHexagonMesh()` function generates a hexagonal mesh with a given radius, and the `GenerateVoronoiTexture()` function generates 
a voronoi diagram with a given number of random points. Note that this implementation of the voronoi diagram generation is not optimized 
and may be slow for large numbers of points.

To use this script, simply attach it to a game object in your Unity scene. The `numHexagons` and `hexagonRadius` variables can be 
adjusted to change the number of hexagons and the size of the hexagons, respectively.
*/
public class HexagonMesh : MonoBehaviour
{
    public int numHexagons = 20;
    public float hexagonRadius = 1f;
    public int subdivisions = 1;

    private Mesh hexagonMesh;
    private Material hexagonMaterial;

    void Start()
    {
        // Create the hexagon mesh
        hexagonMesh = CreateHexagonMesh(hexagonRadius);

        // Create the hexagon material
        hexagonMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

        // Generate a voronoi diagram with random colors
        Texture2D voronoiTexture = GenerateVoronoiTexture(numHexagons);

        // Apply the voronoi texture to the hexagon material
        hexagonMaterial.mainTexture = voronoiTexture;

        // Create the hexagon game objects and assign the material
        for (int i = 0; i < numHexagons; i++)
        {
            Vector3 position = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            GameObject hexagon = new GameObject("Hexagon " + i);
            hexagon.transform.position = position;
            hexagon.transform.rotation = rotation;
            hexagon.AddComponent<MeshFilter>().sharedMesh = hexagonMesh;
            hexagon.AddComponent<MeshRenderer>().sharedMaterial = hexagonMaterial;
        }
    }

    private Mesh CreateHexagonMesh(float radius)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[7];
        vertices[0] = Vector3.zero;
        for (int i = 1; i < 7; i++)
        {
            float angle_deg = 60f * (i - 1);
            float angle_rad = Mathf.PI / 180f * angle_deg;
            vertices[i] = new Vector3(radius * Mathf.Cos(angle_rad), radius * Mathf.Sin(angle_rad), 0f);
        }

        int[] triangles = new int[18];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 6;
        triangles[3] = 1;
        triangles[4] = 2;
        triangles[5] = 6;
        triangles[6] = 2;
        triangles[7] = 3;
        triangles[8] = 6;
        triangles[9] = 3;
        triangles[10] = 4;
        triangles[11] = 6;
        triangles[12] = 4;
        triangles[13] = 5;
        triangles[14] = 6;
        triangles[15] = 5;
        triangles[16] = 1;
        triangles[17] = 6;

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Subdivide mesh
        for (int i = 0; i < subdivisions; i++)
        {
            mesh = SubdivideMesh(mesh);
        }


        return mesh;
    }

    private Texture2D GenerateVoronoiTexture(int numPoints)
    {
        Texture2D texture = new Texture2D(6, 6);

        // Generate random points
        Vector2[] points = new Vector2[numPoints];
        for (int i = 0; i < numPoints; i++)
        {
            points[i] = new Vector2(Random.Range(0, 512), Random.Range(0, 512));
        }

        // Generate voronoi diagram
        for (int y = 0; y < 512; y++)
        {
            for (int x = 0; x < 512; x++)
            {
                // Calculate the distance from the current point to each of the random points
                float[] distances = new float[numPoints];
                for (int i = 0; i < numPoints; i++)
                {
                    distances[i] = Vector2.Distance(new Vector2(x, y), points[i]);
                }

                // Find the index of the closest point
                int closestIndex = 0;
                float closestDistance = distances[0];
                for (int i = 1; i < numPoints; i++)
                {
                    if (distances[i] < closestDistance)
                    {
                        closestIndex = i;
                        closestDistance = distances[i];
                    }
                }

                // Set the pixel color based on the closest point
                texture.filterMode = FilterMode.Point;
                texture.SetPixel(x, y, new Color(Random.value, Random.value, Random.value, 1f));
            }
        }

        texture.Apply();

        return texture;
    }

    Mesh SubdivideMesh(Mesh mesh)
    {
        Mesh subdividedMesh = new Mesh();
       
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Generate new vertices by splitting existing edges
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int index1 = mesh.triangles[i];
            int index2 = mesh.triangles[i + 1];
            int index3 = mesh.triangles[i + 2];

            Vector3 vertex1 = mesh.vertices[index1];
            Vector3 vertex2 = mesh.vertices[index2];
            Vector3 vertex3 = mesh.vertices[index3];

            Vector3 vertex12 = Vector3.Lerp(vertex1, vertex2, 0.5f);
            Vector3 vertex23 = Vector3.Lerp(vertex2, vertex3, 0.5f);
            Vector3 vertex31 = Vector3.Lerp(vertex3, vertex1, 0.5f);

            vertices.Add(vertex1);
            vertices.Add(vertex2);
            vertices.Add(vertex3);
            vertices.Add(vertex12);
            vertices.Add(vertex23);
            vertices.Add(vertex31);
        }

        // Generate new triangles using the new vertices
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int index1 = mesh.triangles[i];
            int index2 = mesh.triangles[i + 1];
            int index3 = mesh.triangles[i + 2];

            int index12 = mesh.vertices.Length + ((i / 3) * 3);
            int index23 = mesh.vertices.Length + ((i / 3) * 3) + 1;
            int index31 = mesh.vertices.Length + ((i / 3) * 3) + 2;

            triangles.Add(index1);
            triangles.Add(index12);
            triangles.Add(index31);

            triangles.Add(index12);
            triangles.Add(index2);
            triangles.Add(index23);

            triangles.Add(index23);
            triangles.Add(index3);
            triangles.Add(index31);

            triangles.Add(index12);
            triangles.Add(index23);
            triangles.Add(index31);
        }

        subdividedMesh.vertices = vertices.ToArray();
        subdividedMesh.triangles = triangles.ToArray();
        subdividedMesh.RecalculateNormals();
        subdividedMesh.RecalculateBounds();

        return subdividedMesh;
    }
}

