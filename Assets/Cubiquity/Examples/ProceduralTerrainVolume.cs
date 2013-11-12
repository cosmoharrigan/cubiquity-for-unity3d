using UnityEngine;
using System.Collections;

using Cubiquity;

public class ProceduralTerrainVolume : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		int width = 128;
		int height = 32;
		int depth = 128;
		
		// FIXME - Where should we delete this?
		TerrainVolumeData data = ScriptableObject.CreateInstance<TerrainVolumeData>();
		data.Init(new Region(0, 0, 0, width-1, height-1, depth-1));
		
		TerrainVolume.CreateGameObject(data);
			
		// Create some ground in the terrain so it shows up in the editor.
		// Soil as a base (mat 0) and then a couple of layers of grass (mat 1).
		//TerrainVolumeGenerator.GenerateFloor(data, 6, (uint)0, 8, (uint)1);
		
		MaterialSet materialSet = new MaterialSet();
		
		float scaleX = 32.0f;
		float scaleY = 32.0f;
		float scaleZ = 32.0f;
		
		float invScaleX = 1.0f / scaleX;
		float invScaleY = 1.0f / scaleY;
		float invScaleZ = 1.0f / scaleZ;
		
		float minVal = 1000000.0f;
		float maxVal = -1000000.0f;
		
		for(int z = 0; z < depth; z++)
		{
			for(int y = 0; y < height; y++)
			{
				for(int x = 0; x < width; x++)
				{		
					float altitude = (float)(y + 1) / (float)height;
					
					materialSet.weights[0] = 0;
					
					float sampleX = (float)x * invScaleX;
					float sampleY = (float)y * invScaleY;
					float sampleZ = (float)z * invScaleZ;
					
					float val = 0.0f;
					int noOfOctaves = 1;
					float octaveScale = 1.0f;
					float rangeCounter = 0.0f;
					for(int octave = 0; octave < noOfOctaves; octave++)
					{
						rangeCounter += octaveScale;
						val += octaveScale * SimplexNoise.Noise.Generate(sampleX / octaveScale, sampleY / octaveScale, sampleZ / octaveScale);
						
						octaveScale *= 0.5f;
					}
					
					//float val = 0.5f;
					
					minVal = Mathf.Min(minVal, val);
					maxVal = Mathf.Max(maxVal, val);
					
					//val *= (1.0f - altitude);
					
					//float scaledVal = (val * 10.0f + 127.5f);
					
					float scaledVal = (val / rangeCounter) + 0.5f;
					
					//altitude = altitude * altitude;
					
					altitude = Mathf.Sqrt(altitude);
					
					scaledVal *= (1.0f - altitude);
					
					scaledVal *= 255;
					
					//scaledVal *= (1.0f - altitude);
					
					scaledVal = Mathf.Clamp(scaledVal, 0.0f, 255.0f);
					
					materialSet.weights[0] = (byte)scaledVal;
					
					if(y < 1)
					{
						materialSet.weights[0] = 255;
					}
					
					data.SetVoxel(x, y, z, materialSet);
				}
			}
		}
		
		Debug.Log("Min = " + minVal);
		Debug.Log("Max = " + maxVal);
	}
}
