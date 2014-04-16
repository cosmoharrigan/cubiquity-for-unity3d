using UnityEngine;
using System.Collections;

using Cubiquity.Impl;

namespace Cubiquity
{	
	[ExecuteInEditMode]
	public class ColoredCubesVolumeRenderer : VolumeRenderer
	{
		void Awake()
		{
			if(material == null)
			{
				// This shader should be appropriate in most scenarios, and makes a good default.
				material = new Material(Shader.Find("ColoredCubesVolume"));
			}
		}
	}
}
