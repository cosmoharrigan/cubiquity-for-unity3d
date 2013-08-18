using UnityEditor;
using UnityEngine;

using System;
using System.Collections;
using System.IO;

public class CreateColoredCubesVolumeFromImagesWizard : ScriptableWizard
{
	private string datasetName = "New Volume";
	
	private string imageFolder = "";
	
	[MenuItem ("GameObject/Create Other/Colored Cubes Volume/Create Colored Cubes Volume From Images...")]
    static void CreateWizard ()
	{
        ScriptableWizard.DisplayWizard<CreateColoredCubesVolumeFromImagesWizard>("Create Colored Cubes Volume From Images");
    }
	
	void OnGUI()
	{
		GUIStyle labelWrappingStyle = new GUIStyle(GUI.skin.label);
		labelWrappingStyle.wordWrap = true;
		
		GUIStyle rightAlignmentStyle = new GUIStyle(GUI.skin.textField);
		rightAlignmentStyle.alignment = TextAnchor.MiddleRight;
		
		EditorGUILayout.BeginHorizontal();
			GUILayout.Space(20);
			EditorGUILayout.LabelField("Cubiquity volumes are not Unity3D assets and they do not belong in the 'Assets' folder. " +
				"Please choose or create an empty folder inside the 'Volumes' folder.", labelWrappingStyle);
			GUILayout.Space(20);
		EditorGUILayout.EndHorizontal();
		
		GUILayout.Space(10);
		
		EditorGUILayout.BeginHorizontal();	
			GUILayout.Space(50);
			EditorGUILayout.LabelField("Folder name:", GUILayout.Width(80));
			EditorGUILayout.TextField("", datasetName);
			if(GUILayout.Button("Select folder...", GUILayout.Width(120)))
			{
				string selectedFolderAsString = EditorUtility.SaveFolderPanel("Create or choose and empty folder for the volume data", Cubiquity.pathToData, "");
				
				DirectoryInfo assetDirInfo = new DirectoryInfo(Application.dataPath);
				DirectoryInfo executableDirInfo = assetDirInfo.Parent;
				DirectoryInfo volumeDirInfo = new DirectoryInfo(executableDirInfo.FullName + Path.DirectorySeparatorChar + Cubiquity.pathToData);
			
				Uri volumeUri = new Uri(volumeDirInfo.FullName + Path.DirectorySeparatorChar);
				Uri selectedUri = new Uri(selectedFolderAsString);
				Uri relativeUri = volumeUri.MakeRelativeUri(selectedUri);
			
				datasetName = relativeUri.ToString();
			}
			GUILayout.Space(20);
		EditorGUILayout.EndHorizontal();
		
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
		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Create volume", GUILayout.Width(128)))
			{
				OnCreatePressed ();
			}
			GUILayout.Space(50);
			if(GUILayout.Button("Cancel", GUILayout.Width(128)))
			{
				OnCancelPressed ();
			}
			EditorGUILayout.Space();
		EditorGUILayout.EndHorizontal();
	}
	
	void OnCreatePressed()
	{
		Close();		
		Debug.Log("Creating volume");
		
		if(GameObject.Find("Voxel Terrain") != null)
		{
			Debug.LogError("A voxel terrain already exists - you (currently) can't create another one.");
		}
		
		//GameObject voxelGameObject = ColoredCubesVolumeFactory.CreateVolume("Voxel Terrain", new Region(0, 0, 0, width-1, height-1, depth-1), datasetName);
		GameObject voxelGameObject = ColoredCubesVolumeFactory.CreateVolumeFromVolDat("Voxel Terrain", imageFolder, datasetName);
	}
	
	void OnCancelPressed()
	{
		Debug.Log("Cancelling");
		Close ();
	}
}
