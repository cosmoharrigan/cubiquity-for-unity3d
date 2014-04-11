using UnityEngine;

using System;
using System.IO;
using System.Collections;

using Cubiquity;
using Cubiquity.Impl;

// VolumeData and it's subclasses are not in the Cubiquity namespace because it seems to cause problems with 'EditorGUILayout.ObjectField(...)'.
// Specifically the pop-up window (which appears when click the small circle with the dot in it) does not display the other volume data assets 
// in the project if the Cubiquity namespace is used. This appears to simply be a namespace-related bug in Unity.
//namespace Cubiquity
//{
	/// Base class representing the actual 3D grid of voxel values
	/**
	 * This class primarily serves as a light-weight wrapper around the \ref secVoxelDatabase "voxel databases" which are used by the %Cubiquity engine,
	 * allowing them to be treated as Unity3D assets. The voxel databases themselves are typically stored in the 'Streaming Assets' or 'Temporary Cache'
	 * folder (depending on the usage scenario), with the actual path being specified by the 'fullPathToVoxelDatabase' property. The VolumeData and it's
	 * subclasses then forward requests such as finding the dimensions of the voxel data or getting/setting the values of the individual voxels.
	 * 
	 * An instance of VolumeData can be created from an existing voxel database, or it can create an empty voxel database on demand. The class also
	 * abstracts the properties and functionality which are common to all types of volume data regardless of the type of the underlying voxel. Note that
	 * users should not interact with the VolumeData directly but should instead work with one of the derived classes.
	 * 
	 * \sa TerrainVolumeData, ColoredCubesVolumeData
	 */
	[System.Serializable]
	public abstract class VolumeData : ScriptableObject
	{
		private enum VoxelDatabasePaths { Streaming, Temporary };
		
		/// Gets the dimensions of the VolumeData.
		/**
		 * %Cubiquity voxel databases (and by extension the VolumeData) have a fixed size which is specified on creation. You should not attempt to access
		 * and location outside of this range.
		 * 
		 * \return The dimensions of the volume. The values are inclusive, so you can safely access the voxel at the positions given by Region.lowerCorner
		 * and Region.upperCorner.
		 */
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
		private VoxelDatabasePaths basePath;
		
		[SerializeField]
		private string relativePathToVoxelDatabase;
		
		/// Gets the full path to the voxel database which backs this instance.
		/**
		 * The full path to the voxel database is derived from the relative path which you can optionally specify at creation time, and the base path
		 * which depends on the way the instance was created. See CreateEmptyVolumeData<VolumeDataType>() and CreateFromVoxelDatabase<VolumeDataType>()
		 * for more information about how the base path is chosen.
		 * 
		 * This property is provided mostly for informational and debugging purposes as you are unlikely to directly make use of it.
		 * 
		 * \return The full path to the voxel database
		 */
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
				case VoxelDatabasePaths.Streaming:
					basePathString = Paths.voxelDatabases;
					break;
				case VoxelDatabasePaths.Temporary:
					basePathString = Application.temporaryCachePath;
					break;
				}
				return basePathString + Path.DirectorySeparatorChar + relativePathToVoxelDatabase;
			}
		}
		
		// If set, this identifies the volume to the Cubiquity DLL. It can
		// be tested against null to find if the volume is currently valid.
		/// \cond
		protected uint? mVolumeHandle = null;
		public uint? volumeHandle
		{
			get { return mVolumeHandle; }
			protected set { mVolumeHandle = value; }
		}
		/// \endcond
		
		// Don't really like having this defined here. The base node size should be a rendering property rather than a
		// property of the actual volume data. Need to make this change in the underlying Cubiquity library as well though.
		/// \cond
		protected static uint DefaultBaseNodeSize = 32;
		/// \endcond
		
		/**
		 * It is possible for %Cubiquity voxel databse files to be created outside of the %Cubiquity for Unity3D ecosystem (see the \ref secCubiquity
		 * "user manual" if you are not clear on the difference between 'Cubiquity and 'Cubiquity for Unity3D'). For example, the %Cubiquity SDK contains
		 * importers for converting a variety of external file formats into voxel databases. This function provides a way for you to create volume data
		 * which is linked to such a user provided voxel database.
		 * 
		 * \param relativePathToVoxelDatabase The path to the .vdb files which should be relative to the location given by Paths.voxelDatabases.
		 */
		public static VolumeDataType CreateFromVoxelDatabase<VolumeDataType>(string relativePathToVoxelDatabase) where VolumeDataType : VolumeData
		{			
			VolumeDataType volumeData = ScriptableObject.CreateInstance<VolumeDataType>();
			volumeData.basePath = VoxelDatabasePaths.Streaming;
			volumeData.relativePathToVoxelDatabase = relativePathToVoxelDatabase;
			
			volumeData.InitializeExistingCubiquityVolume();
			
			return volumeData;
		}
		
		/**
		 * This function will normally be used if you want to create volume data at runtime. Because no filename is provided the voxel database will be a temporary file and
		 * will be deleted when the volume data is destroyed. If you wish to create a persistant volume data then you should use the other version of this function.
		 * 
		 * Note that it therefore does not make sense to serialize instances of VolumeData created with this version of the function. The 'hideFlags' property is automatically
		 * set to 'HideFlags.DontSave' to prevent you from doing this.
		 * 
		 * \param region A Region instance specifying the dimensions of the volume data. You should not later try to access voxels outside of this range.
		 */
		public static VolumeDataType CreateEmptyVolumeData<VolumeDataType>(Region region) where VolumeDataType : VolumeData
		{
			string pathToCreateVoxelDatabase = Cubiquity.Impl.Utility.GenerateRandomVoxelDatabaseName();
			
			VolumeDataType volumeData = ScriptableObject.CreateInstance<VolumeDataType>();
			volumeData.basePath = VoxelDatabasePaths.Temporary;
			volumeData.relativePathToVoxelDatabase = pathToCreateVoxelDatabase;
		
			volumeData.hideFlags = HideFlags.DontSave; //Don't serialize this instance as it uses a temporary file for the voxel database.
			
			volumeData.InitializeEmptyCubiquityVolume(region);
			
			return volumeData;
		}
		
		/**
		 * This version of the function accepts a filename which will be given to the voxel database which is created. The path is relative to
		 * Paths.voxelDatabases and the voxel database will not be deleted when the volume data is destroyed, so you have the option to reuse it later.
		 * Note that you should only use this function from edit mode, as otherwise the streaming assets folder might not be writable (particularly
		 * standalone builds).
		 * 
		 * \param region A Region instance specifying the dimensions of the volume data. You should not later try to access voxels outside of this range.
		 * \param relativePathToVoxelDatabase The path where the voxel database should be created, repative to the location given by Paths.voxelDatabases.
		 */
		public static VolumeDataType CreateEmptyVolumeData<VolumeDataType>(Region region, string relativePathToVoxelDatabase) where VolumeDataType : VolumeData
		{
			// If the user is providing a name for the voxel database then it follows that they want to make use of it later.
			// In this case it should not be in the temp folder so we put it in streaming assets.
			if(Application.isPlaying)
			{
				Debug.LogWarning("You should not provide a path when creating empty volume data in play mode.");
			}
			
			VolumeDataType volumeData = ScriptableObject.CreateInstance<VolumeDataType>();
			volumeData.basePath = VoxelDatabasePaths.Streaming;
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
			// Note: For some reason this function is not called when transitioning between edit/play mode if this scriptable 
			// object has been turned into an asset. Therefore we also call Initialize...()/Shutdown...() from the Volume class.
			
			// This OnEnable() function is called as soon as the VolumeData is instantiated, but at this point it has not yet
			// been initilized with the path and so in this case we cannot yet initialize the underlying Cubiquity volume.
			if(relativePathToVoxelDatabase != null)
			{
				InitializeExistingCubiquityVolume();
			}
		}
		
		private void OnDisable()
		{
			// Note: For some reason this function is not called when transitioning between edit/play mode if this scriptable 
			// object has been turned into an asset. Therefore we also call Initialize...()/Shutdown...() from the Volume class.
			ShutdownCubiquityVolume();
		}
		
		private void OnDestroy()
		{
			// If the voxel database was created in the temporary location
			// then we can be sure the user has no further use for it.
			if(basePath == VoxelDatabasePaths.Temporary)
			{
				File.Delete(fullPathToVoxelDatabase);
				
				if(File.Exists(fullPathToVoxelDatabase))
				{
					Debug.LogWarning("Failed to delete voxel database from temporary cache");
				}
			}
		}
		
		/// \cond
		protected abstract void InitializeEmptyCubiquityVolume(Region region);
		public abstract void InitializeExistingCubiquityVolume();
		public abstract void ShutdownCubiquityVolume();
		/// \endcond
	}
//}
