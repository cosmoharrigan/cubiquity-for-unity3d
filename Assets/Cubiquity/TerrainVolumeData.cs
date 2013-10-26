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
		/*[SerializeField]
		public Region region {get; private set;}*/
		
		[SerializeField]
		private Region _region;
	    public Region region
	    {
	        //set the person name
	        //set { this._region = value; }
	        //get the person name 
	        get { return this._region; }
	    }
		
		public TerrainMaterial[] materials;
		
		// Don't really like having this defined here. The base node size should be a rendering property rather than a
		// property of the actual volume data. Need to make this change in the underlying Cubiquity library as well though.
		private static uint DefaultBaseNodeSize = 32;
		
		private static uint DefaultFloorDepth = 8;
		
		public TerrainVolumeData(Region region, string pathToVoxels)
		{
			this._region = region;
			this.pathToVoxels = pathToVoxels;
			
			materials = new TerrainMaterial[License.MaxNoOfMaterials];
			
			for(int i = 0; i < materials.Length; i++)
			{
				materials[i] = new TerrainMaterial();
			}
		}

		internal void Initialize()
		{	
			// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
			// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.

			if(volumeHandle == null)
			{
				// Create an empty region of the desired size.
				volumeHandle = CubiquityDLL.NewTerrainVolume(region.lowerCorner.x, region.lowerCorner.y, region.lowerCorner.z,
					region.upperCorner.x, region.upperCorner.y, region.upperCorner.z, pathToVoxels, DefaultBaseNodeSize, 0, 0);
			}
		}
		
		internal void InitializeWithFloor()
		{	
			uint floorDepth = DefaultFloorDepth;
			
			// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
			// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.
			if(volumeHandle == null)
			{	
				// Create an empty region of the desired size.
				volumeHandle = CubiquityDLL.NewTerrainVolume(region.lowerCorner.x, region.lowerCorner.y, region.lowerCorner.z,
					region.upperCorner.x, region.upperCorner.y, region.upperCorner.z, pathToVoxels, DefaultBaseNodeSize, 1, floorDepth);
				
				//CubiquityDLL.GenerateFloor(volumeHandle.Value, (int)floorDepth - 2, (uint)0, (int)floorDepth, (uint)1);
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