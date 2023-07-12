using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

Here's how the script works:

- The `GenerateHexagonMesh` method creates a hexagonal mesh with one central vertex and six surrounding vertices. 
The central vertex has a position of (0, 0, 0), and the surrounding vertices are positioned on a circle with a radius 
of `_hexagonRadius`. The method then assigns the mesh to the `MeshFilter` component of the game object.
- The `SubdivideHexagonMesh` method iteratively subdivides the hexagonal mesh by adding new vertices at the midpoint 
of each triangle edge and creating new triangles using those vertices. The method repeats `_subdivisions` times.
- The `GenerateVoronoiDiagram` method creates a new `Texture2D` and loops through every pixel in the texture. For 
each pixel, the method finds the closest vertex in the hexagonal mesh and assigns a color to the pixel based on 
that vertex. The color is generated using the `GetVertexColor` method, which generates a color based on the index 
of the vertex. The method then assigns the texture to the `Material` of the game object's `MeshRenderer`.


*/
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FourHex : MonoBehaviour
{
    [SerializeField] private int _subdivisions = 1;
    [SerializeField] private float _hexagonRadius = 1f;

    // public int subdivisions = 1;

    private Mesh _hexagonMesh;

    private void Awake()
    {
        GenerateHexagonMesh();
        //SubdivideHexagonMesh();
        // GenerateVoronoiDiagram();
    }

    private void GenerateHexagonMesh()
    {
        _hexagonMesh = new Mesh();

        

        var vertices = new Vector3[7];
        var triangles = new int[18];

        vertices[0] = Vector3.zero;
        for (int i = 1; i < 7; i++)
        {
            float rad = Mathf.PI / 3f * i;
            vertices[i] = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * _hexagonRadius;
        }

        triangles[0] = 1;
        triangles[1] = 2;
        triangles[2] = 0;

        triangles[3] = 2;
        triangles[4] = 3;
        triangles[5] = 0;

        triangles[6] = 3;
        triangles[7] = 4;
        triangles[8] = 0;

        triangles[9] = 4;
        triangles[10] = 5;
        triangles[11] = 0;

        triangles[12] = 5;
        triangles[13] = 6;
        triangles[14] = 0;

        triangles[15] = 6;
        triangles[16] = 1;
        triangles[17] = 0;

        _hexagonMesh.vertices = vertices;
        _hexagonMesh.triangles = triangles;
        _hexagonMesh.RecalculateNormals();
        _hexagonMesh.RecalculateBounds();

        
        // // Subdivide mesh
        // for (int i = 0; i < subdivisions; i++)
        // {
        //     _hexagonMesh = SplitMesh(_hexagonMesh);
        // }

        GetComponent<MeshFilter>().mesh = _hexagonMesh;
    }

   
    private void SubdivideHexagonMesh()
    {
        for (int i = 0; i < _subdivisions; i++)
        {
            var vertices = _hexagonMesh.vertices;
            var triangles = _hexagonMesh.triangles;

            var newVertices = new Vector3[vertices.Length + 6];
            var newTriangles = new int[triangles.Length * 4];

            for (int j = 0; j < vertices.Length; j++)
            {
                newVertices[j] = vertices[j];
            }

            int currentVertexIndex = vertices.Length;

            for (int j = 0; j < triangles.Length; j += 3)
            {
                int v1 = triangles[j];
                int v2 = triangles[j + 1];
                int v3 = triangles[j + 2];

                Vector3 midPoint1 = (vertices[v1] + vertices[v2]) / 2f;
                Vector3 midPoint2 = (vertices[v2] + vertices[v3]) / 2f;
                Vector3 midPoint3 = (vertices[v3] + vertices[v1]) / 2f;

                newVertices[currentVertexIndex++] = midPoint1;
                newVertices[currentVertexIndex++] = midPoint2;
                newVertices[currentVertexIndex++] = midPoint3;

                int m1 = currentVertexIndex - 3;
                int m2 = currentVertexIndex - 2;
                int m3 = currentVertexIndex - 1;

                newTriangles[j * 4] = v1;
                newTriangles[j * 1] = m1;
                newTriangles[j * 4 + 2] = m3;
                newTriangles[j * 4 + 1] = m1;
                newTriangles[j * 4 + 3] = v2;
                newTriangles[j * 4 + 4] = m2;

                newTriangles[j * 4 + 5] = m1;
                newTriangles[j * 4 + 6] = m2;
                newTriangles[j * 4 + 7] = m3;

                newTriangles[j * 4 + 8] = m3;
                newTriangles[j * 4 + 9] = m2;
                newTriangles[j * 4 + 10] = v3;
            }

            _hexagonMesh.vertices = newVertices;
            _hexagonMesh.triangles = newTriangles;
            _hexagonMesh.RecalculateNormals();
            _hexagonMesh.RecalculateBounds();
        }
    }
  

    private void GenerateVoronoiDiagram()
    {
    var texture = new Texture2D(512, 512, TextureFormat.RGBA32, false);
      for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                var color = GetRandomColor();

                float shortestDistance = float.MaxValue;

                var vertices = _hexagonMesh.vertices;
                for (int i = 0; i < vertices.Length; i++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(vertices[i].x, vertices[i].z));
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        color = GetVertexColor(i);
                    }
                }

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        GetComponent<MeshRenderer>().material.mainTexture = texture;
        }

    private Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }

    private Color GetVertexColor(int vertexIndex)
    {
        return new Color(
        Mathf.Sin(vertexIndex * 0.5f) * 0.5f + 0.5f,
        Mathf.Sin(vertexIndex * 0.3f + 1.2345f) * 0.5f + 0.5f,
        Mathf.Sin(vertexIndex * 0.7f + 9.8765f) * 0.5f + 0.5f);
    }
}

