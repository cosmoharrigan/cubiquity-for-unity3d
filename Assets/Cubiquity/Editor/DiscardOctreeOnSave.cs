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
			
			Object[] volumes = Object.FindObjectsOfType(typeof(Volume));
			foreach(Object volume in volumes)
			{
				Debug.Log ("  Discarding Octree");
				Object.DestroyImmediate(((Volume)volume).rootGameObject);
			}
	    }
	}
}
