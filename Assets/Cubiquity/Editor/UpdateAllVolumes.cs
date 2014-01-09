using UnityEditor;
using UnityEngine;

namespace Cubiquity
{
	[InitializeOnLoad]
	class UpdateAllVolumes
	{
	    static UpdateAllVolumes()
	    {
	        EditorApplication.update += Update;
	    }
	 
	    static void Update ()
	    {
			if(!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				// In play mode the volume syncing is driven by a coroutine, but this doesn't run in edit mode. We could try
				// syncing when an action occurs (such as a terrain modification) but this doesn't give us background loading
				// of the terrain. We find the list of Volumes using the rather slow method below. We tried maintaining our
				// own global list of volumes but found this got cleared whenever scripts got recompiled (and Awake/Start are
				// apparently not called in this case).
				Object[] volumes = Object.FindObjectsOfType(typeof(Volume));
				foreach(Object volume in volumes)
				{
					((Volume)volume).Synchronize();
				}
			}
	    }
	}
}
