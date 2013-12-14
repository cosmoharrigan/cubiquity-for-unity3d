using UnityEngine;

using System;
using System.IO;
using System.Collections;

namespace Cubiquity
{
	[System.Serializable]
	public abstract class VolumeData : ScriptableObject
	{
		public enum Paths { StreamingAssets, TemporaryCache };
		
		private Region cachedEnclosingRegion;
	    public Region enclosingRegion
	    {
	        get
			{
				if(cachedEnclosingRegion == null)
				{
					cachedEnclosingRegion = new Region(0, 0, 0, 0, 0, 0);
					CubiquityDLL.GetEnclosingRegion(volumeHandle.Value,
						out cachedEnclosingRegion.lowerCorner.x, out cachedEnclosingRegion.lowerCorner.y, out cachedEnclosingRegion.lowerCorner.z,
						out cachedEnclosingRegion.upperCorner.x, out cachedEnclosingRegion.upperCorner.y, out cachedEnclosingRegion.upperCorner.z);
				}
				
				return cachedEnclosingRegion;
			}
	    }
		
		[SerializeField]
		private Paths basePath;
		
		[SerializeField]
		private string relativePathToVoxelDatabase;
		
		protected string fullPathToVoxelDatabase
		{
			get
			{
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
		[System.NonSerialized] // Internal variables aren't serialized anyway?
		internal uint? volumeHandle = null;
		
		// Don't really like having this defined here. The base node size should be a rendering property rather than a
		// property of the actual volume data. Need to make this change in the underlying Cubiquity library as well though.
		protected static uint DefaultBaseNodeSize = 32;
		
		// We use a static Random for making filenames, as Randoms are seeded by timestamp
		// and client code could potentially create a number of volumes on quick sucession.  
		protected static System.Random randomIntGenerator = new System.Random();
		
		protected static VolumeDataType CreateFromVoxelDatabase<VolumeDataType>(Paths basePath, string relativePathToVoxelDatabase) where VolumeDataType : VolumeData
		{			
			VolumeDataType volumeData = ScriptableObject.CreateInstance<VolumeDataType>();
			volumeData.basePath = basePath;
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
			VolumeDataType volumeData = ScriptableObject.CreateInstance<VolumeDataType>();
			volumeData.cachedEnclosingRegion = region;
			volumeData.basePath = basePath;
			volumeData.relativePathToVoxelDatabase = relativePathToVoxelDatabase;
			
			volumeData.InitializeEmptyCubiquityVolume();
			
			return volumeData;
		}
		
		private void Awake()
		{
			// Warn about license restrictions.			
			Debug.LogWarning("This version of Cubiquity is for non-commercial and evaluation use only. Please see LICENSE.txt for further details.");
			
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
		}
		
		protected abstract void InitializeEmptyCubiquityVolume();
		protected abstract void InitializeExistingCubiquityVolume();
		protected abstract void ShutdownCubiquityVolume();
		
		public static string GeneratePathToVoxelDatabase()
		{
			// Generate a random filename from an integer
			return randomIntGenerator.Next().ToString("X8") + ".vdb";
		}
	}
}
