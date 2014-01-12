using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
 
namespace Cubiquity
{
	[CustomEditor (typeof(ColoredCubesVolume))]
	public class ColoredCubesVolumeInspector : Editor
	{
		ColoredCubesVolume coloredCubesVolume;
		
		private bool addMode = true;
		private bool deleteMode = false;
		private bool paintMode = false;
		
		Color paintColor = Color.white;
		
		public void OnEnable()
		{
		    coloredCubesVolume = target as ColoredCubesVolume;
		}
		
		public override void OnInspectorGUI()
		{		
			if(EditorGUILayout.Toggle("Add cubes", addMode))
			{
				addMode = true;
				deleteMode = false;
				paintMode = false;
			}
			
			if(EditorGUILayout.Toggle("Delete cubes", deleteMode))
			{
				addMode = false;
				deleteMode = true;
				paintMode = false;
			}
			
			if(EditorGUILayout.Toggle("Paint cubes", paintMode))
			{
				addMode = false;
				deleteMode = false;
				paintMode = true;
			}
			
			paintColor = EditorGUILayout.ColorField(paintColor, GUILayout.Width(200));
			
			if(GUILayout.Button("Load Voxel Database..."))
			{			
				string pathToVoxelDatabase = EditorUtility.OpenFilePanel("Choose a Voxel Database (.vdb) file to load", Application.streamingAssetsPath, "vdb");
				
				/*Uri uriToVoxelDatabase = new Uri(pathToVoxelDatabase);	
				Uri uriToStreamingAssets = new Uri(Application.streamingAssetsPath + Path.PathSeparator);			
				Uri relativeUri = uriToStreamingAssets.MakeRelativeUri(uriToVoxelDatabase);			
				string relativePathToVoxelDatabase = relativeUri.ToString();*/
				
				string relativePathToVoxelDatabase = MakeRelativePath(Application.streamingAssetsPath + Path.DirectorySeparatorChar, pathToVoxelDatabase);
				
				ColoredCubesVolumeData data = ColoredCubesVolumeData.CreateFromVoxelDatabase(VolumeData.Paths.StreamingAssets, relativePathToVoxelDatabase);
				
				coloredCubesVolume.data = data;
				
				/*string pathToVoxelDatabase = "C:/Code/cubiquity-for-unity3d/Assets/StreamingAssets/0D2705DA.vdb";
				string streamingAssetsPath = "C:/Code/cubiquity-for-unity3d/Assets/StreamingAssets" + Path.DirectorySeparatorChar;
				
				string relativePath = MakeRelativePath(streamingAssetsPath, pathToVoxelDatabase);
				Debug.Log(relativePath);*/
			}
		}
		
		public void OnSceneGUI()
		{
			//Debug.Log ("ColoredCubesVolumeEditor.OnSceneGUI()");
			Event e = Event.current;
			
			Ray ray = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
			Vector3 dir = ray.direction * 1000.0f; //The maximum distance out ray will be cast.
			
			if(((e.type == EventType.MouseDown) || (e.type == EventType.MouseDrag)) && (e.button == 0))
			{
				// Perform the raycasting. If there's a hit the position will be stored in these ints.
				int resultX, resultY, resultZ;
				if(addMode)
				{
					bool hit = Picking.PickLastEmptyVoxel(coloredCubesVolume, ray.origin.x, ray.origin.y, ray.origin.z, dir.x, dir.y, dir.z, out resultX, out resultY, out resultZ);
					if(hit)
					{
						coloredCubesVolume.data.SetVoxel(resultX, resultY, resultZ, (QuantizedColor)paintColor);
					}
				}
				else if(deleteMode)
				{
					PickVoxelResult pickResult;
					bool hit = Picking.PickFirstSolidVoxel(coloredCubesVolume, ray, 1000.0f, out pickResult);
					if(hit)
					{
						coloredCubesVolume.data.SetVoxel(pickResult.volumeSpacePos.x, pickResult.volumeSpacePos.y, pickResult.volumeSpacePos.z, new QuantizedColor(0,0,0,0));
					}
				}
				else if(paintMode)
				{
					PickVoxelResult pickResult;
					bool hit = Picking.PickFirstSolidVoxel(coloredCubesVolume, ray, 1000.0f, out pickResult);
					if(hit)
					{
						coloredCubesVolume.data.SetVoxel(pickResult.volumeSpacePos.x, pickResult.volumeSpacePos.y, pickResult.volumeSpacePos.z, (QuantizedColor)paintColor);
					}
				}
				
				Selection.activeGameObject = coloredCubesVolume.gameObject;
			}
			else if ( e.type == EventType.Layout )
		    {
		       // See: http://answers.unity3d.com/questions/303248/how-to-paint-objects-in-the-editor.html
		       HandleUtility.AddDefaultControl( GUIUtility.GetControlID( GetHashCode(), FocusType.Passive ) );
		    }
		}
		
		public static String MakeRelativePath(String fromPath, String toPath)
	    {
	        if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
	        if (String.IsNullOrEmpty(toPath))   throw new ArgumentNullException("toPath");
	
	        Uri fromUri = new Uri(fromPath);
	        Uri toUri = new Uri(toPath);
	
	        if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.
	
	        Uri relativeUri = fromUri.MakeRelativeUri(toUri);
	        String relativePath = Uri.UnescapeDataString(relativeUri.ToString());
	
	        if (toUri.Scheme.ToUpperInvariant() == "FILE")
	        {
	            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
	        }
	
	        return relativePath;
	    }
	}
}
