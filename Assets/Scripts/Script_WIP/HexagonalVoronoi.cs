using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonalVoronoi : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float hexSize = 1f;

    public Material material;

    private List<GameObject> hexagons = new List<GameObject>();

    private void Start()
    {
        GenerateHexagonalMesh();
    }

    private void GenerateHexagonalMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 center = HexagonalCoordinates(x, y);
                for (int i = 0; i < 6; i++)
                {
                    Vector3 corner1 = HexagonalCorner(center, i);
                    Vector3 corner2 = HexagonalCorner(center, i + 1);
                    AddTriangle(vertices, triangles, corner1, corner2, center);
                    AddUV(uv);
                }

                GameObject hexagon = new GameObject();
                hexagon.name = "Hexagon (" + x + "," + y + ")";
                hexagon.transform.parent = transform;
                hexagon.transform.localPosition = center;
                hexagon.AddComponent<MeshFilter>();
                hexagon.AddComponent<MeshRenderer>();
                hexagon.GetComponent<MeshRenderer>().material = material;

                hexagons.Add(hexagon);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();

        for (int i = 0; i < hexagons.Count; i++)
        {
            hexagons[i].GetComponent<MeshFilter>().sharedMesh = mesh;
            GenerateVoronoiTexture(hexagons[i].GetComponent<MeshRenderer>());
        }
    }

    private void AddTriangle(List<Vector3> vertices, List<int> triangles, Vector3 corner1, Vector3 corner2, Vector3 center)
    {
        int index1 = vertices.Count;
        int index2 = vertices.Count + 1;
        int index3 = vertices.Count + 2;

        vertices.Add(corner1);
        vertices.Add(corner2);
        vertices.Add(center);

        triangles.Add(index1);
        triangles.Add(index2);
        triangles.Add(index3);
    }

    private void AddUV(List<Vector2> uv)
    {
        uv.Add(new Vector2(0f, 0f));
        uv.Add(new Vector2(1f, 0f));
        uv.Add(new Vector2(0.5f, 1f));
    }

    private Vector3 HexagonalCoordinates(int x, int y)
    {
        float xOffset = y % 2 == 0 ? 0f : hexSize * Mathf.Sqrt(3f) / 2f;
        float xCoord = x * hexSize * 1.5f;
        float yCoord = y * hexSize * Mathf.Sqrt(3f) / 2f;
        return new Vector3(xCoord + xOffset, 0f, yCoord);
    }

    private Vector3 HexagonalCorner(Vector3 center, int cornerIndex)
    {
        float angle = 60f * cornerIndex;
        angle = Mathf.PI / 180f * angle;
        float x = center.x + hexSize * Mathf.Cos(angle);
        float z = center.z + hexSize*Mathf.Sin(angle);
    return new Vector3(x, 0f, z);
    }

    private void GenerateVoronoiTexture(MeshRenderer renderer)
    {
        int textureSize = 64;

        Texture2D texture = new Texture2D(textureSize, textureSize);
        Color[] colors = new Color[textureSize * textureSize];

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                Vector2 point = new Vector2(x + Random.Range(-0.4f, 0.4f), y + Random.Range(-0.4f, 0.4f));
                float distance = Mathf.Infinity;
                Color color = Color.white;

                for (int i = 0; i < hexagons.Count; i++)
                {
                    Vector2 hexagonCenter = new Vector2(hexagons[i].transform.localPosition.x, hexagons[i].transform.localPosition.z);
                    float d = Vector2.Distance(point, hexagonCenter);

                    if (d < distance)
                    {
                        distance = d;
                        color = hexagons[i].GetComponent<MeshRenderer>().material.color;
                    }
                }

                colors[y * textureSize + x] = color;
            }
        }

        texture.SetPixels(colors);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.Apply();

        renderer.material.mainTexture = texture;
    }
}

/*

This script creates a hexagonal mesh made up of individual hexagons and applies a unique Voronoi diagram texture to each hexagon. 
The Voronoi diagram texture is generated procedurally within the script, with each hexagon using its own color for the diagram. 
The script also allows you to customize the width, height, and size of the hexagons, as well as the material used for the mesh.

*/

