using UnityEngine;
using System.Collections;
using System.IO;

public static class Cubiquity
{
	// This is the path to where the volumes are stored on disk.
	public static string volumesPath
	{
		get
		{
			string pathToData = System.IO.Path.Combine(Application.streamingAssetsPath, "Cubiquity/Volumes");
			return pathToData;
		}
	}
	
	public static bool PickFirstSolidVoxel(ColoredCubesVolume volume, float rayStartX, float rayStartY, float rayStartZ, float rayDirX, float rayDirY, float rayDirZ, out int resultX, out int resultY, out int resultZ)
	{
		uint hit = CubiquityDLL.PickFirstSolidVoxel((uint)volume.volumeHandle, rayStartX, rayStartY, rayStartZ, rayDirX, rayDirY, rayDirZ, out resultX, out resultY, out resultZ);
		
		return hit == 1;
	}
	
	public static bool PickLastEmptyVoxel(ColoredCubesVolume volume, float rayStartX, float rayStartY, float rayStartZ, float rayDirX, float rayDirY, float rayDirZ, out int resultX, out int resultY, out int resultZ)
	{
		uint hit = CubiquityDLL.PickLastEmptyVoxel((uint)volume.volumeHandle, rayStartX, rayStartY, rayStartZ, rayDirX, rayDirY, rayDirZ, out resultX, out resultY, out resultZ);
		
		return hit == 1;
	}
	
	public static bool PickTerrainSurface(SmoothTerrainVolume volume, float rayStartX, float rayStartY, float rayStartZ, float rayDirX, float rayDirY, float rayDirZ, out float resultX, out float resultY, out float resultZ)
	{
		uint hit = CubiquityDLL.PickTerrainSurface((uint)volume.volumeHandle, rayStartX, rayStartY, rayStartZ, rayDirX, rayDirY, rayDirZ, out resultX, out resultY, out resultZ);
		
		return hit == 1;
	}
}
