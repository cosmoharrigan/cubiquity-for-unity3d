using UnityEngine;
using System.Collections;

using Cubiquity;

public class ColoredCubeMazeFromImage : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		// We will load our texture from the supplied maze image. If you wish to supply your own image then please
		// note that in Unity 4 you have to set the 'Read/Write Enabled flag' in the texture import properties.
		// To do this, find the texture in your project view, go to the import settings in the inspector, change
		// the 'Texture Type' to 'Advanced' and then check the 'Read/Write Enabled' checkbox.
		Texture2D mazeImage = Resources.Load("Images/Maze") as Texture2D;
		
		// The size of the volume we will generate
		int width = mazeImage.width;
		int height = 32;
		int depth = mazeImage.height;
		
		ColoredCubesVolumeData data = ColoredCubesVolumeData.CreateEmptyVolumeData(new Region(0, 0, 0, width-1, height-1, depth-1));
		
		// Now we take the TerrainVolumeData we have just created and build a TerrainVolume from it.
		// We also name it and make it a child of the generator to keep things tidy, though this isn't required.
		GameObject terrain = ColoredCubesVolume.CreateGameObject(data);
		terrain.name = "Maze Volume";
		terrain.transform.parent = transform;
		
		// Iterate over every pixel of our mae image
		for(int z = 0; z < depth; z++)
		{
			for(int x = 0; x < width; x++)			
			{
				//For each pixel of the maze image we check it's color to detemine the height of the floor.
				int floorHeight;
				if(mazeImage.GetPixel(x, z).r > 0.5)
				{
					floorHeight = 5;
				}
				else
				{
					floorHeight = 20;
				}
					
				for(int y = height-1; y > 0; y--)
				{
					QuantizedColor red = new QuantizedColor(255, 0, 0, 255);
					if(y < floorHeight)
					{
						data.SetVoxel(x, y, z, red);
					}
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
