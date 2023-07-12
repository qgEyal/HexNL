using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonalMeshGenerator1 : MonoBehaviour
{
    // Public variables to control the width and height of the grid, and the radius of the hexagons.
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float hexRadius = 1f;

    // add sphere prefab
    // public GameObject spherePrefab;

    // Private variables for storing the vertices and triangles of the hexagonal mesh, and the actual mesh.
    private Vector3[] vertices;
    private int[] triangles;
    private Mesh hexagonalMesh;


    private HashSet<Vector3> spherePositions = new HashSet<Vector3>();

    // Start method is called when the script component is enabled.
    private void Start()
    {
        // Call GenerateHexagonalMesh and CreateGridLayout methods to create the hexagonal grid.
        GenerateHexagonalMesh();
        CreateGridLayout();
    }

    // This method generates a hexagonal mesh.
    private void GenerateHexagonalMesh()
    {
        // Create an array of 7 vertices for the hexagonal mesh.
        vertices = new Vector3[7];

        // Generate vertices for the hexagonal mesh by using a loop.
        for (int i = 0; i < 7; i++)
        {
            // Calculate the angle based on the current iteration and convert it to radians.
            float angle = 60 * i + 30;
            vertices[i] = new Vector3(hexRadius * Mathf.Sin(angle * Mathf.Deg2Rad), 0, hexRadius * Mathf.Cos(angle * Mathf.Deg2Rad));
        }

        // Create an array of triangles for the hexagonal mesh.
        triangles = new int[] { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5, 0, 5, 6 };

        // Create a new mesh and assign the vertices and triangles.
        hexagonalMesh = new Mesh();
        hexagonalMesh.vertices = vertices;
        hexagonalMesh.triangles = triangles;

        // Recalculate the normals of the mesh.
        hexagonalMesh.RecalculateNormals(); 
    }

    // This method creates a grid layout from the hexagonal mesh.

    private void CreateGridLayout()
    {
        
        // List<Vector3> uniquePositions = new List<Vector3>();
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

                // Create a new GameObject to represent the hexagonal mesh.
                GameObject hexagonalObject = new GameObject("Hexagonal Mesh " + i + "," + j);
                hexagonalObject.transform.position = position;

                // Add a MeshFilter component to the GameObject and assign the hexagonal mesh.
                MeshFilter meshFilter = hexagonalObject.AddComponent<MeshFilter>();
                //! Assign the mesh
                meshFilter.mesh = hexagonalMesh;
                MeshRenderer meshRenderer = hexagonalObject.AddComponent<MeshRenderer>();
                meshRenderer.material = new Material(Shader.Find("Diffuse"));

                //! need to parent to main object

                // Debug.LogFormat("vertices x{0}  z{1}", position.x, position.z);

               
                //! sphere in center of hex

                // if(!spherePositions.Contains(position))
                // {
                //     spherePositions.Add(position);
                //     GameObject sphere = Instantiate(spherePrefab, position,Quaternion.identity);
                //     sphere.name = "Sphere " + i + "," + j;
                //     sphere.transform.parent = hexagonalObject.transform;
                // }
                

                /*
                // Sphere on every vertex

                // Create spheres on each vertex
                for (int k = 0; k < vertices.Length; k++)
                {
                    Vector3 spherePosition = hexagonalObject.transform.position + vertices[k];
                    if (spherePositions.Contains(spherePosition))
                    {
                        continue;
                    }
                    GameObject sphere = Instantiate(spherePrefab, spherePosition, Quaternion.identity);
                    sphere.transform.parent = hexagonalObject.transform;
                    spherePositions.Add(spherePosition);
                }
                */                
                
            }
        }
       
    }
}


    /*
    The CreateGridLayout method creates a grid of hexagonal meshes using the previously generated mesh. 
    It starts by looping through the rows and columns of the grid and calculating the position of each 
    hexagonal mesh based on the grid indices and hexRadius. 
    A new GameObject is created for each mesh, 
    positioned at the calculated position, and given a mesh filter and renderer component to display the mesh. 
    The material used for the renderer is created using the "Diffuse" shader.
    */


/*

Vector3[] verts = RemoveDuplicates(vertices);


    Vector3[] RemoveDuplicates(Vector3[] dupArray) {

        Vector3[] newArray = new Vector3[vertexNum];  //change 8 to a variable dependent on shape
        bool isDup = false;
        int newArrayIndex = 0;
        for (int i = 0; i < dupArray.Length; i++) {
            for (int j = 0; j < newArray.Length; j++) {
                if (dupArray[i] == newArray[j]) {
                    isDup = true;
                }
            }
            if (!isDup) {
                newArray[newArrayIndex] = dupArray[i];
                newArrayIndex++;
                isDup = false;
            }
        }
        return newArray;
    }   
}

    void DrawSpheres(Vector3[] verts) 
    {
        GameObject[] Spheres = new GameObject[verts.Length];
        for (int i = 0; i < verts.Length; i++) {
            Spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Spheres[i].transform.position = verts[i];
            Spheres[i].transform.localScale -= new Vector3(0.8F, 0.8F, 0.8F);
        }
    }
   
   
}
*/
