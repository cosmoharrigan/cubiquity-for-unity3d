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
			//this.basePath = basePath;
			//this.relativePathToVoxelDatabase = relativePathToVoxelDatabase;
			
			/*if(!File.Exists(fullPathToVoxelDatabase))
			{
				throw new FileNotFoundException("Voxel database '" + fullPathToVoxelDatabase + "' does not exist (or you do not have the required permissions)");
			}*/
			
			/*if(!IsFileSomewhereInFolder(pathToVoxelDatabase, Application.streamingAssetsPath))
			{
				//FIXME - Better exception type
				throw new Exception("The voxel database must be inside the StreamingAssets folder");
			}*/
			
			/*Uri uriToVoxelDatabase = new Uri(pathToVoxelDatabase);	
			Uri uriToStreamingAssets = new Uri(Application.streamingAssetsPath);			
			Uri relativeUri = uriToStreamingAssets.MakeRelativeUri(uriToVoxelDatabase);			
			string relativePathToVoxelDatabase = relativeUri.ToString();*/
			
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
			//this.basePath = basePath;
			
			/*if(File.Exists(pathToCreateVoxelDatabase))
			{
				throw new FileNotFoundException("Voxel database '" + pathToCreateVoxelDatabase + "' already exists. Please choose a different filename.");
			}*/
			
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
			if(relativePathToVoxelDatabase != null)
			{
				InitializeExistingCubiquityVolume();
			}
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
		
		public static string GeneratePathToVoxelDatabase()
		{
			// Generate a random filename from an integer
			return randomIntGenerator.Next().ToString("X8") + ".vdb";
		}
		
		private static bool IsInsideStreamingAssets(string pathToVoxelDatabase)
		{
			string pathToContainingFolder = Path.GetDirectoryName(pathToVoxelDatabase);
			return IsSubfolder(Application.streamingAssetsPath, pathToContainingFolder);
		}
		
		// Based on http://stackoverflow.com/a/7710620
		private static bool IsSubfolder(string parentPath, string childPath)
	    {
	        Uri parentUri = new Uri( parentPath ) ;
	
	        DirectoryInfo childUri = new DirectoryInfo( childPath ).Parent ;
	
	        while( childUri != null )
	        {
	            if( new Uri( childUri.FullName ) == parentUri )
	            {
	                return true ;
	            }
	
	            childUri = childUri.Parent ;
	        }
	
	        return false ;
	    }
		
		private static bool IsFileSomewhereInFolder(string fileToFind, string folderToSearch)
	    {
	        Uri parentUri = new Uri( folderToSearch ) ;
	
	        DirectoryInfo childUri = new DirectoryInfo( fileToFind ).Parent ;
	
	        while( childUri != null )
	        {
	            if( new Uri( childUri.FullName ) == parentUri )
	            {
	                return true ;
	            }
	
	            childUri = childUri.Parent ;
	        }
	
	        return false ;
	    }
	}
}
