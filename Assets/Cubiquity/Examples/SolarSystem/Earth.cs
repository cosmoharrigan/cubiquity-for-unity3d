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
			
			TerrainVolume volume = GetComponent<TerrainVolume>();
			
			int earthRadius = 60;
			Region volumeBounds = new Region(-earthRadius, -earthRadius, -earthRadius, earthRadius, earthRadius, earthRadius);		
			TerrainVolumeData result = VolumeData.CreateEmptyVolumeData<TerrainVolumeData>(volumeBounds);
			
			// The numbers below control the thinkness of the various layers.
			TerrainVolumeGenerator.GeneratePlanet(result, earthRadius, earthRadius - 1, earthRadius - 10, earthRadius - 35);
			
			volume.data = result;
		}
		
		void Update()
		{
			earthOrbitPoint.transform.Rotate(new Vector3(0,1,0), Time.deltaTime * earthOrbitSpeed);
			transform.Rotate(new Vector3(0,1,0), Time.deltaTime * earthRotationSpeed);
		}
	}
}
