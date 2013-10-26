using UnityEngine;

using System;
using System.IO;
using System.Collections;

namespace Cubiquity
{
	[System.Serializable]
	public sealed class TerrainVolumeData : ScriptableObject
	{		
		[SerializeField]
		private string pathToVoxels;
		
		// If set, this identifies the volume to the Cubiquity DLL. It can
		// be tested against null to find if the volume is currently valid.
		[System.NonSerialized] // Internal variables aren't serialized anyway?
		internal uint? volumeHandle = null;
		
		// We need to explicitly serialize the private field because
		// Unity3D doesn't automatically serialize the public property
		[SerializeField]
		private Region _region;
	    public Region region
	    {
	        get { return this._region; }
	    }
		
		public TerrainMaterial[] materials;
		
		// Don't really like having this defined here. The base node size should be a rendering property rather than a
		// property of the actual volume data. Need to make this change in the underlying Cubiquity library as well though.
		private static uint DefaultBaseNodeSize = 32;
		
		// We use a static Random for making filenames, as Randoms are seeded by timestamp
		// and client code could potentially create a number of volumes on quick sucession.  
		private static System.Random randomIntGenerator = new System.Random();
		
		public TerrainVolumeData()
		{			
			materials = new TerrainMaterial[License.MaxNoOfMaterials];
			
			for(int i = 0; i < materials.Length; i++)
			{
				materials[i] = new TerrainMaterial();
			}
		}
		
		// Ideally we will get rid of this function in the future. It is needed at the moment because the Region cannot be
		// specified via the constructor as this class is created with ScriptableObject.CreateInstance(). However, Init() get
		// called after OnEnable() (which will have failed to create the Cubiquity volume due to not having the Region) so we
		// have to try and create the cubiquity volume again.
		//
		// This should improve once we remove the concept of Cubiquity volumes needing a region.
		public void Init(Region region)
		{
			this._region = region;
			this.pathToVoxels = GeneratePathToVoxels();
			
			InitializeCubiquityVolume();
		}
		
		void OnEnable()
		{
			InitializeCubiquityVolume();
		}

		internal void InitializeCubiquityVolume()
		{	
			// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
			// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.

			if((volumeHandle == null) && (_region != null))
			{
				// Create an empty region of the desired size.
				volumeHandle = CubiquityDLL.NewTerrainVolume(region.lowerCorner.x, region.lowerCorner.y, region.lowerCorner.z,
					region.upperCorner.x, region.upperCorner.y, region.upperCorner.z, pathToVoxels, DefaultBaseNodeSize, 0, 0);
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
		
		private string GeneratePathToVoxels()
		{
			// Generate a random filename from an integer
			string filename = randomIntGenerator.Next().ToString("X8") + ".vol";
			return Application.streamingAssetsPath + Path.DirectorySeparatorChar + filename;
		}
	}
}