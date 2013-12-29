using UnityEngine;
using System.Collections;
using UnityEditor;

using System.IO;

namespace Cubiquity
{
	public class MainMenuEntries : MonoBehaviour
	{		
		[MenuItem ("GameObject/Create Other/Terrain Volume")]
		static void CreateTerrainVolume()
		{
			int width = 128;
			int height = 32;
			int depth = 128;
			
			TerrainVolumeData data = TerrainVolumeData.CreateEmptyVolumeData(new Region(0, 0, 0, width-1, height-1, depth-1), VolumeData.Paths.StreamingAssets, VolumeData.GeneratePathToVoxelDatabase());
			
			// Create some ground in the terrain so it shows up in the editor.
			// Soil as a base (mat 1) and then a couple of layers of grass (mat 2).
			TerrainVolumeGenerator.GenerateFloor(data, 6, (uint)1, 8, (uint)2);
			
			// Now create the terrain game object from the data.
			GameObject terrain = TerrainVolume.CreateGameObject(data);
			
			// And select it, so the user can get straight on with editing.
			Selection.activeGameObject = terrain;
			
			// Set up our textures in the appropriate material slots.
			/*terrain.GetComponent<TerrainVolume>().materials[0].diffuseMap = Resources.Load("Textures/Rock") as Texture2D;
			terrain.GetComponent<TerrainVolume>().materials[0].scale = new Vector3(16.0f, 16.0f, 16.0f);		
			terrain.GetComponent<TerrainVolume>().materials[1].diffuseMap = Resources.Load("Textures/Soil") as Texture2D;		
			terrain.GetComponent<TerrainVolume>().materials[2].diffuseMap = Resources.Load("Textures/Grass") as Texture2D;
			terrain.GetComponent<TerrainVolume>().materials[2].scale = new Vector3(1.0f, 1.0f, 1.0f);*/
			
			Shader shader = Shader.Find("TerrainVolume");
			Material material = new Material(shader);
			material.name = "Test String";
			
			material.SetTexture("_Tex0", Resources.Load("Textures/Rock") as Texture2D);
			//material.SetVector("_TexInvScale0", new Vector3(1.0f/16.0f, 1.0f/16.0f, 1.0f/16.0f));
			//material.SetVector("_TexOffset0", new Vector3(0.0f, 0.0f, 0.0f));
			
			material.SetTexture("_Tex1", Resources.Load("Textures/Soil") as Texture2D);
			//material.SetVector("_TexInvScale1", new Vector3(1.0f/16.0f, 1.0f/16.0f, 1.0f/16.0f));
			//material.SetVector("_TexOffset1", new Vector3(0.0f, 0.0f, 0.0f));
			
			material.SetTexture("_Tex2", Resources.Load("Textures/Grass") as Texture2D);
			material.SetTextureScale("_Tex2", new Vector2(0.1f, 0.1f));
			//material.SetVector("_TexInvScale2", new Vector3(1.0f/16.0f, 1.0f/16.0f, 1.0f/16.0f));
			//material.SetVector("_TexOffset2", new Vector3(0.0f, 0.0f, 0.0f));
			
			terrain.AddComponent<TerrainVolumeRenderer>();
			terrain.GetComponent<TerrainVolumeRenderer>().material = material;
			
			// It's possible the textures won't actually be found, as they are just examples and the
			// user might have decided not to include them when importing Cubiquity. This doesn't
			// matter and just means the uer will have to set up their own textures.
			/*if((terrain.GetComponent<TerrainVolume>().materials[0].diffuseMap == null) ||
				(terrain.GetComponent<TerrainVolume>().materials[1].diffuseMap == null) ||
				(terrain.GetComponent<TerrainVolume>().materials[2].diffuseMap == null))
			{
				Debug.LogWarning("Failed to set up the default Cubiquity terrain textures. This is probably " +
					"because you chose not to import the examples when importing Cubiquity? It doesn't matter, " +
					"it just means you have to configure your own textures through the inspector.");
			}*/
		}
		
		[MenuItem ("GameObject/Create Other/Colored Cubes Volume")]
		static void CreateColoredCubesVolume()
		{
			int width = 256;
			int height = 32;
			int depth = 256;
			
			ColoredCubesVolumeData data = ColoredCubesVolumeData.CreateEmptyVolumeData(new Region(0, 0, 0, width-1, height-1, depth-1), VolumeData.Paths.StreamingAssets, VolumeData.GeneratePathToVoxelDatabase());
			
			GameObject coloredCubesGameObject = ColoredCubesVolume.CreateGameObject(data);
			
			// And select it, so the user can get straight on with editing.
			Selection.activeGameObject = coloredCubesGameObject;
			
			// Call Initialize so we can start drawing into the volume right away.
			
			int floorThickness = 8;
			QuantizedColor floorColor = new QuantizedColor(192, 192, 192, 255);
			
			for(int z = 0; z <= depth-1; z++)
			{
				for(int y = 0; y < floorThickness; y++)
				{
					for(int x = 0; x <= width-1; x++)
					{
						data.SetVoxel(x, y, z, floorColor);
					}
				}
			}
		}
	}
}
