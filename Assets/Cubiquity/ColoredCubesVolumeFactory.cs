using UnityEngine;
using System.Collections;
using System.IO;

public class ColoredCubesVolumeFactory
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
		VoxelTerrainRoot.AddComponent<ColoredCubesVolume>();
		
		ColoredCubesVolume coloredCubesVolume = VoxelTerrainRoot.GetComponent<ColoredCubesVolume>();
		coloredCubesVolume.region = region;
		coloredCubesVolume.baseNodeSize = (int)baseNodeSize;
		coloredCubesVolume.datasetName = datasetName;
		
		coloredCubesVolume.Initialize();
		
		return VoxelTerrainRoot;
	}
	
	public static GameObject CreateVolumeFromVolDat(string name, string voldatFolder, string datasetName)
	{
		return CreateVolumeFromVolDat(name, voldatFolder, datasetName, DefaultBaseNodeSize);
	}
	
	public static GameObject CreateVolumeFromVolDat(string name, string voldatFolder, string datasetName, uint baseNodeSize)
	{		
		// Make sure the Cubiquity library is installed.
		Installation.ValidateAndFix();
		
		// Make sure the page folder exists
		CreateDatasetName(datasetName);
		
		GameObject VoxelTerrainRoot = new GameObject(name);
		VoxelTerrainRoot.AddComponent<ColoredCubesVolume>();
		
		ColoredCubesVolume coloredCubesVolume = VoxelTerrainRoot.GetComponent<ColoredCubesVolume>();
		coloredCubesVolume.voldatFolder = voldatFolder;
		coloredCubesVolume.baseNodeSize = (int)baseNodeSize;
		coloredCubesVolume.datasetName = datasetName;
		
		coloredCubesVolume.Initialize();
		
		return VoxelTerrainRoot;
	}
	
	public static GameObject CreateVolumeFromHeightmap(string name, string heightmapFileName, string colormapFileName, string datasetName)
	{
		return CreateVolumeFromHeightmap(name, heightmapFileName, colormapFileName, datasetName, DefaultBaseNodeSize);
	}
	
	public static GameObject CreateVolumeFromHeightmap(string name, string heightmapFileName, string colormapFileName, string datasetName, uint baseNodeSize)
	{		
		// Make sure the Cubiquity library is installed.
		Installation.ValidateAndFix();
		
		// Make sure the page folder exists
		CreateDatasetName(datasetName);
		
		GameObject VoxelTerrainRoot = new GameObject(name);
		VoxelTerrainRoot.AddComponent<ColoredCubesVolume>();
		
		ColoredCubesVolume coloredCubesVolume = VoxelTerrainRoot.GetComponent<ColoredCubesVolume>();
		coloredCubesVolume.heightmapFileName = heightmapFileName;
		coloredCubesVolume.colormapFileName = colormapFileName;
		coloredCubesVolume.baseNodeSize = (int)baseNodeSize;
		coloredCubesVolume.datasetName = datasetName;
		
		coloredCubesVolume.Initialize();
		
		return VoxelTerrainRoot;
	}
	
	private static void CreateDatasetName(string datasetName)
	{
		string pathToData = Cubiquity.GetPathToData() + Path.DirectorySeparatorChar;
		System.IO.Directory.CreateDirectory(pathToData + datasetName);
		System.IO.Directory.CreateDirectory(pathToData + datasetName + "/override");
	}
}
