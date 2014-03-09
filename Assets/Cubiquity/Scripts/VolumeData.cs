using UnityEngine;

using System;
using System.IO;
using System.Collections;

using Cubiquity;
using Cubiquity.Impl;

namespace Cubiquity
{
	[System.Serializable]
	public abstract class VolumeData : ScriptableObject
	{
		public enum Paths { StreamingAssets, TemporaryCache };
		
	    public Region enclosingRegion
	    {
	        get
			{
				Region result = new Region(0, 0, 0, 0, 0, 0);
				CubiquityDLL.GetEnclosingRegion(volumeHandle.Value,
					out result.lowerCorner.x, out result.lowerCorner.y, out result.lowerCorner.z,
					out result.upperCorner.x, out result.upperCorner.y, out result.upperCorner.z);
				
				return result;
			}
	    }
		
		[SerializeField]
		private Paths basePath;
		
		[SerializeField]
		private string relativePathToVoxelDatabase;
		
		public string fullPathToVoxelDatabase
		{
			get
			{
				if(relativePathToVoxelDatabase.Length == 0)
				{
					throw new System.ArgumentException(@"The relative path to the voxel database should never be empty.
						Perhaps you created the VolumeData with ScriptableObject.CreateInstance(), rather than with
						CreateEmptyVolumeData() or CreateFromVoxelDatabase()?");
				}
				
				string basePathString = null;
				switch(basePath)
				{
				case Paths.StreamingAssets:
					basePathString = Application.streamingAssetsPath;
					break;
				case Paths.TemporaryCache:
					basePathString = Application.temporaryCachePath;
					break;
				}
				return basePathString + Path.DirectorySeparatorChar + relativePathToVoxelDatabase;
			}
		}
		
		// If set, this identifies the volume to the Cubiquity DLL. It can
		// be tested against null to find if the volume is currently valid.
		protected uint? mVolumeHandle = null;
		public uint? volumeHandle
		{
			get { return mVolumeHandle; }
			protected set { mVolumeHandle = value; }
		}
		
		// Don't really like having this defined here. The base node size should be a rendering property rather than a
		// property of the actual volume data. Need to make this change in the underlying Cubiquity library as well though.
		protected static uint DefaultBaseNodeSize = 32;
		
		// We use a static Random for making filenames, as Randoms are seeded by timestamp
		// and client code could potentially create a number of volumes on quick sucession.  
		protected static System.Random randomIntGenerator = new System.Random();
		
		protected static VolumeDataType CreateFromVoxelDatabase<VolumeDataType>(string relativePathToVoxelDatabase) where VolumeDataType : VolumeData
		{			
			VolumeDataType volumeData = ScriptableObject.CreateInstance<VolumeDataType>();
			volumeData.basePath = Paths.StreamingAssets;
			volumeData.relativePathToVoxelDatabase = relativePathToVoxelDatabase;
			
			volumeData.InitializeExistingCubiquityVolume();
			
			return volumeData;
		}
		
		protected static VolumeDataType CreateEmptyVolumeData<VolumeDataType>(Region region) where VolumeDataType : VolumeData
		{
			string pathToCreateVoxelDatabase = GeneratePathToVoxelDatabase();
			return CreateEmptyVolumeData<VolumeDataType>(region, Paths.TemporaryCache, pathToCreateVoxelDatabase);
		}
		
		protected static VolumeDataType CreateEmptyVolumeData<VolumeDataType>(Region region, Paths basePath, string relativePathToVoxelDatabase) where VolumeDataType : VolumeData
		{
			if(Application.isPlaying && basePath == Paths.StreamingAssets)
			{
				Debug.LogWarning("You should not create a voxel database in 'Paths.StreamingAssets' when in play mode. Use 'Paths.TemporaryCache' instead");
			}
			
			VolumeDataType volumeData = ScriptableObject.CreateInstance<VolumeDataType>();
			volumeData.basePath = basePath;
			volumeData.relativePathToVoxelDatabase = relativePathToVoxelDatabase;
			
			volumeData.InitializeEmptyCubiquityVolume(region);
			
			return volumeData;
		}
		
		private void Awake()
		{
			// Make sure the Cubiquity library is installed.
			Installation.ValidateAndFix();
		}
		
		private void OnEnable()
		{			
			// This OnEnable() function is called as soon as the VolumeData is instantiated, but at this point it has not yet
			// been initilized with the path and so in this case we cannot yet initialize the underlying Cubiquity volume.
			if(relativePathToVoxelDatabase != null)
			{
				InitializeExistingCubiquityVolume();
			}
		}
		
		private void OnDisable()
		{
			ShutdownCubiquityVolume();
			
			// If the voxel database was created in the temporary cache
			// then we can be sure the user has no further use for it.
			if(basePath == Paths.TemporaryCache)
			{
				File.Delete(fullPathToVoxelDatabase);
			}
			
			if(File.Exists(fullPathToVoxelDatabase))
			{
				Debug.LogWarning("Failed to delete voxel database from temporary cache");
			}
		}
		
		protected abstract void InitializeEmptyCubiquityVolume(Region region);
		protected abstract void InitializeExistingCubiquityVolume();
		protected abstract void ShutdownCubiquityVolume();
		
		public static string GeneratePathToVoxelDatabase()
		{
			// Generate a random filename from an integer
			return randomIntGenerator.Next().ToString("X8") + ".vdb";
		}
	}
}
