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
						// We are going to compute our density value based on the distance of a voxel from the center of our earth.
						// This is a function which (by definition) is zero at the center of the earth and has a smoothly increasing
						// value as we move away from the center.
						//
						// Note: For efficiency we could probably adapt this to work with squared distances (thereby eliminating
						// the square root operation), but we'd like to keep this example as intuitive as possible.
						float distFromCenter = Mathf.Sqrt(x * x + y * y + z * z);
						
						// We actually want our volume to have high values in the center and low values as we move out, because our
						// eath should be a solid sphere surrounded by empty space. If we invert the distance then this is a step in
						// the right direction. We still have zero in the center, but lower (negative) values as we move out.
						float density = -distFromCenter;
						
						density += earthRadius;
						
						density *= 50;
						density += 127;
						
						byte densityAsByte = (byte)(Mathf.Clamp(density, 0, 255));
						
						rock.weights[0] = densityAsByte;
						
						volume.data.SetVoxel(x, y, z, rock);
						
					}
				}
			}
		}
	}
}
