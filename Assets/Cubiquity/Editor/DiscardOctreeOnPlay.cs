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
				//foreach(Volume volume in Volume.allEnabled)
				Object[] volumes = Object.FindObjectsOfType(typeof(Volume));
				foreach(Object volume in volumes)
				{
					Debug.Log("Deleting root node");
					((Volume)volume).StopCoroutine("SynchronizationCoroutine");
					Object.DestroyImmediate(((Volume)volume).rootGameObject);
					((Volume)volume).rootGameObject = null;
					//volume.syncOnUpdate = false;
					//UpdateAllVolumes.syncVolumes = false;
				}
			}
	    }
	}
}
