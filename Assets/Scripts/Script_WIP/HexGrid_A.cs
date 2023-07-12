using UnityEngine;

public class HexGrid_A : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float hexSize = 1.0f;
    public Material material;

    private Mesh hexMesh;

    void Start()
    {
        GenerateHexGrid();
    }

    void GenerateHexGrid()
    {
        hexMesh = new Mesh();
        hexMesh.name = "Hex Mesh";

        Vector3[] vertices = new Vector3[7];
        Vector2[] uv = new Vector2[7];
        int[] triangles = new int[18];

        vertices[0] = Vector3.zero;
        uv[0] = new Vector2(0.5f, 0.5f);

        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i;
            float angle_rad = Mathf.PI / 180 * angle_deg;
            vertices[i + 1] = new Vector3(hexSize * Mathf.Cos(angle_rad), 0, hexSize * Mathf.Sin(angle_rad));
            uv[i + 1] = new Vector2((vertices[i + 1].x / hexSize + 1) / 2, (vertices[i + 1].z / hexSize + 1) / 2);
        }

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;
        triangles[6] = 0;
        triangles[7] = 3;
        triangles[8] = 4;
        triangles[9] = 0;
        triangles[10] = 4;
        triangles[11] = 5;
        triangles[12] = 0;
        triangles[13] = 5;
        triangles[14] = 6;
        triangles[15] = 0;
        triangles[16] = 6;
        triangles[17] = 1;

        hexMesh.vertices = vertices;
        hexMesh.uv = uv;
        hexMesh.triangles = triangles;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject hex = new GameObject();
                hex.name = "Hexagon";
                hex.transform.position = new Vector3(x * 1.5f * hexSize, 0, y * 2 * hexSize);
                hex.AddComponent<MeshFilter>().mesh = hexMesh;
                hex.AddComponent<MeshRenderer>().material = material;
            }
        }
    }
}
