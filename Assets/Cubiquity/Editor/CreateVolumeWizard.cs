using UnityEditor;
using UnityEngine;

using System;
using System.Collections;
using System.IO;

abstract public class CreateVolumeWizard : ScriptableWizard
{
	// This is the path to where the volumes are stored on disk.
	public static string volumesPath
	{
		get
		{
			string pathToData = System.IO.Path.Combine(Application.streamingAssetsPath, "Cubiquity/Volumes");
			return pathToData;
		}
	}
	
	protected string datasetName = "New Volume";
	
	protected int width = 128;
	protected int height = 32;
	protected int depth = 128;
	private int maximumVolumeSize = 256; // FIXME - Should get this from the library.
	
	protected void OnGuiHeader(bool drawSizeSelector)
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
			EditorGUILayout.LabelField("Folder name: StreamingAssets/Cubiquity/Volumes/", GUILayout.Width(298));
			EditorGUILayout.TextField("", datasetName, GUILayout.MinWidth(100));
			if(GUILayout.Button("Select folder...", GUILayout.Width(100)))
			{
				string selectedFolderAsString = EditorUtility.SaveFolderPanel("Create or choose and empty folder for the volume data", volumesPath, "");
			
				if(IsSubfolder(volumesPath, selectedFolderAsString))
				{
					Uri selectedFolderUri = new Uri(selectedFolderAsString);
					Uri volumeDataUri = new Uri(volumesPath + Path.DirectorySeparatorChar);			
					Uri relativeUri = volumeDataUri.MakeRelativeUri(selectedFolderUri);			
					datasetName = relativeUri.ToString();
				}
				else
				{
					EditorUtility.DisplayDialog("Invalid folder", "The folder you create must be inside 'StreamingAssets/Cubiquity/Volumes/'", "Ok");
				}
			}
			GUILayout.Space(20);
		EditorGUILayout.EndHorizontal();
		
		GUILayout.Space(10);
		
		if(drawSizeSelector)
		{					
			EditorGUILayout.BeginHorizontal();
				GUILayout.Space(20);
				EditorGUILayout.LabelField("Set the volume dimensions below. Please note that the values cannot exceed 256 in any dimension.", labelWrappingStyle);
				GUILayout.Space(20);
			EditorGUILayout.EndHorizontal();
			
			GUILayout.Space(10);
			
			EditorGUILayout.BeginHorizontal();
				GUILayout.Space(50);
				EditorGUILayout.LabelField("Width:", GUILayout.Width(50));
				width = Math.Min(EditorGUILayout.IntField("", width, GUILayout.Width(40)), maximumVolumeSize);
				GUILayout.Space(20);
				EditorGUILayout.LabelField("Height:", GUILayout.Width(50));
				height = Math.Min(EditorGUILayout.IntField("", height, GUILayout.Width(40)), maximumVolumeSize);
				GUILayout.Space(20);
				EditorGUILayout.LabelField("Depth:", GUILayout.Width(50));
				depth = Math.Min(EditorGUILayout.IntField("", depth, GUILayout.Width(40)), maximumVolumeSize);
				GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
	}
	
	protected void OnGuiFooter()
	{
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
	
	abstract public void OnCreatePressed();
	
	void OnCancelPressed()
	{
		Close ();
	}
	
	// Based on http://stackoverflow.com/a/7710620
	private bool IsSubfolder(string parentPath, string childPath)
    {
        var parentUri = new Uri( parentPath ) ;

        var childUri = new DirectoryInfo( childPath ).Parent ;

        while( childUri != null )
        {
            if( new Uri( childUri.FullName ) == parentUri )
            {
                return true ;
            }

            childUri = childUri.Parent ;
        }

        return false ;
    }
}
