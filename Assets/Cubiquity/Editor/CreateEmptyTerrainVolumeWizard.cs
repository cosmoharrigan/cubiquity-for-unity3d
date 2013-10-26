using UnityEditor;
using UnityEngine;

using System;
using System.Collections;
using System.IO;

namespace Cubiquity
{
	public class CreateEmptyTerrainVolumeWizard : CreateVolumeWizard
	{	
		private bool createFloor = true;
		
		private int floorThickness = 8;
		
		//[MenuItem ("GameObject/Create Other/Terrain Volume (Old)/Create Empty Terrain Volume...")]
	    static void CreateWizard ()
		{
			// Make sure the Cubiquity library is installed.
			Installation.ValidateAndFix();
			
	        CreateEmptyTerrainVolumeWizard wizard = ScriptableWizard.DisplayWizard<CreateEmptyTerrainVolumeWizard>("Create Empty Terrain Volume");
			wizard.position = new Rect(100, 100, 600, 300);
	    }
		
		void OnGUI()
		{
			GUIStyle labelWrappingStyle = new GUIStyle(GUI.skin.label);
			labelWrappingStyle.wordWrap = true;
			
			GUIStyle rightAlignmentStyle = new GUIStyle(GUI.skin.textField);
			rightAlignmentStyle.alignment = TextAnchor.MiddleRight;
			
			OnGuiHeader(true); //Pass true because we let the user specify the size.
			
			GUILayout.Space(10);
			
			EditorGUILayout.BeginHorizontal();
				GUILayout.Space(20);
				EditorGUILayout.LabelField("You can set the first few layers of your new volume to be filled with a solid color. " +
					"If you do not do this then you will not see the volume until you add some voxels through code.", labelWrappingStyle);
			EditorGUILayout.EndHorizontal();
			
			GUILayout.Space(10);
			
			EditorGUILayout.BeginHorizontal();
				GUILayout.Space(50);
				EditorGUILayout.LabelField("Create floor", GUILayout.Width(80));
				createFloor = EditorGUILayout.Toggle("", createFloor, GUILayout.Width(20));
				GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
				GUILayout.Space(80);
				EditorGUILayout.LabelField("Floor thickness:", GUILayout.Width(100));
				floorThickness = EditorGUILayout.IntField("", floorThickness, GUILayout.Width(40));
				GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			
			GUILayout.Space(20); // A space before the create/cancel buttons
			
			OnGuiFooter();
		}
		
		public override void OnCreatePressed()
		{
			Close();
			
			if(createFloor)
			{
				TerrainVolumeFactory.CreateVolumeWithFloor("Voxel Terrain", new Region(0, 0, 0, width-1, height-1, depth-1), (uint)floorThickness);
			}
			else
			{
				TerrainVolumeFactory.CreateVolume("Voxel Terrain", new Region(0, 0, 0, width-1, height-1, depth-1));
			}
	
		}
	}
}
