using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
class UpdateAllVolumes
{
    static UpdateAllVolumes()
    {
        EditorApplication.update += Update;
    }
 
    static void Update ()
    {
		// According to the docs this is very slow, but I don't know a better way. The code below finds all volumes
		// and calls their syncronize() function to update the geometry. Althoughthe volume can be set to execute in
		// edit mode, the update function is then only called when an event such as a mouse movement occurs. But for
		// progressive loading of the volume we want continuous events.
		Object[] volumes = Object.FindObjectsOfType(typeof(ColoredCubesVolume));
		foreach(Object volume in volumes)
		{
			ColoredCubesVolume coloredCubesVolume = volume as ColoredCubesVolume;
			coloredCubesVolume.Synchronize();
		}
		
		Object[] smoothVolumes = Object.FindObjectsOfType(typeof(TerrainVolume));
		foreach(Object volume in smoothVolumes)
		{
			TerrainVolume terrainVolume = volume as TerrainVolume;
			terrainVolume.Synchronize();
		}
    }
}