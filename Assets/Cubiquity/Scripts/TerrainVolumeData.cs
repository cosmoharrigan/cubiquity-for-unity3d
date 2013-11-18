using UnityEngine;

using System;
using System.IO;
using System.Collections;

namespace Cubiquity
{
	[System.Serializable]
	public sealed class TerrainVolumeData : VolumeData
	{		
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
		
		public MaterialSet GetVoxel(int x, int y, int z)
		{
			MaterialSet materialSet;
			if(volumeHandle.HasValue)
			{
				CubiquityDLL.GetVoxelMC(volumeHandle.Value, x, y, z, out materialSet);
			}
			else
			{
				// Should maybe throw instead?
				materialSet = new MaterialSet();
			}
			return materialSet;
		}
		
		public void SetVoxel(int x, int y, int z, MaterialSet materialSet)
		{
			if(volumeHandle.HasValue)
			{
				if(x >= region.lowerCorner.x && y >= region.lowerCorner.y && z >= region.lowerCorner.z
					&& x <= region.upperCorner.x && y <= region.upperCorner.y && z <= region.upperCorner.z)
				{						
					CubiquityDLL.SetVoxelMC(volumeHandle.Value, x, y, z, materialSet);
				}
			}
		}
		
		private void OnEnable()
		{
			InitializeCubiquityVolume();
		}
		
		private void OnDisable()
		{
			if(volumeHandle.HasValue)
			{
				// We only save if we are in editor mode, not if we are playing.
				bool saveChanges = !Application.isPlaying;
				
				if(saveChanges)
				{
					CubiquityDLL.AcceptOverrideBlocksMC(volumeHandle.Value);
				}
				CubiquityDLL.DiscardOverrideBlocksMC(volumeHandle.Value);
				
				CubiquityDLL.DeleteTerrainVolume(volumeHandle.Value);
				volumeHandle = null;
			}
		}

		private void InitializeCubiquityVolume()
		{	
			// Make sure the Cubiquity library is installed.
			Installation.ValidateAndFix();
			
			// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
			// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.
			if((volumeHandle == null) && (_region != null))
			{
				// Create an empty region of the desired size.
				volumeHandle = CubiquityDLL.NewTerrainVolume(region.lowerCorner.x, region.lowerCorner.y, region.lowerCorner.z,
					region.upperCorner.x, region.upperCorner.y, region.upperCorner.z, pathToVoxels, DefaultBaseNodeSize, 0, 0);
			}
		}
	}
}