using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	public struct PickResult
	{
		public Vector3 volumeSpacePos;
		public Vector3 worldSpacePos;
	}
	
	public static class Picking
	{
		public static bool PickSurface(TerrainVolume volume, Vector3 origin, Vector3 direction, float distance, out PickResult pickResult)
		{
			direction *= distance;
			
			Transform volumeTransform = volume.transform;
			
			origin = volumeTransform.InverseTransformPoint(origin);
			direction = volumeTransform.InverseTransformDirection(direction);
			
			pickResult = new PickResult();
			uint hit = CubiquityDLL.PickTerrainSurface((uint)volume.data.volumeHandle,
				origin.x, origin.y, origin.z,
				direction.x, direction.y, direction.z,
				out pickResult.volumeSpacePos.x, out pickResult.volumeSpacePos.y, out pickResult.volumeSpacePos.z);
			
			pickResult.worldSpacePos = volumeTransform.TransformPoint(pickResult.volumeSpacePos);
			
			return hit == 1;
		}
		
		// This funcion should be implemented to find the point where the ray
		// pierces the mesh, between the last empty voxel and the first solid voxel.
		/*public static bool PickSurface(ColoredCubesVolume volume, Vector3 origin, Vector3 direction, float distance, PickResult pickResult)
		{
		}*/
	}
}
