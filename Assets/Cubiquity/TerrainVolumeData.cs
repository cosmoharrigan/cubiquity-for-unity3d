using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	[System.Serializable]
	public sealed class TerrainVolumeData
	{
		public TerrainVolumeData()
		{
			materials = new TerrainMaterial[License.MaxNoOfMaterials];
			
			for(int i = 0; i < materials.Length; i++)
			{
				materials[i] = new TerrainMaterial();
			}
		}
		
		public string pathToVoxels;
		
		// The extents (dimensions in voxels) of the volume.
		public Region region = null;
		
		public TerrainMaterial[] materials;
	}
}