using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! The VoronoiDiagram script creates and displays a Voronoi diagram based on specified parameters.
//! The script uses the MonoBehaviour class, which allows it to be attached to a Unity game object and called in the game engine.
public class VoronoiDiagram : MonoBehaviour
{
	//! Variables:

    // public GameObject prefab;

    //! imageDimension: The size of the image to be generated.
    public Vector2Int imageDimension;

	 //! regionAmount: The number of regions the image will be divided into.
    public int regionAmount;

     //! drawByDistance: A flag that determines whether the diagram will be generated using the distance between the pixels 
     //! and their closest centroids. Creates a greyscale or solid Voronoi diagram
    public bool drawByDistance = false;
	
    //! The Start() method uses the SpriteRenderer component to create a sprite from either GetDiagram() or GetDiagramByDistance(), 
    //! depending on the value of drawByDistance.
    private void Start()
	{
		GetComponent<SpriteRenderer>().sprite = Sprite.Create((drawByDistance ? GetDiagramByDistance() : GetDiagram()), 
                                                                new Rect(0, 0, imageDimension.x, imageDimension.y), Vector2.one * 0.5f);

        
	}

    //! GetDiagram() generates a Voronoi diagram by randomly selecting a number of centroids and assigning a random color to each. 
	Texture2D GetDiagram()
	{
		//! points to divide area with
        // Create arrays to hold centroids and regions color
        Vector2Int[] centroids = new Vector2Int[regionAmount];
		Color[] regions = new Color[regionAmount];

        // Populate centroids and regions arrays with random values
		for(int i = 0; i < regionAmount; i++)
		{
			centroids[i] = new Vector2Int(Random.Range(0, imageDimension.x), Random.Range(0, imageDimension.y));
			regions[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            
            // Debug.LogFormat("index centroids[i]: {0} , {1} ", centroids[i], regions[i]);
            // Instantiate(prefab,new Vector3(centroids[i][0],centroids[i][1],0), Quaternion.identity);
            // prefab.GetComponent<Material>().color = regions[i];

            // prefab.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_color",regions[i]);

            
           
		}

        // Create a color array to hold pixel colors
		Color[] pixelColors = new Color[imageDimension.x * imageDimension.y];

        // Set each pixel color based on the closest centroid color
		for(int x = 0; x < imageDimension.x; x++)
		{
			for(int y = 0; y < imageDimension.y; y++)
			{
				//! find right position for the color index
                //! The closest centroid to each pixel is found, and the pixel is colored based on the closest centroid's color. 
                int index = x * imageDimension.x + y;
				pixelColors[index] = regions[GetClosestCentroidIndex(new Vector2Int(x, y), centroids)];
                
  
			}
		}
		//! The method returns a Texture2D image created from the pixel colors.
        return GetImageFromColorArray(pixelColors);
	}

    //! GetDiagramByDistance() generates a Voronoi diagram where each pixel's color is based on its distance from its closest centroid. 
    //! The method calculates the distances, finds the maximum distance, and creates a gray-scale image based on each pixel's distance value. 
    //! The method returns a Texture2D image created from the pixel colors.
	Texture2D GetDiagramByDistance()
	{
		//! points to divide area with
        // Create an array to hold centroids
        Vector2Int[] centroids = new Vector2Int[regionAmount];
		
		for (int i = 0; i < regionAmount; i++)
		{
			centroids[i] = new Vector2Int(Random.Range(0, imageDimension.x), Random.Range(0, imageDimension.y));
		}
		Color[] pixelColors = new Color[imageDimension.x * imageDimension.y];
		float[] distances = new float[imageDimension.x * imageDimension.y];

		//you can get the max distance in the same pass as you calculate the distances. :P oops!
		float maxDst = float.MinValue;
		for (int x = 0; x < imageDimension.x; x++)
		{
			for (int y = 0; y < imageDimension.y; y++)
			{
				int index = x * imageDimension.x + y;
				distances[index] = Vector2.Distance(new Vector2Int(x,y), centroids[GetClosestCentroidIndex(new Vector2Int(x,y), centroids)]);
				if(distances[index] > maxDst)
				{
					maxDst = distances[index];
				}
			}	
		}

		for(int i = 0; i < distances.Length; i++)
		{
			float colorValue = distances[i] / maxDst;
            //! assign a greyscale value based on distance
			pixelColors[i] = new Color(colorValue, colorValue, colorValue, 1f);
		}
		return GetImageFromColorArray(pixelColors);
	}
	
    //! GetClosestCentroidIndex() finds the closest centroid to a given pixel position. 
    //! The method returns the INT index of the closest centroid.
	int GetClosestCentroidIndex(Vector2Int pixelPos, Vector2Int[] centroids)
	{
		//! Declares a variable smallestDst of type float 
        //! and sets its initial value to float.MaxValue, which is the largest positive value of type float.
        float smallestDst = float.MaxValue;
		int index = 0;
		for(int i = 0; i < centroids.Length; i++)
		{
			if (Vector2.Distance(pixelPos, centroids[i]) < smallestDst)
			{
				smallestDst = Vector2.Distance(pixelPos, centroids[i]);
				index = i;
			}
		}
		return index;
	}

    //! GetImageFromColorArray() creates a Texture2D image from an array of pixel colors. The method returns the created image.
	Texture2D GetImageFromColorArray(Color[] pixelColors)
	{
		Texture2D tex = new Texture2D(imageDimension.x, imageDimension.y);
		tex.filterMode = FilterMode.Point;
		tex.SetPixels(pixelColors);
		tex.Apply();
		return tex;
	}
}


