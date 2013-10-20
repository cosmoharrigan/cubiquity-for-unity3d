using UnityEngine;
using System.Collections;
using System.IO;

public static class Cubiquity
{
	// This is the path to where the volumes are stored on disk.
	public static string volumesPath
	{
		get
		{
			string pathToData = System.IO.Path.Combine(Application.streamingAssetsPath, "Cubiquity/Volumes");
			return pathToData;
		}
	}
	
	
}
