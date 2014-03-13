using UnityEngine;
using UnityEditor;

using System.Collections;
using System.IO;

namespace Cubiquity
{
	public class TerrainVolumeDataAsset
	{
		public static TerrainVolumeData CreateFromVoxelDatabase()
		{
			string pathToVoxelDatabase = EditorUtility.OpenFilePanel("Choose a Voxel Database (.vdb) file to load", Paths.voxelDatabases, "vdb");
			string relativePathToVoxelDatabase = Paths.MakeRelativePath(Paths.voxelDatabases + Path.DirectorySeparatorChar, pathToVoxelDatabase);
			TerrainVolumeData data = TerrainVolumeData.CreateFromVoxelDatabase(relativePathToVoxelDatabase);
			ScriptableObjectUtility.CreateAssetFromInstance<TerrainVolumeData>(data);
			return data;
		}
		
		public static TerrainVolumeData CreateFromVoxelDatabase(string relativePathToVoxelDatabase)
		{
			TerrainVolumeData data = TerrainVolumeData.CreateFromVoxelDatabase(relativePathToVoxelDatabase);
			ScriptableObjectUtility.CreateAssetFromInstance<TerrainVolumeData>(data);
			return data;
		}
		
		/*protected static VolumeDataType CreateEmptyVolumeData<VolumeDataType>(Region region) where VolumeDataType : VolumeData
		{
		}*/
	}
}
