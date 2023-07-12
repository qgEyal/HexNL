using System.Collections;
using UnityEngine;
using System.Collections.Generic;

/*

Explanation:

The `Start` method creates the hexagon mesh by generating its vertices and triangles. It also adds a `MeshRenderer` component to the 
game object and assigns the mesh to it. Then it generates the Voronoi diagram by creating random point sites, computing the Voronoi 
diagram using the `Voronoi` class (which is not part of Unity and needs to be implemented separately), and assigning random colors to 
the Voronoi cells. Finally, it subdivides the hexagon mesh to match the Voronoi diagram pattern.

The `GenerateVoronoiColors` method takes the subdivision level as input and returns an array of colors, one color per vertex of the 
hex
*/
public class ThreeHex : MonoBehaviour
{
    public int hexagonSize = 1;
    public int subdivisionLevel = 1;

    private Mesh hexagonMesh;
    private MeshRenderer hexagonRenderer;

    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Color> colors;

    void Start()
    {
        // Create the hexagon mesh
        hexagonMesh = new Mesh();
        hexagonMesh.name = "Hexagon Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();

        float x, y;
        for (int i = 0; i < 6; i++)
        {
            x = hexagonSize * Mathf.Cos(Mathf.PI / 3 * i);
            y = hexagonSize * Mathf.Sin(Mathf.PI / 3 * i);
            vertices.Add(new Vector3(x, y, 0));
            colors.Add(Color.white);
        }

        for (int i = 0; i < 6; i++)
        {
            triangles.Add(i);
            triangles.Add((i + 1) % 6);
            triangles.Add(6);
        }

        hexagonMesh.vertices = vertices.ToArray();
        hexagonMesh.triangles = triangles.ToArray();
        hexagonMesh.colors = colors.ToArray();

        hexagonRenderer = gameObject.AddComponent<MeshRenderer>();
        MeshFilter filter = gameObject.AddComponent<MeshFilter>();
        filter.mesh = hexagonMesh;

        /*
        // Generate the Voronoi diagram
        Color[] voronoiColors = GenerateVoronoiColors(subdivisionLevel);
        for (int i = 0; i < hexagonMesh.vertices.Length; i++)
        {
            hexagonMesh.colors[i] = voronoiColors[i];
        }
        */
        // Subdivide the hexagon mesh
        SubdivideHexagonMesh(subdivisionLevel);
    }

    /*
    Color[] GenerateVoronoiColors(int subdivisionLevel)
    {
        // Create the random point sites
        List<Vector2> pointSites = new List<Vector2>();
        for (int i = 0; i < 20; i++)
        {
            pointSites.Add(new Vector2(Random.Range(-hexagonSize, hexagonSize), Random.Range(-hexagonSize, hexagonSize)));
        }

        // Create the Voronoi diagram
        List<Vector2> vertices2D = new List<Vector2>(hexagonMesh.vertices.Length);
        foreach (Vector3 vertex in hexagonMesh.vertices)
        {
            vertices2D.Add(new Vector2(vertex.x, vertex.y));
        }
        Voronoi voronoi = new Voronoi(pointSites, vertices2D);

        // Generate the Voronoi colors
        Color[] voronoiColors = new Color[hexagonMesh.vertices.Length];
        foreach (Edge edge in voronoi.edges)
        {
            if (edge.a == null || edge.b == null) continue;
            int indexA = vertices2D.IndexOf(edge.a);
            int indexB = vertices2D.IndexOf(edge.b);
            if (indexA == -1 || indexB == -1) continue;
            Color color = Random.ColorHSV();
            for (int i = 0; i < subdivisionLevel; i++)
            {
                int newIndex = vertices2D.Count;
                Vector2 newVertex = Vector2.Lerp(edge.a, edge.b, (float)(i + 1) / (subdivisionLevel + 1));
                vertices2D.Add(newVertex);
                voronoiColors[newIndex] = color;
                if (i == 0)
                {
                    voronoiColors[indexA] = color;
                    voronoiColors[indexB] = color;
                }
                else
                {
                    int prevIndex = vertices2D.Count - 2;
                    triangles.Add(prevIndex);
                    triangles.Add(indexB);
                    triangles.Add(newIndex);
                    triangles.Add(prevIndex);
                    triangles.Add(newIndex);
                    triangles.Add(indexA);
                }
            }
        }

        return voronoiColors;
    }
    */
    void SubdivideHexagonMesh(int subdivisionLevel)
    {
        for (int i = 0; i < subdivisionLevel; i++)
        {
            List<Vector3> newVertices = new List<Vector3>();
            List<int> newTriangles = new List<int>();
            List<Color> newColors = new List<Color>();

            for (int j = 0; j < triangles.Count; j += 3)
            {
                int indexA = triangles[j];
                int indexB = triangles[j + 1];
                int indexC = triangles[j + 2];
                Vector3 vertexA = hexagonMesh.vertices[indexA];
                Vector3 vertexB = hexagonMesh.vertices[indexB];
                Vector3 vertexC = hexagonMesh.vertices[indexC];
                Color colorA = hexagonMesh.colors[indexA];
                Color colorB = hexagonMesh.colors[indexB];
                Color colorC = hexagonMesh.colors[indexC];
                Vector3 vertexAB = Vector3.Lerp(vertexA, vertexB, 0.5f);
                Vector3 vertexBC = Vector3.Lerp(vertexB, vertexC, 0.5f);
                Vector3 vertexCA = Vector3.Lerp(vertexC, vertexA, 0.5f);
                Color colorAB = Color.Lerp(colorA, colorB, 0.5f);
                Color colorBC = Color.Lerp(colorB, colorC, 0.5f);
                Color colorCA = Color.Lerp(colorC, colorA, 0.5f);

                int indexAB = newVertices.Count;
                newVertices.Add(vertexAB);
                newColors.Add(colorAB);

                int indexBC = newVertices.Count;
                newVertices.Add(vertexBC);
                newColors.Add(colorBC);

                int indexCA = newVertices.Count;
                newVertices.Add(vertexCA);
                newColors.Add(colorCA);

                newTriangles.Add(indexA);
                newTriangles.Add(indexAB);
                newTriangles.Add(indexCA);

                newTriangles.Add(indexB);
                newTriangles.Add(indexBC);
                newTriangles.Add(indexAB);

                newTriangles.Add(indexC);
                newTriangles.Add(indexCA);
                newTriangles.Add(indexBC);

                newTriangles.Add(indexAB);
                newTriangles.Add(indexBC);
                newTriangles.Add(indexCA);
            }

            hexagonMesh.vertices = newVertices.ToArray();
            hexagonMesh.triangles = newTriangles.ToArray();
            hexagonMesh.colors = newColors.ToArray();
        }
    }
}
