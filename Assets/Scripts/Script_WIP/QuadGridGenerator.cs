using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadGridGenerator : MonoBehaviour
{
    public int width = 5; // number of quads to generate in the x-axis
    public int height = 5; // number of quads to generate in the y-axis
    public float quadSize = 1.0f; // size of each quad in Unity distance units

    void Start()
    {
        float totalWidth = width * quadSize; // calculate total width
        float totalHeight = height * quadSize; // calculate total height

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // create a new quad game object
                GameObject quad = new GameObject("Quad_" + x + "_" + y);
                quad.transform.parent = transform;

                // add mesh filter and renderer components
                MeshFilter meshFilter = quad.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = quad.AddComponent<MeshRenderer>();

                // create the quad mesh
                Mesh mesh = new Mesh();
                mesh.vertices = new Vector3[] {
                    new Vector3(-0.5f, 0.0f, -0.5f) * quadSize,
                    new Vector3(0.5f,  0.0f,-0.5f) * quadSize,
                    new Vector3(0.5f,  0.0f,0.5f) * quadSize,
                    new Vector3(-0.5f,  0.0f, 0.5f) * quadSize
                };
                mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                mesh.uv = new Vector2[] {
                    new Vector2(0.0f, 0.0f),
                    new Vector2(1.0f, 0.0f),
                    new Vector2(1.0f, 1.0f),
                    new Vector2(0.0f, 1.0f)
                };
                mesh.RecalculateNormals();

                // set the quad mesh
                meshFilter.mesh = mesh;

                // set the quad position based on its x and y index in the grid
                quad.transform.position = new Vector3(x * quadSize, 0.0f,y * quadSize);
            }
        }

        Debug.Log("Total width of grid: " + totalWidth + " Unity distance units");
        Debug.Log("Total height of grid: " + totalHeight + " Unity distance units");
    }
}
