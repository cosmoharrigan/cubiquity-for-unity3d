using UnityEngine;
using UnityEditor;

namespace Cubiquity
{
	[ExecuteInEditMode]
	public class DiscardOctreeOnSave : UnityEditor.AssetModificationProcessor
	{
	    public static void OnWillSaveAssets( string[] assets )
	    {
			Debug.Log("In OnWillSaveAssets()");
			
			foreach(Volume volume in Volume.all)
			{
				Debug.Log ("  Discarding Octree");
				Object.DestroyImmediate(volume.rootGameObject);
			}
	    }
	}
}
