using UnityEngine;
using UnityEditor;

using System.Collections;

namespace Cubiquity
{	
	[CustomEditor (typeof(TerrainVolumeRenderer))]
	public class TerrainVolumeRendererInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			TerrainVolumeRenderer renderer = target as TerrainVolumeRenderer;
			
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Receive Shadows:", EditorStyles.boldLabel);
				renderer.receiveShadows = EditorGUILayout.Toggle(renderer.receiveShadows);
			EditorGUILayout.EndHorizontal();
		}
	}
}