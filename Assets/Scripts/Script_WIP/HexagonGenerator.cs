using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonGenerator : MonoBehaviour
{
    public int rows = 10; // Number of rows in the grid
    public int cols = 10; // Number of columns in the grid
    public float hexSize = 1f; // Size of each hexagonal sprite
    public float xOffset = 1.732f; // Horizontal distance between hexagons
    public float yOffset = 1.5f; // Vertical distance between hexagons
    public Material[] materials; // Array of materials to choose from

    void Start()
    {
        GenerateHexGrid();
    }

    void GenerateHexGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // Calculate position of hexagonal sprite in grid
                float xPos = col * xOffset;
                float yPos = row * yOffset;

                // Offset every other row of hexagonal sprites
                if (row % 2 != 0)
                {
                    xPos += xOffset / 2f;
                }

                // Create hexagonal sprite mesh
                Mesh hexMesh = new Mesh();
                Vector3[] vertices = new Vector3[7];
                int[] triangles = new int[18];
                Vector2[] uv = new Vector2[7];

                float angle_deg = 60f;
                float angle_rad = Mathf.PI / 180f * angle_deg;
                float radius = hexSize;
                float[] x = new float[6];
                float[] y = new float[6];

                for (int i = 0; i < 6; i++)
                {
                    x[i] = radius * Mathf.Cos(angle_rad * i);
                    y[i] = radius * Mathf.Sin(angle_rad * i);
                }

                vertices[0] = Vector3.zero;
                uv[0] = new Vector2(0.5f, 0.5f);

                for (int i = 1; i < 7; i++)
                {
                    vertices[i] = new Vector3(x[i - 1], y[i - 1], 0);
                    uv[i] = new Vector2((x[i - 1] / radius + 1) / 2, (y[i - 1] / radius + 1) / 2);
                }

                triangles[0] = 0;
                triangles[1] = 2;
                triangles[2] = 1;
                triangles[3] = 0;
                triangles[4] = 3;
                triangles[5] = 2;
                triangles[6] = 0;
                triangles[7] = 4;
                triangles[8] = 3;
                triangles[9] = 0;
                triangles[10] = 5;
                triangles[11] = 4;
                triangles[12] = 0;
                triangles[13] = 6;
                triangles[14] = 5;
                triangles[15] = 0;
                triangles[16] = 1;
                triangles[17] = 6;

                hexMesh.vertices = vertices;
                hexMesh.triangles = triangles;
                hexMesh.uv = uv;

                // Create hexagonal sprite game object
                GameObject hexGO = new GameObject("Hexagon");
                hexGO.transform.position = new Vector3(xPos, yPos, 0);

                // Add mesh filter and renderer components
                MeshFilter meshFilter = hexGO.AddComponent<MeshFilter>();
                meshFilter.mesh = hexMesh;
                MeshRenderer meshRenderer = hexGO.AddComponent<MeshRenderer>();
                
                // Assign random material to hexagonal sprite
                int randIndex = Random.Range(0, materials.Length);
                meshRenderer.material = materials[randIndex];
            }
        }
    }
}
