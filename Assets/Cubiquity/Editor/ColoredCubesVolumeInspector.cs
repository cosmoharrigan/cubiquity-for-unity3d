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
		
		public static Tool lastTool = Tool.None;
		
		private static bool mAddMode = true;
		private static bool mDeleteMode = false;
		private static bool mPaintMode = false;
		
		private static bool addMode
		{
			get { return mAddMode; }
			set { if(mAddMode != value) { mAddMode = value; OnEditorToolChanged(); } }
		}
		
		private static bool deleteMode
		{
			get { return mDeleteMode; }
			set { if(mDeleteMode != value) { mDeleteMode = value; OnEditorToolChanged(); } }
		}
		
		private static bool paintMode
		{
			get { return mPaintMode; }
			set { if(mPaintMode != value) { mPaintMode = value; OnEditorToolChanged(); } }
		}
		
		Color paintColor = Color.white;
		
		GUIContent warningLabelContent;
		
		public void OnEnable()
		{
		    coloredCubesVolume = target as ColoredCubesVolume;
			
			Texture2D warnIcon = EditorGUIUtility.FindTexture("console.warnicon");
			warningLabelContent = new GUIContent("This version of Cubiquity is for \n" +
				"non-commercial and evaluation use\n" +
				"only. Please see LICENSE.txt for\n" +
				"further details.", warnIcon);
		}
		
		public override void OnInspectorGUI()
		{		
			// Check whether the selected Unity transform tool has changed.
			if(ColoredCubesVolumeInspector.lastTool != Tools.current)
			{
				OnTransformToolChanged();				
				ColoredCubesVolumeInspector.lastTool = Tools.current;
			}
			
			EditorGUILayout.LabelField("To modify the volume, please choose");
			EditorGUILayout.LabelField("a tool from the options below");
			
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Toggle(addMode, "Add cubes", EditorStyles.miniButtonLeft, GUILayout.Height(24)))
			{
				addMode = true;
				deleteMode = false;
				paintMode = false;
			}
			
			if(GUILayout.Toggle(deleteMode, "Delete cubes", EditorStyles.miniButtonMid, GUILayout.Height(24)))
			{
				addMode = false;
				deleteMode = true;
				paintMode = false;
			}
			
			if(GUILayout.Toggle(paintMode, "Paint cubes", EditorStyles.miniButtonRight, GUILayout.Height(24)))
			{
				addMode = false;
				deleteMode = false;
				paintMode = true;
			}
			EditorGUILayout.EndHorizontal();
			
			paintColor = EditorGUILayout.ColorField(paintColor, GUILayout.Width(200));
			
			if(GUILayout.Button("Load Voxel Database..."))
			{			
				string pathToVoxelDatabase = EditorUtility.OpenFilePanel("Choose a Voxel Database (.vdb) file to load", Paths.voxelDatabases, "vdb");
				
				string relativePathToVoxelDatabase = Paths.MakeRelativePath(Paths.voxelDatabases + Path.DirectorySeparatorChar, pathToVoxelDatabase);
				
				ColoredCubesVolumeData data = ColoredCubesVolumeData.CreateFromVoxelDatabase(relativePathToVoxelDatabase);
				
				coloredCubesVolume.data = data;
			}
			
			// Warn about unlicensed version.
			EditorGUILayout.LabelField(warningLabelContent, GUILayout.Height(64));
		}
		
		public void OnSceneGUI()
		{
			if(addMode || deleteMode || paintMode)
			{
				//Debug.Log ("ColoredCubesVolumeEditor.OnSceneGUI()");
				Event e = Event.current;
				
				Ray ray = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
				
				if(((e.type == EventType.MouseDown) || (e.type == EventType.MouseDrag)) && (e.button == 0))
				{
					// Perform the raycasting. If there's a hit the position will be stored in these ints.
					PickVoxelResult pickResult;
					if(addMode)
					{
						bool hit = Picking.PickLastEmptyVoxel(coloredCubesVolume, ray, 1000.0f, out pickResult);
						if(hit)
						{
							coloredCubesVolume.data.SetVoxel(pickResult.volumeSpacePos.x, pickResult.volumeSpacePos.y, pickResult.volumeSpacePos.z, (QuantizedColor)paintColor);
						}
					}
					else if(deleteMode)
					{					
						bool hit = Picking.PickFirstSolidVoxel(coloredCubesVolume, ray, 1000.0f, out pickResult);
						if(hit)
						{
							coloredCubesVolume.data.SetVoxel(pickResult.volumeSpacePos.x, pickResult.volumeSpacePos.y, pickResult.volumeSpacePos.z, new QuantizedColor(0,0,0,0));
						}
					}
					else if(paintMode)
					{
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
		}
		
		private static void OnEditorToolChanged()
		{
			// Whenever the user selects a terrain editing tool we need to make sure that Unity's transform widgets
			// are disabled. Otherwise the user can end up moving the terrain around while they are editing it.
			if(addMode || deleteMode || paintMode)
			{
				Tools.current = Tool.None;
			}
		}
		
		private static void OnTransformToolChanged()
		{
			// Deselect our editor tools if the user has selected a transform tool
			if(Tools.current != Tool.None)
			{
				mAddMode = false;
				mDeleteMode = false;
				mPaintMode = false;
			}
		}
	}
}
