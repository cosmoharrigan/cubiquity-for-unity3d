using UnityEditor;
using UnityEngine;
using System.Collections;

public class TerrainMaterialEditorWindow : EditorWindow
{
	private TerrainMaterial material;
		
	public static void EditMaterial(TerrainMaterial materialToEdit)
	{
		TerrainMaterialEditorWindow window = ScriptableObject.CreateInstance<TerrainMaterialEditorWindow>();
		window.material = materialToEdit;
		window.ShowUtility();
	}
	
	void OnGUI()
	{
		material.diffuseMap = EditorGUILayout.ObjectField(material.diffuseMap,typeof(Texture),false, GUILayout.Width(80), GUILayout.Height(80)) as Texture2D;
	}
}
