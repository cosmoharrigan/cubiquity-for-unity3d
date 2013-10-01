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
		if(GUILayout.Toggle(sculptPressed, "Sculpt", EditorStyles.miniButtonLeft, GUILayout.Height(24)))
		{
			sculptPressed = true;
			smoothPressed = false;
			paintPressed = false;
			settingPressed = false;
		}
		if(GUILayout.Toggle(smoothPressed, "Smooth", EditorStyles.miniButtonMid, GUILayout.Height(24)))
		{
			sculptPressed = false;
			smoothPressed = true;
			paintPressed = false;
			settingPressed = false;
		}
		if(GUILayout.Toggle(paintPressed, "Paint", EditorStyles.miniButtonMid, GUILayout.Height(24)))
		{
			sculptPressed = false;
			smoothPressed = false;
			paintPressed = true;
			settingPressed = false;
		}
		if(GUILayout.Toggle(settingPressed, "Settings", EditorStyles.miniButtonRight, GUILayout.Height(24)))
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
		DrawInstructions("Click on the terrain to pull the surface out. Hold down shift while clicking to push in instead.");
			
		DrawBrushSelector();
		
		DrawBrushSettings(10.0f, 1.0f);
	}
	
	private void DrawSmoothControls()
	{
		DrawInstructions("Click on the terrain to smooth the surface or to soften the boundary between textures.");
		
		DrawBrushSelector();
		
		DrawBrushSettings(10.0f, 1.0f);
	}
	
	private void DrawPaintControls()
	{
		DrawInstructions("Select a brush and material below, then click the terrain to paint the material on it.");
		
		DrawBrushSelector();
		
		DrawMaterialSelector();
		
		DrawBrushSettings(10.0f, 1.0f);
		
		/*for(int ct = 0; ct < License.MaxNoOfMaterials; ct++)
		{
			smoothTerrainVolume.materials[ct].diffuseMap = EditorGUILayout.ObjectField(smoothTerrainVolume.materials[ct].diffuseMap,typeof(Texture),false, GUILayout.Width(80), GUILayout.Height(80)) as Texture2D;
		}*/
	}
	
	private void DrawSettingsControls()
	{
	}
	
	private void DrawInstructions( string message)
	{
		EditorGUILayout.LabelField("Instructions", EditorStyles.boldLabel);
		EditorGUILayout.HelpBox(message, MessageType.None);
		EditorGUILayout.Space();
	}
	
	private void DrawBrushSelector()
	{
		EditorGUILayout.LabelField("Brushes", EditorStyles.boldLabel);
		selectedBrush = DrawTextureSelectionGrid(selectedBrush, brushTextures, 5, 50);
		EditorGUILayout.Space();
	}
	
	private void DrawMaterialSelector()
	{
		EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
		
		Texture2D[] diffuseMaps = new Texture2D[smoothTerrainVolume.materials.Length];
		for(int i = 0; i < smoothTerrainVolume.materials.Length; i++)
		{
			if(smoothTerrainVolume.materials[i] != null)
			{
				diffuseMaps[i] = smoothTerrainVolume.materials[i].diffuseMap;
			}
		}
		selectedTexture = DrawTextureSelectionGrid(selectedTexture, diffuseMaps, 3, 80);
		
		EditorGUILayout.Space();
		
		if(GUILayout.Button("Edit selected material..."))
		{
			if(smoothTerrainVolume.materials[selectedTexture] != null)
			{
				smoothTerrainVolume.materials[selectedTexture] = new SmoothTerrainMaterial();
			}
			
			SmoothTerrainMaterialEditorWindow.EditMaterial(smoothTerrainVolume.materials[selectedTexture]);
		}
		
		EditorGUILayout.Space();
	}
	
	private void DrawBrushSettings(float maxBrushRadius, float maxOpacity)
	{
		EditorGUILayout.LabelField("Brush settings", EditorStyles.boldLabel);
		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Radius:", GUILayout.Width(50));
			brushOuterRadius = GUILayout.HorizontalSlider(brushOuterRadius, 0.0f, maxBrushRadius);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Opacity:", GUILayout.Width(50));
			opacity = GUILayout.HorizontalSlider(opacity, 0.0f, maxOpacity);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Space();
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
		
		//Now draw the texture selection grid
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
			keywords.Add("BRUSH_MARKER_ON");
			smoothTerrainVolume.material.shaderKeywords = keywords.ToArray();
			//Shader.DisableKeyword("SHOW_BRUSH");
			//Shader.EnableKeyword("HIDE_BRUSH");
			smoothTerrainVolume.material.SetVector("_BrushCenter", new Vector4(resultX, resultY, resultZ, 0.0f));				
			smoothTerrainVolume.material.SetVector("_BrushSettings", new Vector4(brushInnerRadius, brushOuterRadius, opacity, 0.0f));
			smoothTerrainVolume.material.SetVector("_BrushColor", new Vector4(0.0f, 0.5f, 1.0f, 1.0f));
			
			if(((e.type == EventType.MouseDown) || (e.type == EventType.MouseDrag)) && (e.button == 0))
			{
				if(sculptPressed)
				{
					float multiplier = 1.0f;
					if(e.modifiers == EventModifiers.Shift)
					{
						multiplier  = -1.0f;
					}
					Cubiquity.SculptSmoothTerrainVolume(smoothTerrainVolume, resultX, resultY, resultZ, brushInnerRadius, brushOuterRadius, opacity * multiplier);
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