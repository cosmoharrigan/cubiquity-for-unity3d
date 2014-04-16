using UnityEngine;
using System.Collections;

using Cubiquity.Impl;

namespace Cubiquity
{	
	[ExecuteInEditMode]
	public class TerrainVolumeRenderer : VolumeRenderer
	{
		void Awake()
		{
			if(material == null)
			{
				// Triplanar textuing seems like a good default material for the terrain volume.
				material = new Material(Shader.Find("TriplanarTexturing"));
			}
		}
	}
}
