using UnityEditor;
using UnityEngine;

namespace Cubiquity
{
	[InitializeOnLoad]
	class DiscardOctreeOnPlay
	{
	    static DiscardOctreeOnPlay()
	    {
	        EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
	    }
	 
	    static void OnPlaymodeStateChanged ()
	    {
			Debug.Log("OnPlaymodeStateChanged()");
			if(!EditorApplication.isPlaying)
			{
				foreach(Volume volume in Volume.all)
				{
					Debug.Log("Deleting root node");
					volume.StopCoroutine("SynchronizationCoroutine");
					Object.DestroyImmediate(volume.rootGameObject);
					volume.rootGameObject = null;
					//volume.syncOnUpdate = false;
					//UpdateAllVolumes.syncVolumes = false;
				}
			}
	    }
	}
}
