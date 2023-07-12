
using UnityEngine;

public class HexMeshGenerator : MonoBehaviour
{
    public int numColumns = 10;
    public int numRows = 10;
    public float hexRadius = 1.0f;
    public float noiseScale = 0.1f;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    void Start()
    {
        // Create hexagonal mesh
        GenerateHexagonalMesh();

        // Generate voronoi diagram and apply as texture
        Texture2D texture = GenerateVoronoiTexture();
        meshRenderer.material.mainTexture = texture;
    }

    void GenerateHexagonalMesh()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[(numColumns + 1) * (numRows + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[numColumns * numRows * 6];

        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int row = 0; row <= numRows; row++)
        {
            for (int col = 0; col <= numColumns; col++)
            {
                float x = col * hexRadius * 1.5f;
                float y = 0.0f;
                float z = row * hexRadius * Mathf.Sqrt(3.0f);

                if (row % 2 == 1)
                {
                    x += hexRadius * 0.75f;
                }

                vertices[vertexIndex] = new Vector3(x, y, z);
                uv[vertexIndex] = new Vector2(x / (numColumns * hexRadius * 1.5f), z / (numRows * hexRadius * Mathf.Sqrt(3.0f)));

                if (row > 0 && col > 0)
                {
                    int topLeft = (row - 1) * (numColumns + 1) + col - 1;
                    int topRight = (row - 1) * (numColumns + 1) + col;
                    int bottomLeft = row * (numColumns + 1) + col - 1;
                    int bottomRight = row * (numColumns + 1) + col;

                    if (row % 2 == 1)
                    {
                        triangles[triangleIndex++] = topLeft;
                        triangles[triangleIndex++] = topRight;
                        triangles[triangleIndex++] = bottomRight;

                        triangles[triangleIndex++] = topLeft;
                        triangles[triangleIndex++] = bottomRight;
                        triangles[triangleIndex++] = bottomLeft;
                    }
                    else
                    {
                        triangles[triangleIndex++] = topLeft;
                        triangles[triangleIndex++] = topRight;
                        triangles[triangleIndex++] = bottomLeft;

                        triangles[triangleIndex++] = bottomLeft;
                        triangles[triangleIndex++] = topRight;
                        triangles[triangleIndex++] = bottomRight;
                    }
                }

                vertexIndex++;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    Texture2D GenerateVoronoiTexture()
    {
        Texture2D texture = new Texture2D(numColumns * 2, numRows * 2);
        Color[] colors = new Color[texture.width * texture.height];

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                float minDistance = float.MaxValue;
                int closestVertexIndex = -1;

                for (int i = 0; i < meshFilter.mesh.vertices.Length; i++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(meshFilter.mesh.vertices[i].x, meshFilter.mesh.vertices[i].z));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestVertexIndex = i;
                    }
                }

                colors[y * texture.width + x] = GetVoronoiColor(meshFilter.mesh.vertices[closestVertexIndex], noiseScale);
            }
        }

        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }

    Color GetVoronoiColor(Vector3 point, float scale)
    {
        float noiseValue = Mathf.PerlinNoise(point.x * scale, point.z * scale);
        return new Color(noiseValue, noiseValue, noiseValue);
    }

}

/*

The `GenerateHexagonalMesh()` function generates a hexagonal mesh with the specified number of rows and columns, using the radius 
specified by `hexRadius`. The vertices of the hexagons are positioned on a regular grid, with the grid offset for every other row 
to create the hexagonal shape. The mesh is then added to the GameObject as a MeshFilter and MeshRenderer component.

The `GenerateVoronoiTexture()` function generates a 2D texture using a voronoi diagram. For each pixel in the texture, the closest 
vertex of the hexagonal mesh is determined, and the color of the pixel is determined using the `GetVoronoiColor()` function. This 
function returns a color based on a Perlin noise value at the position of the vertex, scaled by `noiseScale`.

The resulting texture is applied to the MeshRenderer component of the GameObject, which will display the hexagonal mesh with the 
voronoi diagram as a texture.

*/


/*
using UnityEngine;
using System.Collections.Generic;

// [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
// [RequireComponent(typeof(SpriteRenderer))]
public class HexMeshGenerator : MonoBehaviour

{
    public int width = 10;
    public int height = 10;
    public float radius = 1f;

    private Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GenerateHexagonalMesh();
        GenerateVoronoiDiagram();
    }

    void GenerateHexagonalMesh()
    {
        Vector3[] vertices = new Vector3[7 * width * height];
        int[] triangles = new int[3 * 6 * width * height];

        int v = 0;
        int t = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xOffset = (y % 2 == 0) ? 0 : 0.5f;
                float xCoord = x + xOffset;

                vertices[v] = new Vector3(xCoord * radius * Mathf.Sqrt(3f), 0, y * radius * 1.5f);
                vertices[v+1] = new Vector3((xCoord + 1) * radius * Mathf.Sqrt(3f), 0, y * radius * 1.5f);
                vertices[v+2] = new Vector3((xCoord + 0.5f) * radius * Mathf.Sqrt(3f), 0, (y + 1) * radius * 1.5f);
                vertices[v+3] = new Vector3((xCoord - 0.5f) * radius * Mathf.Sqrt(3f), 0, (y + 1) * radius * 1.5f);
                vertices[v+4] = new Vector3(xCoord * radius * Mathf.Sqrt(3f), 0, (y + 2) * radius * 1.5f);
                vertices[v+5] = new Vector3((xCoord + 1) * radius * Mathf.Sqrt(3f), 0, (y + 2) * radius * 1.5f);
                vertices[v+6] = new Vector3((xCoord + 0.5f) * radius * Mathf.Sqrt(3f), 0, (y + 1) * radius * 1.5f);

                triangles[t] = v;
                triangles[t+1] = v+1;
                triangles[t+2] = v+2;
                triangles[t+3] = v+2;
                triangles[t+4] = v+3;
                triangles[t+5] = v;

                triangles[t+6] = v+2;
                triangles[t+7] = v+4;
                triangles[t+8] = v+5;
                triangles[t+9] = v+5;
                triangles[t+10] = v+3;
                triangles[t+11] = v+2;

                v += 7;
                t += 12;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    void GenerateVoronoiDiagram()
    {
        Texture2D texture = new Texture2D(width * 7, height * 6, TextureFormat.RGBA32, false);

        for (int y = 0; y < height * 6; y++)
        {
            for (int x = 0; x < width * 7; x++)
            {
                Vector2Int nearestHex = FindNearestHex(x, y);

                Color color = (nearestHex.x % 2 == 0) ? Color.white : Color.gray;
                Vector2 voronoiPoint = new Vector2(x, y);
                float minDistance = float.MaxValue;
                for (int i = 0; i < 7; i++)
                {
                    Vector2 hexPoint = GetHexPoint(nearestHex.x, nearestHex.y, i);
                    float distance = Vector2.Distance(voronoiPoint, hexPoint);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        color = (i % 2 == 0) ? Color.white : Color.gray;
                    }
                }

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

    Vector2Int FindNearestHex(int x, int y)
    {
        float xOffset = (y % 2 == 0) ? 0 : 0.5f;
        float xCoord = x / (radius * Mathf.Sqrt(3f)) - xOffset / radius;
        float yCoord = y / (radius * 1.5f);

        int q = Mathf.RoundToInt(xCoord);
        int r = Mathf.RoundToInt(yCoord - q * 0.5f);
        int s = -q - r;

        return HexToPixel(q, r, s);
    }

    Vector2 GetHexPoint(int q, int r, int i)
    {
        float xOffset = (r % 2 == 0) ? 0 : 0.5f;
        float x = q * radius * Mathf.Sqrt(3f) + xOffset * radius;
        float y = r * radius * 1.5f;

        switch (i)
        {
            case 0: return new Vector2(x + 0.5f * radius * Mathf.Sqrt(3f), y);
            case 1: return new Vector2(x + radius * Mathf.Sqrt(3f), y + 0.5f * radius);
            case 2: return new Vector2(x + radius * Mathf.Sqrt(3f), y + 1.5f * radius);
            case 3: return new Vector2(x + 0.5f * radius * Mathf.Sqrt(3f), y + 2f * radius);
            case 4: return new Vector2(x, y + 1.5f * radius);
            case 5: return new Vector2(x, y + 0.5f * radius);
            case 6: return new Vector2(x + 0.5f * radius * Mathf.Sqrt(3f), y + radius);
            default: return Vector2.zero;
        }
    }

    Vector2Int HexToPixel(int q, int r, int s)
    {
        int x = q;
        int y = r;

        return new Vector2Int(x, y);
    }
}

*/

/*

The `GenerateHexagonalMesh` function creates a hexagonal mesh using the specified `width` and `height` parameters, with each 
hexagon having a radius of `radius`. The mesh is stored in a `Mesh` object and attached to the `MeshFilter` component of the game object.

The `GenerateVoronoiDiagram` function generates a Voronoi diagram for the hexagonal mesh using a simple distance function. 
The function creates a `Texture2D` object with the same dimensions as the hexagonal mesh, and for each pixel in the texture, 
it calculates the nearest hexagonal coordinate and uses the distance from the pixel to the hexagon center to determine the color 
of the pixel. The resulting texture is then used to create a `Sprite` object, which is attached to the `SpriteRenderer` component 
of the game object.


The `GenerateHexagonalMesh` function creates a hexagonal mesh using the formula for hexagonal grids. It sets the `vertices` and `triangles` 
arrays of the `Mesh` object and recalculates the normals.

The `GenerateVoronoiDiagram` function creates a `Texture2D` object with the same dimensions as the hexagonal mesh and sets the color 
of each pixel to the color of the nearest hexagon. It does this by iterating over every pixel in the texture and finding the nearest


*/



/*
{
    public int size = 10;
    public float radius = 1.0f;
    public Material material;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    void Start()
    {
        GenerateMesh();
        ApplyVoronoiDiagram();
    }

    void GenerateMesh()
    {
        // Create vertices and triangles for hexagonal mesh
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = j * radius * Mathf.Sqrt(3);
                float y = i * radius * 3 / 2;
                if (j % 2 == 1)
                {
                    y += radius * Mathf.Sqrt(3) / 2;
                }
                vertices.Add(new Vector3(x, 0, y));

                if (i < size - 1 && j < size - 1)
                {
                    int index = i * size + j;
                    if (j % 2 == 0)
                    {
                        triangles.Add(index);
                        triangles.Add(index + size + 1);
                        triangles.Add(index + size);
                        triangles.Add(index);
                        triangles.Add(index + 1);
                        triangles.Add(index + size + 1);
                    }
                    else
                    {
                        triangles.Add(index);
                        triangles.Add(index + size);
                        triangles.Add(index + 1);
                        triangles.Add(index + 1);
                        triangles.Add(index + size);
                        triangles.Add(index + size + 1);
                    }
                }
            }
        }

        // Create UVs for hexagonal mesh
        for (int i = 0; i < vertices.Count; i++)
        {
            uvs.Add(new Vector2(vertices[i].x, vertices[i].z));
        }

        // Create mesh object and apply material
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        GameObject hexMesh = new GameObject("HexMesh");
        hexMesh.AddComponent<MeshFilter>().mesh = mesh;
        hexMesh.AddComponent<MeshRenderer>().material = material;
        hexMesh.transform.position = Vector3.zero;
    }

    void ApplyVoronoiDiagram()
    {
        // Create Voronoi diagram points
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < size * size; i++)
        {
            float x = vertices[i].x;
            float z = vertices[i].z;
            points.Add(new Vector2(x, z));
        }

        // Create Voronoi diagram texture
        Texture2D voronoiTex = new Texture2D(512, 512);
        //! Voronoi voronoi = new Voronoi(points, new Rect(0, 0, 512, 512));
        for (int x = 0; x < 512; x++)
        {
            for (int y = 0; y < 512; y++)
            {
                //! List<Vector2> cell = voronoi.Cell(new Vector2(x, y));
                if (cell.Count == 0)
                {
                    voronoiTex.SetPixel(x, y, Color.white);
                }
                else
                {
                    Color color = Color.HSVToRGB(Random.value, 1.0f, 1.0f);
                voronoiTex.SetPixel(x, y, color);
                }
            }
        }
        voronoiTex.Apply();

        // Apply Voronoi diagram texture to hex mesh as sprite
        Sprite sprite = Sprite.Create(voronoiTex, new Rect(0, 0, 512, 512), Vector2.zero);
        GameObject hexSprite = new GameObject("HexSprite");
        hexSprite.AddComponent<SpriteRenderer>().sprite = sprite;
        hexSprite.transform.position = Vector3.zero;
    }
}
*/
/*

This script creates a hexagonal mesh with the specified size and radius, and then generates a Voronoi diagram using 
the mesh vertices as input points. It then applies the Voronoi diagram as a texture to the hexagonal mesh, which is 
displayed as a sprite. The script assumes that you have a Voronoi class defined elsewhere in your project, which can 
be used to generate the diagram.

*/