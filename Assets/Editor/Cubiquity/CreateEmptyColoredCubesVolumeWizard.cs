using UnityEditor;
using UnityEngine;

using System;
using System.Collections;
using System.IO;

public class CreateEmptyColoredCubesVolumeWizard : ScriptableWizard
{
	private string datasetName = "New Volume";
	
	private int width = 256;
	private int height = 64;
	private int depth = 256;
	
	private bool createFloor = true;
	
	private int floorThickness = 8;
	
	[MenuItem ("GameObject/Create Other/Colored Cubes Volume/Create Empty Colored Cubes Volume...")]
    static void CreateWizard ()
	{
		// Make sure the Cubiquity library is installed.
		Installation.ValidateAndFix();
		
        ScriptableWizard.DisplayWizard<CreateEmptyColoredCubesVolumeWizard>("Create Empty Colored Cubes Volume");
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
		
		GUILayout.Space(10);
		
		EditorGUILayout.BeginHorizontal();
			GUILayout.Space(20);
			EditorGUILayout.LabelField("Set the volume dimensions below. Please note that the values cannot exceed 256 in any dimension.", labelWrappingStyle);
			GUILayout.Space(20);
		EditorGUILayout.EndHorizontal();
		
		GUILayout.Space(10);
		
		EditorGUILayout.BeginHorizontal();	
			GUILayout.Space(50);
			EditorGUILayout.LabelField("Width:", GUILayout.Width(50));
			width = EditorGUILayout.IntField("", width, GUILayout.Width(40));
			GUILayout.Space(20);
			EditorGUILayout.LabelField("Height:", GUILayout.Width(50));
			height = EditorGUILayout.IntField("", height, GUILayout.Width(40));
			GUILayout.Space(20);
			EditorGUILayout.LabelField("Depth:", GUILayout.Width(50));
			depth = EditorGUILayout.IntField("", depth, GUILayout.Width(40));
			GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
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
		
		GameObject voxelGameObject = ColoredCubesVolumeFactory.CreateVolume("Voxel Terrain", new Region(0, 0, 0, width-1, height-1, depth-1), datasetName);
		ColoredCubesVolume coloredCubesVolume = voxelGameObject.GetComponent<ColoredCubesVolume>();
		
		// Call Initialize so we can start drawing into the volume right away.
		//coloredCubesVolume.Initialize();
		if(createFloor)
		{
			Color32 floorColor = new Color32(192, 192, 192, 255);
			
			for(int z = 0; z <= depth-1; z++)
			{
				for(int y = 0; y < floorThickness; y++)
				{
					for(int x = 0; x <= width-1; x++)
					{
						coloredCubesVolume.SetVoxel(x, y, z, floorColor);
					}
				}
			}
		}
	}
	
	void OnCancelPressed()
	{
		Debug.Log("Cancelling");
		Close ();
	}
}
