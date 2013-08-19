using UnityEditor;
using UnityEngine;

using System;
using System.Collections;
using System.IO;

public class CreateProceduralColoredCubesVolumeWizard : ScriptableWizard
{
	private string datasetName = "New Volume";
	
	private int width = 256;
	private int height = 64;
	private int depth = 256;
	
	[MenuItem ("GameObject/Create Other/Colored Cubes Volume/Create Colored Cubes Volume Procedurally...")]
    static void CreateWizard ()
	{
        ScriptableWizard.DisplayWizard<CreateProceduralColoredCubesVolumeWizard>("Create Colored Cubes Volume Procedurally");
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

		Color32 blue = new Color32(0, 0, 255, 255);
		Color32 grey = new Color32(128, 128, 128, 255);
		Color32 white = new Color32(255, 255, 255, 255);
		
		
		for(int z = 0; z <= depth-1; z++)
		{
			for(int x = 0; x <= width-1; x++)
			{
				float scale = 0.03f;
				float strength = 1.0f;
				int noOfOctaves = 3;
				float perlinValue = 0.0f;
				float normalizationFactor = 0.0f;
				for(int octave = 0; octave < noOfOctaves; octave++)
				{
					perlinValue += Mathf.PerlinNoise(x * scale, z * scale) * strength;
					normalizationFactor += strength;
					
					scale *= 2.0f;
					strength *= 0.5f;
				}
				
				perlinValue /= normalizationFactor;				
				
				int terrainHeight = (int)(perlinValue * height);
				
				int seaLevel = (int)(height * 0.4f);
				int snowLevel = (int)(height * 0.6f);
				
				/*if(terrainHeight < minTerrainHeight)
				{
					terrainHeight = minTerrainHeight;
				}*/
				
				for(int y = 0; y <= height-1; y++)
				{
					if(y < terrainHeight)
					{
						if(y < snowLevel)
						{
							coloredCubesVolume.SetVoxel(x, y, z, grey);
						}
						else
						{
							coloredCubesVolume.SetVoxel(x, y, z, white);
						}
					}
					else if(y < seaLevel)
					{
						coloredCubesVolume.SetVoxel(x, y, z, blue);
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
