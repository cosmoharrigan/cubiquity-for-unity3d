using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	[System.Serializable]
	public sealed class TerrainVolumeData
	{		
		public string pathToVoxels;
		
		// If set, this identifies the volume to the Cubiquity DLL. It can
		// be tested against null to find if the volume is currently valid.
		internal uint? volumeHandle = null;
		
		// The extents (dimensions in voxels) of the volume.		
		public Region region {get; private set;}
		
		public TerrainMaterial[] materials;
		
		public TerrainVolumeData(Region region, string pathToVoxels)
		{
			this.region = region;
			this.pathToVoxels = pathToVoxels;
			
			materials = new TerrainMaterial[License.MaxNoOfMaterials];
			
			for(int i = 0; i < materials.Length; i++)
			{
				materials[i] = new TerrainMaterial();
			}
		}
		
		public byte GetVoxel(int x, int y, int z, uint materialIndex)
		{
			byte materialStrength = 0;
			if(volumeHandle.HasValue)
			{
				CubiquityDLL.GetVoxelMC(volumeHandle.Value, x, y, z, materialIndex, out materialStrength);
			}
			return materialStrength;
		}
		
		public void SetVoxel(int x, int y, int z, uint materialIndex, byte materialStrength)
		{
			if(volumeHandle.HasValue)
			{
				if(x >= region.lowerCorner.x && y >= region.lowerCorner.y && z >= region.lowerCorner.z
					&& x <= region.upperCorner.x && y <= region.upperCorner.y && z <= region.upperCorner.z)
				{
					CubiquityDLL.SetVoxelMC(volumeHandle.Value, x, y, z, materialIndex, materialStrength);
				}
			}
		}
	}
}