using UnityEngine;
using UnityEditor;

using System.Collections;

namespace Cubiquity
{	
	[CustomEditor (typeof(ColoredCubesVolumeData))]
	public class ColoredCubesVolumeDataInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			ColoredCubesVolumeData data = target as ColoredCubesVolumeData;
			
			EditorGUILayout.LabelField("Full path to voxel database:", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox(data.fullPathToVoxelDatabase, MessageType.None);
		}
	}
}