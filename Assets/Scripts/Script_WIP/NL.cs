using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class NL : MonoBehaviour
{
     //! Variables
    [SerializeField]
    private float hexRadius = 1f;

    [SerializeField]
    private int voronoiRegions = 5;
    [SerializeField]
    private int gridWidth = 3;
    [SerializeField]
    private int gridHeight = 3;


    public Vector2Int textureDimensions;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh hexMesh;

    private List<GameObject> hexagons = new List<GameObject>();

    void Awake()
    {
        // meshFilter = GetComponent<MeshFilter>();
        // meshRenderer = GetComponent<MeshRenderer>();
  
        GenerateHexMesh();
        
        CreateGridLayout();
        // GenerateVorTexture();
    }

    private void GenerateHexMesh()
    {
        // Create a new mesh object
        // hexMesh = new Mesh();

        // Declare arrays for vertices, triangles and UVs
        var vertices = new Vector3[7];       
        var triangles = new int[18];
        Vector2[] uvs = new Vector2[7];
        
        //! initialize UVs. Set the center vertex and its UV coordinates
        uvs[0] = new Vector2(0.5f, 0.5f);
        vertices[0] = Vector3.zero;

        // Loop through the other 6 vertices, setting their position and UVs
        for (int i = 1; i < 7; i++)
        {
            float rad = Mathf.PI / 3f * i;
            
            vertices[i] = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * hexRadius;
            //! The magic UV sauce
            uvs[i] = new Vector2((vertices[i].x / hexRadius +1) /2, (vertices[i].z / hexRadius+1) /2);
        }

        // Create an array of triangles for the hexagonal mesh.
        // arrange trianges counterclockwise so they face upwards
        triangles = new int[] {0,2,1,0,3,2,0,4,3,0,5,4,0,6,5,0,1,6};

        // Assign the arrays to the mesh and recalculate its normals and bounds
        // Create a new mesh object
        hexMesh = new Mesh();
        hexMesh.vertices = vertices;
        hexMesh.uv = uvs;
        hexMesh.triangles = triangles;
        hexMesh.RecalculateNormals();
        hexMesh.RecalculateBounds();

        // GetComponent<MeshFilter>().mesh = hexMesh;
    }

    private void CreateGridLayout()
    {

        // Use two nested loops to create a grid of hexagons.
        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                // Calculate the position of the hexagonal object based on its row and column.
                Vector3 position = new Vector3(j * hexRadius * 1.5f, 0, i * hexRadius * Mathf.Sqrt(3));
                if (j % 2 == 1)
                {
                    position.z += hexRadius * Mathf.Sqrt(3) / 2;
                }
                /*
                // Create a new GameObject to represent the hexagonal mesh.
                GameObject hexagon = new GameObject("Hexagonal Mesh " + i + "," + j);
                hexagon.transform.position = position;

                // Add a MeshFilter component to the GameObject and assign the hexagonal mesh.
                MeshFilter meshFilter = hexagon.AddComponent<MeshFilter>();
                //! Assign the mesh
                meshFilter.mesh = hexMesh;
                MeshRenderer meshRenderer = hexagon.AddComponent<MeshRenderer>();
                // meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                */

                GameObject hexagon = new GameObject();
                hexagon.name = "Hexagon (" + i + "," + j + ")";
                hexagon.transform.parent = transform;
                hexagon.transform.position = position;
                hexagon.AddComponent<MeshFilter>();
                hexagon.AddComponent<MeshRenderer>();
                // hexagon.GetComponent<MeshRenderer>().material = material;



                hexagons.Add(hexagon); 
            }
        }

        Debug.Log("Number of hexagons " + hexagons.Count);
        for (int k = 0; k < hexagons.Count; k++)
            {
                hexagons[k].GetComponent<MeshFilter>().mesh = hexMesh;
                GenerateVoronoiTexture(hexagons[k].GetComponent<MeshRenderer>());
            }
    }

    private void GenerateVoronoiTexture(MeshRenderer renderer)
    {
        
        // Create a new texture object
        Texture2D voronoiTexture = new Texture2D(textureDimensions.x,textureDimensions.y);
        voronoiTexture.filterMode = FilterMode.Bilinear;

        // Create a list of randomly placed points
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
                    float distance = Vector2.Distance(points[k], new Vector2(i,j));

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
        // meshRenderer.material.mainTexture = voronoiTexture;
        renderer.material.mainTexture = voronoiTexture;

    }
    

}
