using UnityEngine;
using System.Collections;
using UnityEditor;

using System.IO;

namespace Cubiquity
{
	public class MainMenuEntries : MonoBehaviour
	{
		static string RandomString()
		{
			int randomVal = Random.Range(0, 1000000000);
			return randomVal.ToString();
		}
		
		[MenuItem ("GameObject/Create Other/Terrain Volume")]
		static void CreateTerrainVolume()
		{
			int width = 128;
			int height = 32;
			int depth = 128;
			
			//TerrainVolumeData data = new TerrainVolumeData(new Region(0, 0, 0, width-1, height-1, depth-1), path);
			
			TerrainVolumeData data = ScriptableObject.CreateInstance<TerrainVolumeData>();
			data.Init(new Region(0, 0, 0, width-1, height-1, depth-1));
			
			TerrainVolume.CreateGameObject(data);
			
			// Create some ground in the terrain so it shows up in the editor.
			// Soil as a base (mat 0) and then a couple of layers of grass (mat 1).
			TerrainVolumeGenerator.GenerateFloor(data, 6, (uint)0, 8, (uint)1);
		}
		
		[MenuItem ("GameObject/Create Other/Colored Cubes Volume")]
		static void CreateColoredCubesVolume()
		{
			int width = 256;
			int height = 64;
			int depth = 256;
			
			string path = Application.streamingAssetsPath + Path.DirectorySeparatorChar + RandomString() + ".vol";
			
			GameObject voxelGameObject = ColoredCubesVolumeFactory.CreateVolume("Voxel Terrain", new Region(0, 0, 0, width-1, height-1, depth-1), path);
			ColoredCubesVolume coloredCubesVolume = voxelGameObject.GetComponent<ColoredCubesVolume>();
			
			// Call Initialize so we can start drawing into the volume right away.
			
			int floorThickness = 8;
			Color32 floorColor = new Color32(192, 192, 192, 255);
			
			for(int z = 0; z <= depth-1; z++)
			{
				for(int y = 0; y < floorThickness; y++)
				{
					for(int x = 0; x <= width-1; x++)
					{
						coloredCubesVolume.SetVoxel(x, y, z, floorColor);
					}
				}
			}
		}
	}
}
