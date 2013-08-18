using UnityEngine;
using System.Collections;

public static class Cubiquity
{
	// This is the relative path to where the volumes are stored on disk.
	public const string pathToData = "Volumes/";
	
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
}
