using UnityEngine;
using System.Collections;

using Cubiquity;

/**
 * This class serves as an example of how to generate a TerrainVolume from code. The exact operation
 * of the noise function(s) is not particularly important here as you will want to implement your own
 * approach for your game, but you should focus on understanding how data is written into the volume.
 */
public class ProceduralTerrainVolume : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		// The size of the volume we will generate
		int width = 256;
		int height = 32;
		int depth = 256;
		
		// FIXME - Where should we delete this?
		TerrainVolumeData data = ScriptableObject.CreateInstance<TerrainVolumeData>();
		data.Init(new Region(0, 0, 0, width-1, height-1, depth-1));
		
		// Set up our textures in the appropriate material slots.
		data.materials[0].diffuseMap = Resources.Load("Textures/Rock") as Texture2D;
		data.materials[0].scale = new Vector3(16.0f, 16.0f, 16.0f);		
		data.materials[1].diffuseMap = Resources.Load("Textures/Soil") as Texture2D;		
		data.materials[2].diffuseMap = Resources.Load("Textures/Grass") as Texture2D;
		
		// This scale factor comtrols the size of the rocks which are generated.
		float rockScale = 32.0f;		
		float invRockScale = 1.0f / rockScale;
		
		// Let's keep the allocation outside of the loop.
		MaterialSet materialSet = new MaterialSet();
		
		// Iterate over every voxel of our volume
		for(int z = 0; z < depth; z++)
		{
			for(int y = height-1; y > 0; y--)
			{
				for(int x = 0; x < width; x++)
				{		
				
					float altitude = (float)(y + 1) / (float)height;
					
					//altitude += 0.1f;
					
					altitude = Mathf.Clamp(altitude, 0.0f, 1.0f);
					
					altitude -= 0.5f;
					altitude *= 2.0f;
					
					
					//altitude = Mathf.Sqrt(altitude);
					
					
					materialSet.weights[0] = 0;
					materialSet.weights[1] = 0;
					materialSet.weights[2] = 0;
					
					float sampleX = (float)x * invRockScale;
					float sampleY = (float)y * invRockScale;
					float sampleZ = (float)z * invRockScale;
					
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
					
					float scaledVal = (val / rangeCounter) + 0.5f;
					
					//altitude = altitude * altitude;					
					
					
					//scaledVal *= (1.0f - altitude);
					
					scaledVal -= altitude;
					
					scaledVal -= 0.5f;
					scaledVal *= 5.0f;
					scaledVal += 0.5f;
					
					scaledVal *= 255;
					
					//scaledVal -= 100.0f;
					
					//scaledVal *= (1.0f - altitude);
					
					scaledVal = Mathf.Clamp(scaledVal, 0.0f, 255.0f);
					
					/*if(scaledVal < 100.0f)
					{
						scaledVal = 0.0f;
					}*/
					
					materialSet.weights[0] = (byte)scaledVal;
					
					byte excess = (byte)(255 - materialSet.weights[0]);
					
					if(y < 9)
					{						
						//materialSet.weights[0] = 0;
						materialSet.weights[1] = excess;
						//materialSet.weights[2] = 0;
					}
					else if(y < 12)
					{
						//materialSet.weights[0] = 0;
						//materialSet.weights[1] = 0;
						materialSet.weights[2] = excess;
					}
					
					data.SetVoxel(x, y, z, materialSet);
				}
			}
		}
		
		TerrainVolume.CreateGameObject(data);
	}
}
