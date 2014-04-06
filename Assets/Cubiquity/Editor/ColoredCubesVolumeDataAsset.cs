using UnityEngine;
using UnityEditor;

using System.Collections;
using System.IO;

namespace Cubiquity
{
	public class ColoredCubesVolumeDataAsset : VolumeDataAsset
	{		
		public static ColoredCubesVolumeData CreateFromVoxelDatabase(string relativePathToVoxelDatabase)
		{			
			ColoredCubesVolumeData data = ColoredCubesVolumeData.CreateFromVoxelDatabase(relativePathToVoxelDatabase);
			string assetName = Path.GetFileNameWithoutExtension(relativePathToVoxelDatabase);
			CreateAssetFromInstance<ColoredCubesVolumeData>(data, assetName);
			return data;
		}
		
		public static ColoredCubesVolumeData CreateEmptyVolumeData(Region region)
		{			
			ColoredCubesVolumeData data = ColoredCubesVolumeData.CreateEmptyVolumeData(region, Impl.Utility.GenerateRandomVoxelDatabaseName());
			CreateAssetFromInstance<ColoredCubesVolumeData>(data);			
			return data;
		}
	}
}
