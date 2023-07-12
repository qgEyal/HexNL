using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

Here's how the script works:

1. The `GenerateHexagonalMesh` function creates a hexagonal grid made of individual hexagons. It creates a 
mesh with vertices and triangles for each hexagon and sets it as the mesh for the object.
2. The `GenerateVoronoiDiagram` function generates a random color for each hexagon in the mesh and uses 
these colors to create a 2D texture that represents a Voronoi diagram.
3. The `SubdivideMesh` function checks the color of each hexagon in the mesh and if it has a color, it 
subdivides the hexagon into smaller hexagons to match the Voronoi diagram pattern. It also updates the 
color of each vertex in the subdivided hexagons to match the color of the Voronoi cell.
4. The `ApplyTexture` function applies the Voronoi texture to the mesh.

To use this script, attach it to a GameObject in your scene and set the `gridSize` and `hexRadius` 
variables to the desired values. You can also set the `material` variable to the material you want to
use for the mesh. When you play the scene, the script will generate a hexagonal mesh with a Voronoi diagram 
texture applied.

*/

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexagonalMeshGenerator : MonoBehaviour {

    public int gridSize = 10;
    public float hexRadius = 1f;
    public Material material;
    public Texture2D voronoiTexture;

    private MeshFilter meshFilter;
    private Mesh hexMesh;
    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Color> colors;

    void Start () {
        meshFilter = GetComponent<MeshFilter> ();
        hexMesh = new Mesh ();
        vertices = new List<Vector3> ();
        triangles = new List<int> ();
        colors = new List<Color> ();

        GenerateHexagonalMesh ();
        GenerateVoronoiDiagram ();
        SubdivideMesh ();
        ApplyTexture ();
    }

    void GenerateHexagonalMesh () {
        float x, y;
        for (int i = 0; i < gridSize; i++) {
            for (int j = 0; j < gridSize; j++) {
                x = hexRadius * Mathf.Sqrt (3) * (j + 0.5f * (i % 2));
                y = hexRadius * 1.5f * i;
                AddHexagon (new Vector3 (x, y, 0f), hexRadius);
            }
        }
        
        hexMesh.vertices = vertices.ToArray ();
        hexMesh.triangles = triangles.ToArray ();
        hexMesh.RecalculateNormals ();
        hexMesh.RecalculateBounds ();
        meshFilter.mesh = hexMesh;
        GetComponent<MeshRenderer> ().material = material;
    }

    

    void AddHexagon (Vector3 center, float radius) {
        Vector3[] vertices = new Vector3[6];
        int[] triangles = new int[18];

        for (int i = 0; i < 6; i++) {
            float angle_deg = 60 * i - 30;
            float angle_rad = Mathf.PI / 180f * angle_deg;
            vertices[i] = center + new Vector3 (radius * Mathf.Cos (angle_rad), radius * Mathf.Sin (angle_rad), 0f);
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 2) % 6 + 1;
        }
        int offset = this.vertices.Count;
        this.vertices.AddRange (vertices);
        for (int i = 0; i < triangles.Length; i++) {
            this.triangles.Add (triangles[i] + offset);
        }
    }

    void GenerateVoronoiDiagram () {
        for (int i = 0; i < hexMesh.vertexCount; i++) {
            colors.Add (Random.ColorHSV ());
        }
        voronoiTexture = new Texture2D (gridSize, gridSize);
        voronoiTexture.filterMode = FilterMode.Point;
        voronoiTexture.wrapMode = TextureWrapMode.Clamp;
        for (int i = 0; i < hexMesh.vertexCount; i++) {
            Vector3 vertex = hexMesh.vertices[i];
            Color color = colors[i];
            Vector2Int pixel = new Vector2Int ((int) vertex.x, (int) vertex.y);
            voronoiTexture.SetPixel (pixel.x, pixel.y, color);
        }
        voronoiTexture.Apply ();
    }

    void SubdivideMesh () {
        for (int i = 0; i < hexMesh.vertexCount; i++) {
            Vector3 vertex = hexMesh.vertices[i];
            Vector2Int pixel = new Vector2Int ((int) vertex.x, (int) vertex.y);
            Color color = voronoiTexture.GetPixel (pixel.x, pixel.y);
            if (color != Color.clear) 
                {
                    SubdivideHexagon (i, color);
                }
            }
            hexMesh.vertices = vertices.ToArray ();
            hexMesh.triangles = triangles.ToArray ();
            hexMesh.RecalculateNormals ();
            hexMesh.RecalculateBounds ();
    }
    void SubdivideHexagon (int centerIndex, Color color) {
    Vector3 center = hexMesh.vertices[centerIndex];
    List<Vector3> newVertices = new List<Vector3> ();
    List<int> newTriangles = new List<int> ();

    newVertices.Add (center);
    int indexOffset = vertices.Count;

    for (int i = 0; i < 6; i++) {
        Vector3 vertex = hexMesh.vertices[centerIndex + i + 1];
        Vector3 direction = vertex - center;
        float distance = direction.magnitude;
        direction.Normalize ();
        Vector3 newVertex = center + (direction * hexRadius * 0.5f);
        newVertices.Add (newVertex);
        int triangle1 = i;
        int triangle2 = (i + 1) % 6;
        newTriangles.Add (triangle1);
        newTriangles.Add (indexOffset + i + 1);
        newTriangles.Add (indexOffset + triangle2 + 1);
        newTriangles.Add (indexOffset + i + 1);
        newTriangles.Add (triangle2);
        newTriangles.Add (indexOffset + triangle2 + 1);
    }
    vertices.AddRange (newVertices);
    triangles.AddRange (newTriangles);

    for (int i = centerIndex; i < centerIndex + 7; i++) {
        colors[i] = color;
    }
}

void ApplyTexture () {
    MeshRenderer meshRenderer = GetComponent<MeshRenderer> ();
    meshRenderer.material.mainTexture = voronoiTexture;
}
}
