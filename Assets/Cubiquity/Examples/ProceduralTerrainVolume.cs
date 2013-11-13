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
		
		data.materials[0].diffuseMap = Resources.Load("Textures/Rock") as Texture2D;
		data.materials[0].scale = new Vector3(16.0f, 16.0f, 16.0f);
		
		data.materials[1].diffuseMap = Resources.Load("Textures/Soil") as Texture2D;
		
		data.materials[2].diffuseMap = Resources.Load("Textures/Grass") as Texture2D;
		
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
			for(int y = height-1; y > 0; y--)
			{
				for(int x = 0; x < width; x++)
				{		
					float altitude = (float)(y + 0) / (float)height;
					altitude = Mathf.Clamp(altitude, 0.0f, 1.0f);
					altitude = Mathf.Sqrt(altitude);
					
					
					materialSet.weights[0] = 0;
					materialSet.weights[1] = 0;
					materialSet.weights[2] = 0;
					
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
					
					
					scaledVal *= (1.0f - altitude);
					
					scaledVal -= 0.5f;
					scaledVal *= 5.0f;
					scaledVal += 0.5f;
					
					scaledVal *= 255;
					
					//scaledVal *= (1.0f - altitude);
					
					scaledVal = Mathf.Clamp(scaledVal, 0.0f, 255.0f);
					
					/*if(scaledVal < 100.0f)
					{
						scaledVal = 0.0f;
					}*/
					
					materialSet.weights[0] = (byte)scaledVal;
					
					if(y < 5)
					{
						//MaterialSet temp = data.GetVoxel(x, y+1, z);
						
						materialSet.weights[0] = 0;
						materialSet.weights[1] = 255;
						materialSet.weights[2] = 0;
					}
					else if(y < 6)
					{
						materialSet.weights[0] = 0;
						materialSet.weights[1] = 0;
						materialSet.weights[2] = 255;
					}
					
					data.SetVoxel(x, y, z, materialSet);
				}
			}
		}
		
		Debug.Log("Min = " + minVal);
		Debug.Log("Max = " + maxVal);
	}
}
