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
			// Although the volume can be set to execute in edit mode, the update function is then only called when an
			// event such as a mouse movement occurs. But for progressive loading of the volume we want continuous events.
			foreach(Volume volume in Volume.all)
			{
				volume.Synchronize();
			}
	    }
	}
}
