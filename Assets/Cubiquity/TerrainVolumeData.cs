﻿using UnityEngine;
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
		
		// Don't really like having this defined here. The base node size should be a rendering property rather than a
		// property of the actual volume data. Need to make this change in the underlying Cubiquity library as well though.
		private static uint DefaultBaseNodeSize = 32;
		
		private static int DefaultFloorDepth = 8;
		
		public TerrainVolumeData(Region region, string pathToVoxels)
		{
			this.region = region;
			this.pathToVoxels = pathToVoxels;
			
			volumeHandle = CubiquityDLL.NewTerrainVolume(region.lowerCorner.x, region.lowerCorner.y, region.lowerCorner.z,
						region.upperCorner.x, region.upperCorner.y, region.upperCorner.z, pathToVoxels, DefaultBaseNodeSize, 0, 0);
			
			CubiquityDLL.GenerateFloor(volumeHandle.Value, DefaultFloorDepth - 2, (uint)0, DefaultFloorDepth, (uint)1);
			
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