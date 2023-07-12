using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

//! Look at HexNL for base script
public class HexMaker : MonoBehaviour
{
     //! Variables
     [Header("Hex Grid")]
    [SerializeField]
    private float hexRadius = 1f;

    [SerializeField]
    private int gridWidth = 3;
    [SerializeField]
    private int gridHeight = 3;
    [Header("Voronoi Diagram")]
    [SerializeField]
    private int voronoiRegions = 5;

    [SerializeField]
    private int regionMinBias, regionMaxBias;

   


    public Vector2Int textureDimensions;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh hexMesh;

    private List<GameObject> hexagons = new List<GameObject>();

    void Awake()
    {
        GenerateHexMesh();        
        CreateGridLayout();    
        Bounds bounds = GetBounds(gameObject);

        // Calculate the width and height of the bounds in Unity distance units
        float width = bounds.size.x;
        float height = bounds.size.z;

        Debug.Log("Width of hexagonal grid: " + width + " Unity distance units");
        Debug.Log("Height of hexagonal grid: " + height + " Unity distance units");

        //! Center Hex gameObject on grid
        gameObject.transform.position = new Vector3(-(width/2) + hexRadius, 0.0f, -(height/2) + (hexRadius*Mathf.Sqrt(3)/2));
        
    }

    private void GenerateHexMesh()
    {
        // Create a new mesh object
        
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
                
                GameObject hexagon = new GameObject();
                hexagon.name = "Hexagon (" + i + "," + j + ")";
                //! parent to main GameObject
                hexagon.transform.parent = transform;

                hexagon.transform.position = position;

                hexagon.AddComponent<MeshFilter>();
                hexagon.AddComponent<MeshRenderer>();
                
                // add to List
                hexagons.Add(hexagon); 
            }
        }

        // Debug.Log("Number of hexagons " + hexagons.Count);
        for (int k = 0; k < hexagons.Count; k++)
            {
                hexagons[k].GetComponent<MeshFilter>().mesh = hexMesh;
                // Generate Voronoi texture to each individual Hexagon
                GenerateVoronoiTexture(hexagons[k].GetComponent<MeshRenderer>());
            }
    }


    public int RandomizeInRange(int mainValue, int minRange, int maxRange)
    {
        // error check: if minRange is greater than mainValue, set to 2
        int minNumber = Mathf.Max(2, mainValue - minRange);
        int maxNumber = mainValue + maxRange;

        int randomValue = Random.Range(minNumber, maxNumber + 1);
        return randomValue;
    }

    private void GenerateVoronoiTexture(MeshRenderer renderer)
    {
        
        // Create a new texture object
        Texture2D voronoiTexture = new Texture2D(textureDimensions.x,textureDimensions.y);
        voronoiTexture.filterMode = FilterMode.Bilinear;

        // Create a list of randomly placed points
        List<Vector2> points = new List<Vector2>();

        //! create randomization of Voronoi Regions
        int newVoronoiRegions = RandomizeInRange(voronoiRegions,regionMinBias, regionMaxBias);
        // Debug.Log("new voronoi region " + newVoronoiRegions);

        // for (int i = 0; i < voronoiRegions; i++)
        for (int i = 0; i < newVoronoiRegions; i++)
        {
            //! set range based on textureDimensions
            points.Add(new Vector2(Random.Range(0, textureDimensions.x), Random.Range(0, textureDimensions.y)));
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

                Color color = Color.HSVToRGB(nearestPointIndex / (float)newVoronoiRegions, 1, 1);
                // Color color = Color.HSVToRGB(nearestPointIndex / (float)voronoiRegions, 1, 1);
                voronoiTexture.SetPixel(i, j, color);
            }
        }

        voronoiTexture.Apply();
        
        renderer.material.mainTexture = voronoiTexture;

    }

     // Returns the bounds of a GameObject and its children (if any)
    Bounds GetBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds;
        }

        return new Bounds();
    }

}
