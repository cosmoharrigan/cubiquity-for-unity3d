using UnityEditor;
using UnityEngine;

using System;
using System.Collections;
using System.IO;

public class CreateProceduralColoredCubesVolumeWizard : CreateVolumeWizard
{	
	[MenuItem ("GameObject/Create Other/Colored Cubes Volume/Create Colored Cubes Volume Procedurally...")]
    static void CreateWizard ()
	{
		// Make sure the Cubiquity library is installed.
		Installation.ValidateAndFix();
		
        ScriptableWizard.DisplayWizard<CreateProceduralColoredCubesVolumeWizard>("Create Colored Cubes Volume Procedurally");
    }
	
	void OnGUI()
	{
		GUIStyle labelWrappingStyle = new GUIStyle(GUI.skin.label);
		labelWrappingStyle.wordWrap = true;
		
		GUIStyle rightAlignmentStyle = new GUIStyle(GUI.skin.textField);
		rightAlignmentStyle.alignment = TextAnchor.MiddleRight;
		
		OnGuiHeader(true); //Pass true because we let the user specify the size.
		
		GUILayout.Space(20); // A space before the create/cancel buttons
		
		OnGuiFooter();
	}
	
	public override void OnCreatePressed()
	{
		Close();
		
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
}
