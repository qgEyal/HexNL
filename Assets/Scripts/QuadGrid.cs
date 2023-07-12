using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadGrid : MonoBehaviour
{
    public int width;
    public int height;
    public float quadSize;
    
    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        float totalWidth = 0;
        float totalHeight = 0;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                GameObject quad = CreateQuad();
                quad.transform.position = new Vector3(x * quadSize, 0, y * quadSize);
                totalWidth += quadSize;
            }
            totalHeight += quadSize;
        }

        Debug.Log("Total Width: " + totalWidth);
        Debug.Log("Total Height: " + totalHeight);
    }

    GameObject CreateQuad()
    {
        Mesh quadMesh = new Mesh();
        quadMesh.vertices = new Vector3[]
        {
            new Vector3(-0.5f, 0, -0.5f),
            new Vector3(-0.5f, 0, 0.5f),
            new Vector3(0.5f, 0, 0.5f),
            new Vector3(0.5f, 0, -0.5f)
        };
        quadMesh.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0)
        };
        quadMesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        GameObject quadObject = new GameObject("Quad");
        MeshFilter meshFilter = quadObject.AddComponent<MeshFilter>();
        meshFilter.mesh = quadMesh;

        MeshRenderer meshRenderer = quadObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));

        return quadObject;
    }
}

