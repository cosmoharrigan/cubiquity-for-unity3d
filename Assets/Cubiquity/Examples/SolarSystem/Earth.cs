using UnityEngine;
using System.Collections;

using Cubiquity;

//FIXME - Should check the .Net rules regarding how the naming of namespaces corresponds to the naming of .dlls.
namespace CubiquityExamples
{
	public class Earth : MonoBehaviour
	{
		public float earthOrbitSpeed = 1.0f;
		public float earthRotationSpeed = -5.0f;
		
		GameObject earthOrbitPoint;
		
		void Start()
		{
			earthOrbitPoint = transform.parent.gameObject;
			
			gameObject.AddComponent<TerrainVolume>();
			gameObject.AddComponent<TerrainVolumeRenderer>();
			
			TerrainVolume volume = GetComponent<TerrainVolume>();
			TerrainVolumeRenderer volumeRenderer = GetComponent<TerrainVolumeRenderer>();
			
			Material material = new Material(Shader.Find("Planet"));
			volumeRenderer.material = material;
			
			Cubemap earthSurfaceTexture = Resources.Load("Textures/EarthSurface") as Cubemap;			
			
			material.SetTexture("_Tex0", earthSurfaceTexture);
			
			int earthRadius = 30;
			Region volumeBounds = new Region(-earthRadius, -earthRadius, -earthRadius, earthRadius, earthRadius, earthRadius);		
			TerrainVolumeData result = TerrainVolumeData.CreateEmptyVolumeData(volumeBounds, VolumeData.Paths.TemporaryCache, VolumeData.GeneratePathToVoxelDatabase());
			
			volume.data = result;
			
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
						
						// By adding the 'earthRadius' we now have a function which starts at 'earthRadius' and still decreases as it
						// moves out. The function passes through zero at a distance of 'earthRadius' and then continues do decrease
						// as it gets even further out.
						density += earthRadius;
						
						// Ideally we would like our final density value to be '255' for voxels inside the earth and '0' for voxels
						// outside the earth. At the surface there should be a transition but this should occur not too quickly and
						// not too slowly, as both of these will result in a jagged appearance to the mesh.
						//
						// We probably want the transition to occur over a few voxels, whereas it currently occurs over 255 voxels
						// because it was derived from the distance. By scaling the density field we effectivly compress the rate
						// at which it changes at the surface. We also make the center much too high and the outside very low, but
						// we will clamp these to the corect range later.
						//
						// Note: You can try commenting out or changing the value on this line to see the effect it has.
						density *= 50;
						
						// Until now we've been defining our density field as if the threshold was at zero, with positive densities
						// being solid and negative densities being empty. But actually Cubiquity operates on the range 0 to 255, and
						// uses a threashold of 127 to decide where to place the generated surface.  Therefore we shift and clamp our
						// density value and store it in a byte.
						density += 127;						
						byte densityAsByte = (byte)(Mathf.Clamp(density, 0, 255));
						
						rock.weights[0] = densityAsByte;
						
						volume.data.SetVoxel(x, y, z, rock);
						
					}
				}
			}
		}
		
		void Update()
		{
			earthOrbitPoint.transform.Rotate(new Vector3(0,1,0), Time.deltaTime * earthOrbitSpeed);
			transform.Rotate(new Vector3(0,1,0), Time.deltaTime * earthRotationSpeed);
		}
	}
}
