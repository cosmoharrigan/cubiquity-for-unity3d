using UnityEngine;
using UnityEditor;

using System.Collections;
using System.IO;

namespace Cubiquity
{
	public class VolumeDataAsset
	{		
		// The contents of this method are taken/derived from here:
		// http://wiki.unity3d.com/index.php?title=CreateScriptableObjectAsset
		protected static void CreateAssetFromInstance<T> (T instance, string assetName = "") where T : ScriptableObject
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
