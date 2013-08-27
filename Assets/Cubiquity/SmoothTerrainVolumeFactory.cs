using UnityEngine;
using System.Collections;
using System.IO;

public class SmoothTerrainVolumeFactory
{
	private static uint DefaultBaseNodeSize = 32;
	
	public static GameObject CreateVolume(string name, Region region, string datasetName)
	{
		return CreateVolume (name, region, datasetName, DefaultBaseNodeSize);
	}
	
	public static GameObject CreateVolume(string name, Region region, string datasetName, uint baseNodeSize)
	{		
		// Make sure the Cubiquity library is installed.
		Installation.ValidateAndFix();
		
		// Make sure the page folder exists
		CreateDatasetName(datasetName);
		
		GameObject VoxelTerrainRoot = new GameObject(name);
		VoxelTerrainRoot.AddComponent<SmoothTerrainVolume>();
		
		SmoothTerrainVolume smoothTerrainVolume = VoxelTerrainRoot.GetComponent<SmoothTerrainVolume>();
		smoothTerrainVolume.region = region;
		smoothTerrainVolume.baseNodeSize = (int)baseNodeSize;
		smoothTerrainVolume.datasetName = datasetName;
		
		smoothTerrainVolume.Initialize();
		
		return VoxelTerrainRoot;
	}
	
	private static void CreateDatasetName(string datasetName)
	{
		string pathToData = Cubiquity.volumesPath + Path.DirectorySeparatorChar;
		System.IO.Directory.CreateDirectory(pathToData + datasetName);
		System.IO.Directory.CreateDirectory(pathToData + datasetName + "/override");
	}
}
