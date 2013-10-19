using UnityEngine;
using System.Collections;

[System.Serializable]
public sealed class TerrainVolumeData
{
	public string pathToVoxels;
	
	// The extents (dimensions in voxels) of the volume.
	public Region region = null;
	
	public TerrainMaterial[] materials = new TerrainMaterial[License.MaxNoOfMaterials];
}
