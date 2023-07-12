using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

Here's how to use the script:
1. Create a new GameObject in the scene.
2. Add a MeshFilter and a MeshRenderer component to the GameObject.
3. Attach the CustomQuadGenerator script to the GameObject.
4. Set the widthSubdivisions, depthSubdivisions, voronoiRegions, quadWidth, and quadDepth variables in the Inspector as desired.
5. Press Play to generate the quad mesh and the Voronoi diagram texture.

The script generates a quad mesh with the specified number of width and depth subdivisions and UVs. It also generates a Voronoi diagram texture with the specified number of distinct areas using FilterMode.Point and applies it to the quad. The Voronoi diagram texture is generated using a list of randomly generated points and assigning each pixel in the texture to the nearest point using the Euclidean distance metric. The color of each pixel in the texture is determined by the index of the nearest point divided by the total number of points, resulting in a gradient from one color to the next across the Voronoi regions.
*/
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class QuadGen_A : MonoBehaviour
{
    public int widthSubdivisions = 1;
    public int depthSubdivisions = 1;
    public int voronoiRegions = 5;
    public float quadWidth = 1.0f;
    public float quadDepth = 1.0f;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh mesh;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        mesh = new Mesh();

        GenerateQuad();
        GenerateVoronoiTexture();
    }

    void GenerateQuad()
    {
        int verticesCount = (widthSubdivisions + 1) * (depthSubdivisions + 1);
        int trianglesCount = widthSubdivisions * depthSubdivisions * 2;

        Vector3[] vertices = new Vector3[verticesCount];
        Vector2[] uvs = new Vector2[verticesCount];
        int[] triangles = new int[trianglesCount * 3];

        

        float widthStep = quadWidth / widthSubdivisions;
        float depthStep = quadDepth / depthSubdivisions;

        int vertexIndex = 0;

        Debug.LogFormat("vertex coung {0}, uvs {1} {2}", verticesCount, uvs[0], uvs[1]);

        for (int i = 0; i <= depthSubdivisions; i++)
        {
            for (int j = 0; j <= widthSubdivisions; j++)
            {
                vertices[vertexIndex] = new Vector3(j * widthStep - quadWidth * 0.5f, 0, i * depthStep - quadDepth * 0.5f);
                uvs[vertexIndex] = new Vector2((float)j / widthSubdivisions, (float)i / depthSubdivisions);
                vertexIndex++;
            }
        }

        int triangleIndex = 0;

        for (int i = 0; i < depthSubdivisions; i++)
        {
            for (int j = 0; j < widthSubdivisions; j++)
            {
                int topLeft = i * (widthSubdivisions + 1) + j;
                int topRight = topLeft + 1;
                int bottomLeft = topLeft + widthSubdivisions + 1;
                int bottomRight = bottomLeft + 1;

                triangles[triangleIndex] = topLeft;
                triangles[triangleIndex + 1] = bottomLeft;
                triangles[triangleIndex + 2] = topRight;
                triangles[triangleIndex + 3] = topRight;
                triangles[triangleIndex + 4] = bottomLeft;
                triangles[triangleIndex + 5] = bottomRight;

                triangleIndex += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }

    void GenerateVoronoiTexture()
    {
        Texture2D voronoiTexture = new Texture2D(512, 512);
        voronoiTexture.filterMode = FilterMode.Point;

        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < voronoiRegions; i++)
        {
            points.Add(new Vector2(Random.Range(0, 512), Random.Range(0, 512)));
    }

    for (int i = 0; i < voronoiTexture.width; i++)
    {
        for (int j = 0; j < voronoiTexture.height; j++)
        {
            int nearestPointIndex = 0;
            float nearestDistance = float.MaxValue;

            for (int k = 0; k < points.Count; k++)
            {
                float distance = Vector2.Distance(points[k], new Vector2(i, j));

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPointIndex = k;
                }
            }

            Color color = Color.HSVToRGB(nearestPointIndex / (float)voronoiRegions, 1, 1);
            voronoiTexture.SetPixel(i, j, color);
        }
    }

    voronoiTexture.Apply();

    // meshRenderer.material = new Material(Shader.Find("Standard"));  <--- breaks the images
    meshRenderer.material.mainTexture = voronoiTexture;
}






}
