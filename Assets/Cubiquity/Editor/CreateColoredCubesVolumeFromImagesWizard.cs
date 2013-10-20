using UnityEditor;
using UnityEngine;

using System;
using System.Collections;
using System.IO;

namespace Cubiquity
{
	public class CreateColoredCubesVolumeFromImagesWizard : CreateVolumeWizard
	{		
		private string imageFolder = "";
		
		//[MenuItem ("GameObject/Create Other/Colored Cubes Volume (Old)/Create Colored Cubes Volume From Images...")]
	    static void CreateWizard ()
		{
			// Make sure the Cubiquity library is installed.
			Installation.ValidateAndFix();
			
	        ScriptableWizard.DisplayWizard<CreateColoredCubesVolumeFromImagesWizard>("Create Colored Cubes Volume From Images");
	    }
		
		void OnGUI()
		{
			OnGuiHeader(false); //Pass false because volume size is computed from the images.
			
			GUIStyle labelWrappingStyle = new GUIStyle(GUI.skin.label);
			labelWrappingStyle.wordWrap = true;
			
			GUIStyle rightAlignmentStyle = new GUIStyle(GUI.skin.textField);
			rightAlignmentStyle.alignment = TextAnchor.MiddleRight;
			
			EditorGUILayout.BeginHorizontal();
				GUILayout.Space(20);
				EditorGUILayout.LabelField("Please choose a folder containing the images you wish to import. Images should all " + 
					"be the same size, should be numbered sequentially from '0', and should be in .png (recommended), .jpg, or .bmp format.", labelWrappingStyle);
				GUILayout.Space(20);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();	
				GUILayout.Space(50);
				EditorGUILayout.LabelField("Image folder:", GUILayout.Width(80));
				EditorGUILayout.TextField("", imageFolder);
				if(GUILayout.Button("Select folder...", GUILayout.Width(120)))
				{
					imageFolder = EditorUtility.SaveFolderPanel("Please choose a folder containing the images you wish to import.", "", "");
				}
				GUILayout.Space(20);
			EditorGUILayout.EndHorizontal();	
			
			GUILayout.Space(20); // A space before the create/cancel buttons
			
			OnGuiFooter();
		}
		
		public override void OnCreatePressed()
		{
			Close();		
			ColoredCubesVolumeFactory.CreateVolumeFromVolDat("Voxel Terrain", imageFolder, datasetName);
		}
	}
}