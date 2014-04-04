using UnityEngine;

using System;
using System.IO;
using System.Collections;

using Cubiquity;
using Cubiquity.Impl;

namespace Cubiquity
{
	/// An implementation of VolumeData which stores a MaterialSet for each voxel.
	/**
	 * This class provides the actual 3D grid of material weights which are used by the TerrainVolume. You can use the provided interface to directly
	 * manipulate the volume by getting or setting the weights of each voxel.
	 */
	[System.Serializable]
	public sealed class TerrainVolumeData : VolumeData
	{
		/** 
		 * \copydoc CreateFromVoxelDatabase<VolumeDataType>(string)
		 */
		public static TerrainVolumeData CreateFromVoxelDatabase(string relativePathToVoxelDatabase)
		{
			return CreateFromVoxelDatabase<TerrainVolumeData>(relativePathToVoxelDatabase);
		}
		
		/** 
		 * \copydoc CreateEmptyVolumeData<VolumeDataType>(Region)
		 */
		public static TerrainVolumeData CreateEmptyVolumeData(Region region)
		{
			return CreateEmptyVolumeData<TerrainVolumeData>(region);
		}
		
		/** 
		 * \copydoc CreateEmptyVolumeData<VolumeDataType>(Region, string)
		 */
		public static TerrainVolumeData CreateEmptyVolumeData(Region region, string relativePathToVoxelDatabase)
		{
			return CreateEmptyVolumeData<TerrainVolumeData>(region, relativePathToVoxelDatabase);
		}
		
		/// Gets the material weights of the specified position.
		/**
		 * \param x The 'x' position of the voxel to get.
		 * \param y The 'y' position of the voxel to get.
		 * \param z The 'z' position of the voxel to get.
		 * \return The material weights of the voxel.
		 */
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
		
		/// Sets the material weights of the specified position.
		/**
		 * \param x The 'x' position of the voxel to set.
		 * \param y The 'y' position of the voxel to set.
		 * \param z The 'z' position of the voxel to set.
		 * \param materialSet The material weights the voxel should be set to.
		 */
		public void SetVoxel(int x, int y, int z, MaterialSet materialSet)
		{
			if(volumeHandle.HasValue)
			{
				if(x >= enclosingRegion.lowerCorner.x && y >= enclosingRegion.lowerCorner.y && z >= enclosingRegion.lowerCorner.z
					&& x <= enclosingRegion.upperCorner.x && y <= enclosingRegion.upperCorner.y && z <= enclosingRegion.upperCorner.z)
				{						
					CubiquityDLL.SetVoxelMC(volumeHandle.Value, x, y, z, materialSet);
				}
			}
		}
		
		/// \cond
		protected override void InitializeEmptyCubiquityVolume(Region region)
		{			
			// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
			// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.
			if(volumeHandle == null)
			{
				// Create an empty region of the desired size.
				volumeHandle = CubiquityDLL.NewEmptyTerrainVolume(region.lowerCorner.x, region.lowerCorner.y, region.lowerCorner.z,
					region.upperCorner.x, region.upperCorner.y, region.upperCorner.z, fullPathToVoxelDatabase, DefaultBaseNodeSize);
			}
		}
		/// \endcond
		
		/// \cond
		public override void InitializeExistingCubiquityVolume()
		{			
			// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
			// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.
			if(volumeHandle == null)
			{
				// Create an empty region of the desired size.
				volumeHandle = CubiquityDLL.NewTerrainVolumeFromVDB(fullPathToVoxelDatabase, DefaultBaseNodeSize);
			}
		}
		/// \endcond
		
		/// \cond
		public override void ShutdownCubiquityVolume()
		{
			// Shutdown could get called multiple times. E.g by OnDisable() and then by OnDestroy().
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
		/// \endcond
	}
}
