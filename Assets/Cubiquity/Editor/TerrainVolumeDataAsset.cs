using UnityEngine;
using UnityEditor;

using System.Collections;
using System.IO;

namespace Cubiquity
{
	public class TerrainVolumeDataAsset : VolumeDataAsset
	{		
		public static TerrainVolumeData CreateFromVoxelDatabase(string relativePathToVoxelDatabase)
		{			
			TerrainVolumeData data = TerrainVolumeData.CreateFromVoxelDatabase(relativePathToVoxelDatabase);
			string assetName = Path.GetFileNameWithoutExtension(relativePathToVoxelDatabase);
			CreateAssetFromInstance<TerrainVolumeData>(data, assetName);
			return data;
		}
		
		public static TerrainVolumeData CreateEmptyVolumeData(Region region)
		{			
			TerrainVolumeData data = TerrainVolumeData.CreateEmptyVolumeData(region, Impl.Utility.GenerateRandomVoxelDatabaseName());
			CreateAssetFromInstance<TerrainVolumeData>(data);			
			return data;
		}
	}
}
