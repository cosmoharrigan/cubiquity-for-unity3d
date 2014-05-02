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
			
			TerrainVolumeData data = VolumeDataAsset.CreateEmptyVolumeData<TerrainVolumeData>(new Region(0, 0, 0, width-1, height-1, depth-1));
			
			// Create some ground in the terrain so it shows up in the editor.
			// Soil as a base (mat 1) and then a couple of layers of grass (mat 2).
			TerrainVolumeGenerator.GenerateFloor(data, 6, (uint)1, 8, (uint)2);
			
			// Now create the terrain game object from the data.
			GameObject terrain = TerrainVolume.CreateGameObject(data, true, true);
			
			// And select it, so the user can get straight on with editing.
			Selection.activeGameObject = terrain;
		}
		
		[MenuItem ("Assets/Create/Terrain Volume Data/Empty Volume Data...")]
		static void CreateEmptyTerrainVolumeDataAsset()
		{			
			ScriptableWizard.DisplayWizard<CreateEmptyTerrainVolumeDataAssetWizard>("Create Terrain Volume Data", "Create");
		}
		
		[MenuItem ("Assets/Create/Terrain Volume Data/From Voxel Database...")]
		static void CreateTerrainVolumeDataAssetFromVoxelDatabase()
		{	
			string pathToVoxelDatabase = EditorUtility.OpenFilePanel("Choose a Voxel Database (.vdb) file to load", Paths.voxelDatabases, "vdb");
			
			if(pathToVoxelDatabase.Length != 0)
			{
				string relativePathToVoxelDatabase = Paths.MakeRelativePath(Paths.voxelDatabases + Path.DirectorySeparatorChar, pathToVoxelDatabase);
			
				// Pass through to the other version of the method.
				VolumeDataAsset.CreateFromVoxelDatabase<TerrainVolumeData>(relativePathToVoxelDatabase);
			}
		}
		
		[MenuItem ("GameObject/Create Other/Colored Cubes Volume")]
		static void CreateColoredCubesVolume()
		{
			int width = 256;
			int height = 32;
			int depth = 256;
			
			ColoredCubesVolumeData data = VolumeDataAsset.CreateEmptyVolumeData<ColoredCubesVolumeData>(new Region(0, 0, 0, width-1, height-1, depth-1));
			
			GameObject coloredCubesGameObject = ColoredCubesVolume.CreateGameObject(data, true, true);
			
			// And select it, so the user can get straight on with editing.
			Selection.activeGameObject = coloredCubesGameObject;
			
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
		
		[MenuItem ("Assets/Create/Colored Cubes Volume Data/Empty Volume Data...")]
		static void CreateEmptyColoredCubesVolumeDataAsset()
		{			
			ScriptableWizard.DisplayWizard<CreateEmptyColoredCubesVolumeDataAssetWizard>("Create Colored Cubes Volume Data", "Create");
		}
		
		[MenuItem ("Assets/Create/Colored Cubes Volume Data/From Voxel Database...")]
		static void CreateColoredCubesVolumeDataAssetFromVoxelDatabase()
		{	
			string pathToVoxelDatabase = EditorUtility.OpenFilePanel("Choose a Voxel Database (.vdb) file to load", Paths.voxelDatabases, "vdb");
			
			if(pathToVoxelDatabase.Length != 0)
			{
				string relativePathToVoxelDatabase = Paths.MakeRelativePath(Paths.voxelDatabases + Path.DirectorySeparatorChar, pathToVoxelDatabase);
			
				// Pass through to the other version of the method.
				VolumeDataAsset.CreateFromVoxelDatabase<ColoredCubesVolumeData>(relativePathToVoxelDatabase);
			}
		}
	}
}
