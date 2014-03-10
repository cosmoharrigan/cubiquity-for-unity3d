using UnityEngine;
using UnityEditor;
using System.IO;

namespace Cubiquity
{
	// The contents of this calss are taken/derived from here:
	// http://wiki.unity3d.com/index.php?title=CreateScriptableObjectAsset
	public static class ScriptableObjectUtility
	{
		public static T CreateAsset<T> () where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T> ();
	 
			string path = AssetDatabase.GetAssetPath (Selection.activeObject);
			if (path == "") 
			{
				path = "Assets";
			} 
			else if (Path.GetExtension (path) != "") 
			{
				path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
			}
	 
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof(T).ToString() + ".asset");
	 
			AssetDatabase.CreateAsset (asset, assetPathAndName);
	 
			AssetDatabase.SaveAssets ();
			EditorUtility.FocusProjectWindow ();
			Selection.activeObject = asset;
			
			return asset;
		}
		
		public static void CreateAssetFromInstance<T> (T instance) where T : ScriptableObject
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
	 
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof(T).ToString() + ".asset");
	 
			AssetDatabase.CreateAsset (instance, assetPathAndName);
	 
			AssetDatabase.SaveAssets ();
			EditorUtility.FocusProjectWindow ();
			Selection.activeObject = instance;
		}
	}
}
