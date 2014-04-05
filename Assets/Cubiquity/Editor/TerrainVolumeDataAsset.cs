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
			
			if(pathToVoxelDatabase.Length == 0)
			{
				return null;
			}
				
			string relativePathToVoxelDatabase = Paths.MakeRelativePath(Paths.voxelDatabases + Path.DirectorySeparatorChar, pathToVoxelDatabase);
			
			// Pass through to the other version of the method.
			return CreateFromVoxelDatabase(relativePathToVoxelDatabase);
		}
		
		public static TerrainVolumeData CreateFromVoxelDatabase(string relativePathToVoxelDatabase)
		{			
			TerrainVolumeData data = TerrainVolumeData.CreateFromVoxelDatabase(relativePathToVoxelDatabase);
			string assetName = Path.GetFileNameWithoutExtension(relativePathToVoxelDatabase);
			CreateAssetFromInstance<TerrainVolumeData>(data, assetName);
			return data;
		}
		
		public static TerrainVolumeData CreateEmptyVolumeData()
		{
			// Get the dimensions
			int width = 128;
			int height = 32;
			int depth = 128;
			
			// Pass through to the other version of the method.
			return CreateEmptyVolumeData(new Region(0, 0, 0, width-1, height-1, depth-1));	
		}
		
		public static TerrainVolumeData CreateEmptyVolumeData(Region region)
		{			
			TerrainVolumeData data = TerrainVolumeData.CreateEmptyVolumeData(region, Impl.Utility.GenerateRandomVoxelDatabaseName());
			CreateAssetFromInstance<TerrainVolumeData>(data);			
			return data;
		}
		
		// The contents of this method are taken/derived from here:
		// http://wiki.unity3d.com/index.php?title=CreateScriptableObjectAsset
		private static void CreateAssetFromInstance<T> (T instance, string assetName = "") where T : ScriptableObject
		{	 
			string path = AssetDatabase.GetAssetPath (Selection.activeObject);
			if (path == "") 
			{
				path = "Assets";
			} 
			else if (Path.GetExtension (path) != "") 
			{
				path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
			}
			
			if(assetName == "")
			{
				assetName = "New " + typeof(T).ToString();
			}
	 
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/" + assetName + ".asset");
	 
			AssetDatabase.CreateAsset (instance, assetPathAndName);
	 
			AssetDatabase.SaveAssets ();
			EditorUtility.FocusProjectWindow ();
			Selection.activeObject = instance;
		}
	}
}
