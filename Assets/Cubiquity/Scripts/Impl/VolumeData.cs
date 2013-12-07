using UnityEngine;

using System;
using System.IO;
using System.Collections;

namespace Cubiquity
{
	[System.Serializable]
	public abstract class VolumeData : ScriptableObject
	{
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
		protected string pathToVoxelDatabase;
		
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
		
		protected static VolumeDataType CreateFromVoxelDatabase<VolumeDataType>(string pathToVoxelDatabase) where VolumeDataType : VolumeData
		{
			if(!File.Exists(pathToVoxelDatabase))
			{
				throw new FileNotFoundException("Voxel database '" + pathToVoxelDatabase + "' does not exist (or you do not have the required permissions)");
			}
			
			VolumeDataType volumeData = ScriptableObject.CreateInstance<VolumeDataType>();
			volumeData.pathToVoxelDatabase = pathToVoxelDatabase;
			
			volumeData.InitializeExistingCubiquityVolume();
			
			return volumeData;
		}
		
		protected static VolumeDataType CreateEmptyVolumeData<VolumeDataType>(Region region) where VolumeDataType : VolumeData
		{
			string pathToCreateVoxelDatabase = GeneratePathToVoxelDatabase();
			return CreateEmptyVolumeData<VolumeDataType>(region, pathToCreateVoxelDatabase);
		}
		
		protected static VolumeDataType CreateEmptyVolumeData<VolumeDataType>(Region region, string pathToCreateVoxelDatabase) where VolumeDataType : VolumeData
		{
			if(File.Exists(pathToCreateVoxelDatabase))
			{
				throw new FileNotFoundException("Voxel database '" + pathToCreateVoxelDatabase + "' already exists. Please choose a different filename.");
			}
			
			VolumeDataType volumeData = ScriptableObject.CreateInstance<VolumeDataType>();
			volumeData.cachedEnclosingRegion = region;
			volumeData.pathToVoxelDatabase = pathToCreateVoxelDatabase;
			
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
			InitializeExistingCubiquityVolume();
		}
		
		private void OnDisable()
		{
			ShutdownCubiquityVolume();
		}
		
		private void OnDestroy()
		{
			ShutdownCubiquityVolume();
		}
		
		protected abstract void InitializeEmptyCubiquityVolume();
		protected abstract void InitializeExistingCubiquityVolume();
		protected abstract void ShutdownCubiquityVolume();
		
		protected static string GeneratePathToVoxelDatabase()
		{
			// Generate a random filename from an integer
			string filename = randomIntGenerator.Next().ToString("X8") + ".vdb";
			return Application.streamingAssetsPath + Path.DirectorySeparatorChar + filename;
		}
	}
}
