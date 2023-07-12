using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

To use this script, create an empty game object in your Unity scene and attach the `HexagonMesh` script to it. 
Assign a material to the `Material` field in the inspector, and adjust the `subdivisions`, `numSites`, and `radius` fields as desired. 
When you play the scene, a hexagonal mesh will be created, and a Voronoi diagram with random colors will be generated and applied as a 
2D texture to the mesh. The mesh will be subdivided to match the Voronoi diagram pattern based on the number of subdivisions specified.

*/
public class TwoHex : MonoBehaviour
{
    public int subdivisions = 1;
    public int numSites = 20;
    public float radius = 1f;
    public Material material;

    private Mesh hexagonMesh;

    void Start()
    {
        GenerateHexagonMesh();
        ApplyVoronoiDiagram();
    }

    void GenerateHexagonMesh()
    {
        hexagonMesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Generate vertices
        for (int i = 0; i < 6; i++)
        {
            float angle = 60f * i;
            Vector3 vertex = new Vector3(radius * Mathf.Cos(Mathf.Deg2Rad * angle), 0f, radius * Mathf.Sin(Mathf.Deg2Rad * angle));
            vertices.Add(vertex);
        }

        // Generate triangles
        for (int w = 0; w < 7; w++)
        {
            triangles.Add(0);
            triangles.Add(w + 1);
            triangles.Add(w == 5 ? 1 : w + 2);
        }

        hexagonMesh.vertices = vertices.ToArray();
        hexagonMesh.triangles = triangles.ToArray();
        hexagonMesh.RecalculateNormals();
        hexagonMesh.RecalculateBounds();

        // Create game object with mesh renderer and mesh filter components
        GameObject hexagonObject = new GameObject("Hexagon");
        hexagonObject.transform.position = Vector3.zero;
        hexagonObject.AddComponent<MeshRenderer>().material = material;
        hexagonObject.AddComponent<MeshFilter>().mesh = hexagonMesh;

        // Subdivide mesh
        for (int i = 0; i < subdivisions; i++)
        {
            hexagonMesh = SubdivideMesh(hexagonMesh);
        }

        // Assign the subdivided mesh to the mesh filter component
        hexagonObject.GetComponent<MeshFilter>().mesh = hexagonMesh;
    }

    void ApplyVoronoiDiagram()
    {
        List<Vector2> sites = new List<Vector2>();

        // Generate random sites
        for (int i = 0; i < numSites; i++)
        {
            float x = Random.Range(-radius, radius);
            float z = Random.Range(-radius, radius);
            sites.Add(new Vector2(x, z));
        }

        // Generate Voronoi diagram
        List<Color> colors = new List<Color>();
        Texture2D voronoiTexture = new Texture2D(512, 512);
        for (int i = 0; i < voronoiTexture.width; i++)
        {
            for (int j = 0; j < voronoiTexture.height; j++)
            {
                Vector2 point = new Vector2(i - (voronoiTexture.width / 2), j - (voronoiTexture.height / 2));
                int siteIndex = GetNearestSiteIndex(point, sites);
                colors.Add(Random.ColorHSV());
                voronoiTexture.SetPixel(i, j, colors[siteIndex]);
            }
        }
        voronoiTexture.Apply();

        // Assign Voronoi diagram as texture to material
        material.mainTexture = voronoiTexture;
    }

    int GetNearestSiteIndex(Vector2 point, List<Vector2> sites)
    {
        int nearestIndex = 0;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < sites.Count; i++)
        {
            float distance = Vector2.Distance(point, sites[i]);
            if (distance < nearestDistance)
            {
                nearestIndex = i;
                nearestDistance = distance;
            }
        }

        return nearestIndex;
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

