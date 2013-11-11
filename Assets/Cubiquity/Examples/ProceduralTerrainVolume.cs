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
		
		float scaleX = 10.0f;
		float scaleY = 10.0f;
		float scaleZ = 10.0f;
		
		float invScaleX = 1.0f / scaleX;
		float invScaleY = 1.0f / scaleY;
		float invScaleZ = 1.0f / scaleZ;
		
		for(int z = 0; z < depth; z++)
		{
			for(int y = 0; y < height; y++)
			{
				for(int x = 0; x < width; x++)
				{		
					materialSet.weights[0] = 0;
					
					float sampleX = (float)x * invScaleX;
					float sampleY = (float)y * invScaleY;
					float sampleZ = (float)z * invScaleZ;
					
					float val = SimplexNoise.Noise.Generate(sampleX, sampleY, sampleZ);
					
					float scaledVal = (val * 1000.0f + 127.5f);
					scaledVal = Mathf.Clamp(scaledVal, 0.0f, 255.0f);
					
					materialSet.weights[0] = (byte)scaledVal;
					
					data.SetVoxel(x, y, z, materialSet);
				}
			}
		}
	}
}
