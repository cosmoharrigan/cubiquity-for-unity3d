using UnityEngine;
using System.Collections;
using System.IO;

namespace Cubiquity
{
	public class TerrainVolumeFactory
	{
		private static uint DefaultBaseNodeSize = 32;
		
		public static GameObject CreateVolume(string name, Region region)
		{
			return CreateVolume (name, region, DefaultBaseNodeSize);
		}
		
		public static GameObject CreateVolume(string name, Region region, uint baseNodeSize)
		{		
			//TerrainVolumeData data = new TerrainVolumeData(region, datasetName);			
			TerrainVolumeData data = ScriptableObject.CreateInstance<TerrainVolumeData>();
			data.Init(region);
			return TerrainVolume.CreateGameObject(data);
		}
		
		public static GameObject CreateVolumeWithFloor(string name, Region region, uint floorDepth)
		{
			return CreateVolumeWithFloor (name, region, DefaultBaseNodeSize, floorDepth);
		}
		
		public static GameObject CreateVolumeWithFloor(string name, Region region, uint baseNodeSize, uint floorDepth)
		{		
			// Make sure the Cubiquity library is installed.
			Installation.ValidateAndFix();
			
			// Make sure the page folder exists
			//CreateDatasetName(datasetName);
			
			GameObject VoxelTerrainRoot = new GameObject(name);
			VoxelTerrainRoot.AddComponent<TerrainVolume>();
			
			TerrainVolume terrainVolume = VoxelTerrainRoot.GetComponent<TerrainVolume>();
			//terrainVolume.baseNodeSize = (int)baseNodeSize;
			
			//TerrainVolumeData data = ScriptableObjectUtility.CreateAsset<TerrainVolumeData>();
			//TerrainVolumeData data = new TerrainVolumeData(region, datasetName);
			TerrainVolumeData data = ScriptableObject.CreateInstance<TerrainVolumeData>();
			data.Init(region);
			
			terrainVolume.data = data;
			
			return VoxelTerrainRoot;
		}
	}
}
