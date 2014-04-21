using UnityEngine;
using UnityEditor;

using System.Collections;

namespace Cubiquity
{	
	[CustomEditor (typeof(ColoredCubesVolumeRenderer))]
	public class ColoredCubesVolumeRendererInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			ColoredCubesVolumeRenderer renderer = target as ColoredCubesVolumeRenderer;
			
			float labelWidth = 120.0f;
			
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Receive Shadows:", EditorStyles.boldLabel, GUILayout.Width(labelWidth));
				renderer.receiveShadows = EditorGUILayout.Toggle(renderer.receiveShadows);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Cast Shadows:", EditorStyles.boldLabel, GUILayout.Width(labelWidth));
				renderer.castShadows = EditorGUILayout.Toggle(renderer.castShadows);
			EditorGUILayout.EndHorizontal();
		}
	}
}