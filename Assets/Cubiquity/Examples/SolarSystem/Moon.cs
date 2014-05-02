using UnityEngine;
using System.Collections;

using Cubiquity;

//FIXME - Should check the .Net rules regarding how the naming of namespaces corresponds to the naming of .dlls.
namespace CubiquityExamples
{
	public class Moon : MonoBehaviour
	{
		public float moonOrbitSpeed = 3.0f;
		public float moonRotationSpeed = -10.0f;
		
		GameObject moonOrbitPoint;
		
		void Start()
		{
			moonOrbitPoint = transform.parent.gameObject;
			
			TerrainVolume volume = GetComponent<TerrainVolume>();
			
			int moonRadius = 15;
			Region volumeBounds = new Region(-moonRadius, -moonRadius, -moonRadius, moonRadius, moonRadius, moonRadius);		
			TerrainVolumeData result = VolumeData.CreateEmptyVolumeData<TerrainVolumeData>(volumeBounds);
			
			TerrainVolumeGenerator.GeneratePlanet(result, moonRadius, moonRadius - 1, 0, 0);
			
			volume.data = result;
		}
		
		void Update()
		{
			moonOrbitPoint.transform.Rotate(new Vector3(0,1,0), Time.deltaTime * moonOrbitSpeed);
			transform.Rotate(new Vector3(0,1,0), Time.deltaTime * moonRotationSpeed);
		}
	}
}
