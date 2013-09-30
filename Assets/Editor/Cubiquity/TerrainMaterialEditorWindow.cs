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
		EditorGUILayout.LabelField("Instructions", EditorStyles.boldLabel);
		EditorGUILayout.HelpBox("Please choose a texture to assign to this material slot. You can also adjust the scale and offset of your selected texture." , MessageType.None);
		EditorGUILayout.Space();
		
		EditorGUILayout.LabelField("Texture", EditorStyles.boldLabel);
		Texture2D oldTexture = material.diffuseMap;
		Texture2D newTexture = EditorGUILayout.ObjectField(oldTexture,typeof(Texture),false, GUILayout.Width(80), GUILayout.Height(80)) as Texture2D;
		if(oldTexture != newTexture)
		{
			material.diffuseMap = newTexture;
			HandleUtility.Repaint();
		}
	}
}
