using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
 
[CustomEditor (typeof(SmoothTerrainVolume))]
public class SmoothTerrainVolumeEditor : Editor
{
	SmoothTerrainVolume smoothTerrainVolume;
	
	private const int NoOfBrushes = 5;
	
	private float brushOuterRadius = 5.0f;
	private float opacity = 1.0f;
	
	bool sculptPressed = true;
	bool smoothPressed = false;
	bool paintPressed = false;
	bool settingPressed = false;
	
	int selectedBrush = 0;
	int selectedTexture = 0;
	
	Texture[] brushTextures;

	public void OnEnable()
	{
	    smoothTerrainVolume = target as SmoothTerrainVolume;
		
		brushTextures = new Texture[NoOfBrushes];
		brushTextures[0] = Resources.Load("Icons/SoftBrush") as Texture;
		brushTextures[1] = Resources.Load("Icons/MediumSoftBrush") as Texture;
		brushTextures[2] = Resources.Load("Icons/MediumBrush") as Texture;
		brushTextures[3] = Resources.Load("Icons/MediumHardBrush") as Texture;
		brushTextures[4] = Resources.Load("Icons/HardBrush") as Texture;
	}
	
	public override void OnInspectorGUI()
	{
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Toggle(sculptPressed, "Sculpt", EditorStyles.miniButtonLeft))
		{
			sculptPressed = true;
			smoothPressed = false;
			paintPressed = false;
			settingPressed = false;
		}
		if(GUILayout.Toggle(smoothPressed, "Smooth", EditorStyles.miniButtonMid))
		{
			sculptPressed = false;
			smoothPressed = true;
			paintPressed = false;
			settingPressed = false;
		}
		if(GUILayout.Toggle(paintPressed, "Paint", EditorStyles.miniButtonMid))
		{
			sculptPressed = false;
			smoothPressed = false;
			paintPressed = true;
			settingPressed = false;
		}
		if(GUILayout.Toggle(settingPressed, "Settings", EditorStyles.miniButtonRight))
		{
			sculptPressed = false;
			smoothPressed = false;
			paintPressed = false;
			settingPressed = true;
		}
		EditorGUILayout.EndHorizontal();
			
		if(sculptPressed)
		{
			DrawSculptControls();
		}
		
		if(smoothPressed)
		{
			DrawSmoothControls();
		}
		
		if(paintPressed)
		{
			DrawPaintControls();
		}
	}
	
	private void DrawSculptControls()
	{
		selectedBrush = DrawTextureSelectionGrid(selectedBrush, brushTextures, 5, 50);
		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Brush Radius:", GUILayout.Width(120));
			brushOuterRadius = GUILayout.HorizontalSlider(brushOuterRadius, 0.0f, 10.0f);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Opacity:", GUILayout.Width(80));
			opacity = GUILayout.HorizontalSlider(opacity, 0.0f, 1.0f);
		EditorGUILayout.EndHorizontal();
	}
	
	private void DrawSmoothControls()
	{
		selectedBrush = DrawTextureSelectionGrid(selectedBrush, brushTextures, 5, 50);
		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Brush Radius:", GUILayout.Width(120));
			brushOuterRadius = GUILayout.HorizontalSlider(brushOuterRadius, 0.0f, 10.0f);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Opacity:", GUILayout.Width(80));
			opacity = GUILayout.HorizontalSlider(opacity, 0.0f, 2.0f);
		EditorGUILayout.EndHorizontal();
	}
	
	private void DrawPaintControls()
	{
		selectedBrush = DrawTextureSelectionGrid(selectedBrush, brushTextures, 5, 50);
		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Brush Radius:", GUILayout.Width(120));
			brushOuterRadius = GUILayout.HorizontalSlider(brushOuterRadius, 0.0f, 10.0f);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Opacity:", GUILayout.Width(80));
			opacity = GUILayout.HorizontalSlider(opacity, 0.0f, 1.0f);
		EditorGUILayout.EndHorizontal();
		
		selectedTexture = DrawTextureSelectionGrid(selectedTexture, smoothTerrainVolume.diffuseMaps, 3, 80);
		
		for(int ct = 0; ct < License.MaxNoOfMaterials; ct++)
		{
			smoothTerrainVolume.diffuseMaps[ct] = EditorGUILayout.ObjectField(smoothTerrainVolume.diffuseMaps[ct],typeof(Texture),false, GUILayout.Width(80), GUILayout.Height(80)) as Texture2D;
		}
	}
	
	private void DrawSettingsControls()
	{
	}
	
	private int DrawTextureSelectionGrid(int selected, Texture[] images, int xCount, int thumbnailSize)
	{
		// Don't think the selection grid handles wrapping automatically, so we compute it ourselves.
		int imageThumbnailSize = thumbnailSize;
		int inspectorWidth = Screen.width;			
		int widthInThumbnails = inspectorWidth / imageThumbnailSize;
		int noOfThumbbails = (int)License.MaxNoOfMaterials;
		int noOfRows = noOfThumbbails / widthInThumbnails;
		if(noOfThumbbails % widthInThumbnails != 0)
		{
			noOfRows++;
		}
		
		//No draw the texture selection grid
		return GUILayout.SelectionGrid (selected, images, widthInThumbnails, GUILayout.Height(imageThumbnailSize * noOfRows));
	}
	
	public void OnSceneGUI()
	{
		//Debug.Log ("ColoredCubesVolumeEditor.OnSceneGUI()");
		Event e = Event.current;
		
		Ray ray = Camera.current.ScreenPointToRay(new Vector3(e.mousePosition.x, -e.mousePosition.y + Camera.current.pixelHeight));
		Vector3 dir = ray.direction * 1000.0f; //The maximum distance our ray will be cast.
		
		// Perform the raycasting. If there's a hit the position will be stored in these ints.
		float resultX, resultY, resultZ;
		bool hit = Cubiquity.PickTerrainSurface(smoothTerrainVolume, ray.origin.x, ray.origin.y, ray.origin.z, dir.x, dir.y, dir.z, out resultX, out resultY, out resultZ);
		
		if(hit)
		{		
			//Debug.Log("Hit");
			// Selected brush is in the range 0 to NoOfBrushes - 1. Convert this to a 0 to 1 range.
			float brushInnerScaleFactor = (float)selectedBrush / ((float)(NoOfBrushes - 1));
			// Use this value to compute the inner radius as a proportion of the outer radius.
			float brushInnerRadius = brushOuterRadius * brushInnerScaleFactor;
				
			List<string> keywords = new List<string>();
			keywords.Add("SHOW_BRUSH");
			smoothTerrainVolume.material.shaderKeywords = keywords.ToArray();
			//Shader.DisableKeyword("SHOW_BRUSH");
			//Shader.EnableKeyword("HIDE_BRUSH");
			smoothTerrainVolume.material.SetVector("_BrushCenter", new Vector4(resultX, resultY, resultZ, 0.0f));				
			smoothTerrainVolume.material.SetVector("_BrushSettings", new Vector4(brushInnerRadius, brushOuterRadius, opacity, 0.0f));
			
			if(((e.type == EventType.MouseDown) || (e.type == EventType.MouseDrag)) && (e.button == 0))
			{
				if(sculptPressed)
				{
					Cubiquity.SculptSmoothTerrainVolume(smoothTerrainVolume, resultX, resultY, resultZ, brushInnerRadius, brushOuterRadius, opacity);
				}
				else if(smoothPressed)
				{
					Cubiquity.BlurSmoothTerrainVolume(smoothTerrainVolume, resultX, resultY, resultZ, brushInnerRadius, brushOuterRadius, opacity);
				}
				else if(paintPressed)
				{
					Cubiquity.PaintSmoothTerrainVolume(smoothTerrainVolume, resultX, resultY, resultZ, brushInnerRadius, brushOuterRadius, opacity, (uint)selectedTexture);
				}
			}	
		}
		
		if ( e.type == EventType.Layout )
	    {
	       // See: http://answers.unity3d.com/questions/303248/how-to-paint-objects-in-the-editor.html
	       HandleUtility.AddDefaultControl( GUIUtility.GetControlID( GetHashCode(), FocusType.Passive ) );
	    }
		
		smoothTerrainVolume.Synchronize();
		
		//Repaint ();
		HandleUtility.Repaint();
	}
}