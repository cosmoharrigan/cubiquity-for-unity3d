using UnityEngine;
using System.Collections;
using System.IO;

namespace Cubiquity
{
	public static class ColoredCubesVolumePicking
	{
		public static bool PickFirstSolidVoxel(ColoredCubesVolume volume, float rayStartX, float rayStartY, float rayStartZ, float rayDirX, float rayDirY, float rayDirZ, out int resultX, out int resultY, out int resultZ)
		{
			Transform volumeTransform = volume.transform;
			
			Vector3 start = new Vector3(rayStartX, rayStartY, rayStartZ);
			Vector3 direction = new Vector3(rayDirX, rayDirY, rayDirZ);
			
			start = volumeTransform.InverseTransformPoint(start);
			direction = volumeTransform.InverseTransformDirection(direction);
			
			rayStartX = start.x;
			rayStartY = start.y;
			rayStartZ = start.z;
			
			rayDirX = direction.x;
			rayDirY = direction.y;
			rayDirZ = direction.z;
			
			uint hit = CubiquityDLL.PickFirstSolidVoxel((uint)volume.data.volumeHandle, rayStartX, rayStartY, rayStartZ, rayDirX, rayDirY, rayDirZ, out resultX, out resultY, out resultZ);
			
			return hit == 1;
		}
		
		public static bool PickLastEmptyVoxel(ColoredCubesVolume volume, float rayStartX, float rayStartY, float rayStartZ, float rayDirX, float rayDirY, float rayDirZ, out int resultX, out int resultY, out int resultZ)
		{
			Transform volumeTransform = volume.transform;
			
			Vector3 start = new Vector3(rayStartX, rayStartY, rayStartZ);
			Vector3 direction = new Vector3(rayDirX, rayDirY, rayDirZ);
			
			start = volumeTransform.InverseTransformPoint(start);
			direction = volumeTransform.InverseTransformDirection(direction);
			
			rayStartX = start.x;
			rayStartY = start.y;
			rayStartZ = start.z;
			
			rayDirX = direction.x;
			rayDirY = direction.y;
			rayDirZ = direction.z;
			
			uint hit = CubiquityDLL.PickLastEmptyVoxel((uint)volume.data.volumeHandle, rayStartX, rayStartY, rayStartZ, rayDirX, rayDirY, rayDirZ, out resultX, out resultY, out resultZ);
			
			return hit == 1;
		}
	}
}
