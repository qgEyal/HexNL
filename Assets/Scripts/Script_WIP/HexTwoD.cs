using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*

This script generates a 2D hexagonal sprite texture by calculating the vertices of a hexagon and setting the color 
of each pixel within the hexagon to a random color. It then creates a game object with a SpriteRenderer component to 
display the texture and applies a random material from an array of materials to each hexagonal sprite in the user-defined grid.

To use this script, create a new C# script in Unity and paste in the code. Attach the script to a game object in your 
scene, and then set the public variables in the inspector to define the size of your grid, the size of each hexagonal 
sprite, and the materials to choose from. When you run the scene, the script will generate the hexagonal sprites and 
apply the materials to each one.

*/
public class HexTwoD : MonoBehaviour
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

                // Create hexagonal sprite texture
                Texture2D hexTexture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
                hexTexture.filterMode = FilterMode.Point;

                float angle_deg = 60f;
                float angle_rad = Mathf.PI / 180f * angle_deg;
                float radius = hexSize / 2f;
                float[] x = new float[6];
                float[] y = new float[6];

                for (int i = 0; i < 6; i++)
                {
                    x[i] = radius * Mathf.Cos(angle_rad * i);
                    y[i] = radius * Mathf.Sin(angle_rad * i);
                }

                for (int yTex = 0; yTex < 128; yTex++)
                {
                    for (int xTex = 0; xTex < 128; xTex++)
                    {
                        if (IsPointInHexagon(xTex, yTex, x, y))
                        {
                            hexTexture.SetPixel(xTex, yTex, Random.ColorHSV());
                        }
                        else
                        {
                            hexTexture.SetPixel(xTex, yTex, Color.clear);
                        }
                    }
                }

                hexTexture.Apply();

                // Create hexagonal sprite game object
                GameObject hexGO = new GameObject("Hexagon");
                hexGO.transform.position = new Vector3(xPos, yPos, 0);

                // Add sprite renderer component and assign hexagonal sprite texture
                SpriteRenderer spriteRenderer = hexGO.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = Sprite.Create(hexTexture, new Rect(0f, 0f, hexTexture.width, hexTexture.height), Vector2.zero, hexSize);

                // Assign random material to hexagonal sprite
                int randIndex = Random.Range(0, materials.Length);
                spriteRenderer.material = materials[randIndex];
            }
        }
    }

    bool IsPointInHexagon(float x, float y, float[] hexX, float[] hexY)
    {
        bool isInside = false;
        for (int i = 0, j = hexX.Length - 1; i < hexX.Length; j = i++)
        {
            if (((hexY[i] <= y && y < hexY[j]) || (hexY[j] <= y && y < hexY[i])) &&
                (x > (hexX[j] - hexX[i]) * (y - hexY[i]) / (hexY[j] - hexY[i]) + hexX[i]))
            {
                isInside = !isInside;
            }
        }
    return isInside;
    }
}
