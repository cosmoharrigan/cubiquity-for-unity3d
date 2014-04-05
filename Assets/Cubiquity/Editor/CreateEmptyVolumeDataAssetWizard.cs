using UnityEngine;
using UnityEditor;

using System.Collections;

namespace Cubiquity
{
	public class CreateEmptyVolumeDataAssetWizard : ScriptableWizard
	{
		public int width = 128;
		public int height = 32;
		public int depth = 128;
		
		void OnWizardCreate()
		{
	        TerrainVolumeDataAsset.CreateEmptyVolumeData(new Region(0, 0, 0, width-1, height-1, depth-1));
	    }  
	}
}