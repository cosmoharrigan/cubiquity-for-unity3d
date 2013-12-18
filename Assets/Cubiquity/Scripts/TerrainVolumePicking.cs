using UnityEngine;
using System.Collections;
using System.IO;

namespace Cubiquity
{
	public static class TerrainVolumePicking
	{
		public static bool PickTerrainSurface(TerrainVolume volume, float rayStartX, float rayStartY, float rayStartZ, float rayDirX, float rayDirY, float rayDirZ, out float resultX, out float resultY, out float resultZ)
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
			
			uint hit = CubiquityDLL.PickTerrainSurface((uint)volume.data.volumeHandle, rayStartX, rayStartY, rayStartZ, rayDirX, rayDirY, rayDirZ, out resultX, out resultY, out resultZ);
			
			return hit == 1;
		}
	}
}
