using UnityEngine;
using UnityEditor;
using System.Collections;
 
[CustomEditor (typeof(SmoothTerrainVolume))]
public class SmoothTerrainVolumeEditor : Editor
{
	SmoothTerrainVolume smoothTerrainVolume;
	
	private float brushSize = 5.0f;
	private float opacity = 1.0f;
	
	bool sculptPressed = true;
	bool smoothPressed = false;
	bool paintTexture = false;
	bool settingPressed = false;

	public void OnEnable()
	{
	    smoothTerrainVolume = target as SmoothTerrainVolume;
	}
	
	public override void OnInspectorGUI()
	{
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Toggle(sculptPressed, "Sculpt", EditorStyles.miniButtonLeft))
		{
			sculptPressed = true;
			smoothPressed = false;
			paintTexture = false;
			settingPressed = false;
		}
		if(GUILayout.Toggle(smoothPressed, "Smooth", EditorStyles.miniButtonMid))
		{
			sculptPressed = false;
			smoothPressed = true;
			paintTexture = false;
			settingPressed = false;
		}
		if(GUILayout.Toggle(paintTexture, "Paint", EditorStyles.miniButtonMid))
		{
			sculptPressed = false;
			smoothPressed = false;
			paintTexture = true;
			settingPressed = false;
		}
		if(GUILayout.Toggle(settingPressed, "Settings", EditorStyles.miniButtonRight))
		{
			sculptPressed = false;
			smoothPressed = false;
			paintTexture = false;
			settingPressed = true;
		}
		EditorGUILayout.EndHorizontal();
			
		if(sculptPressed)
		{
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Brush Size:", GUILayout.Width(80));
				brushSize = GUILayout.HorizontalSlider(brushSize, 0.0f, 10.0f);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Opacity:", GUILayout.Width(80));
				opacity = GUILayout.HorizontalSlider(opacity, -2.0f, 2.0f);
			EditorGUILayout.EndHorizontal();
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
			float resultX, resultY, resultZ;

			bool hit = Cubiquity.PickTerrainSurface(smoothTerrainVolume, ray.origin.x, ray.origin.y, ray.origin.z, dir.x, dir.y, dir.z, out resultX, out resultY, out resultZ);
			if(hit)
			{
				//smoothTerrainVolume.SetVoxel(resultX, resultY, resultZ, paintColor);
				Debug.Log ("Hit! - " + new Vector3(resultX, resultY, resultZ));
				Cubiquity.SculptSmoothTerrainVolume(smoothTerrainVolume, resultX, resultY, resultZ, brushSize, opacity);
			}
			
			//Selection.activeGameObject = coloredCubesVolume.gameObject;
		}
		else if ( e.type == EventType.Layout )
	    {
	       // See: http://answers.unity3d.com/questions/303248/how-to-paint-objects-in-the-editor.html
	       HandleUtility.AddDefaultControl( GUIUtility.GetControlID( GetHashCode(), FocusType.Passive ) );
	    }
		
		smoothTerrainVolume.Synchronize();
	}
}