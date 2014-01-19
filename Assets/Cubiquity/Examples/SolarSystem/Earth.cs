using UnityEngine;
using System.Collections;

using Cubiquity;

//FIXME - Should check the .Net rules regarding how the naming of namespaces corresponds to the naming of .dlls.
namespace CubiquityExamples
{
	public class Earth : MonoBehaviour
	{
		void Start()
		{
			gameObject.AddComponent<TerrainVolume>();
			gameObject.AddComponent<TerrainVolumeRenderer>();
			
			TerrainVolume volume = GetComponent<TerrainVolume>();
			TerrainVolumeRenderer volumeRenderer = GetComponent<TerrainVolumeRenderer>();
			
			Material material = new Material(Shader.Find("MaterialSetDebug"));
			volumeRenderer.material = material;
			
			int earthRadius = 30;
			Region volumeBounds = new Region(-earthRadius, -earthRadius, -earthRadius, earthRadius, earthRadius, earthRadius);		
			TerrainVolumeData result = TerrainVolumeData.CreateEmptyVolumeData(volumeBounds, VolumeData.Paths.TemporaryCache, VolumeData.GeneratePathToVoxelDatabase());
			
			volume.data = result;
			
			int earthRadiusSquared = earthRadius * earthRadius;
			MaterialSet space = new MaterialSet();
			MaterialSet rock = new MaterialSet();
			for(int z = volumeBounds.lowerCorner.z; z <= volumeBounds.upperCorner.z; z++)
			{
				for(int y = volumeBounds.lowerCorner.y; y <= volumeBounds.upperCorner.y; y++)
				{
					for(int x = volumeBounds.lowerCorner.x; x <= volumeBounds.upperCorner.x; x++)
					{
						float distFromCenterSquared = x * x + y * y + z * z;
						float distFromCenter = Mathf.Sqrt(distFromCenterSquared);
						
						float density = 148 - distFromCenter;
						
						density -= 127;
						density *= 50;
						density += 127;
						
						density = Mathf.Clamp(density, 0, 255);
						
						rock.weights[0] = (byte)density;
						
						volume.data.SetVoxel(x, y, z, rock);
						
					}
				}
			}
		}
	}
}
