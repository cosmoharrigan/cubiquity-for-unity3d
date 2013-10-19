﻿using UnityEngine;
using System.Collections;
using System.IO;

public class TerrainVolumeFactory
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
		
		GameObject VoxelTerrainRoot = new GameObject(name);
		VoxelTerrainRoot.AddComponent<TerrainVolume>();
		
		TerrainVolume terrainVolume = VoxelTerrainRoot.GetComponent<TerrainVolume>();
		terrainVolume.region = region;
		terrainVolume.baseNodeSize = (int)baseNodeSize;
		
		TerrainVolumeData data = new TerrainVolumeData();
		data.pathToVoxels = datasetName;
		terrainVolume.data = data;
		
		terrainVolume.Initialize();
		
		return VoxelTerrainRoot;
	}
	
	public static GameObject CreateVolumeWithFloor(string name, Region region, string datasetName, uint floorDepth)
	{
		return CreateVolumeWithFloor (name, region, datasetName, DefaultBaseNodeSize, floorDepth);
	}
	
	public static GameObject CreateVolumeWithFloor(string name, Region region, string datasetName, uint baseNodeSize, uint floorDepth)
	{		
		// Make sure the Cubiquity library is installed.
		Installation.ValidateAndFix();
		
		// Make sure the page folder exists
		//CreateDatasetName(datasetName);
		
		GameObject VoxelTerrainRoot = new GameObject(name);
		VoxelTerrainRoot.AddComponent<TerrainVolume>();
		
		TerrainVolume terrainVolume = VoxelTerrainRoot.GetComponent<TerrainVolume>();
		terrainVolume.region = region;
		terrainVolume.baseNodeSize = (int)baseNodeSize;
		
		TerrainVolumeData data = new TerrainVolumeData();
		data.pathToVoxels = datasetName;
		terrainVolume.data = data;
		
		terrainVolume.InitializeWithFloor(floorDepth);
		
		return VoxelTerrainRoot;
	}
}
