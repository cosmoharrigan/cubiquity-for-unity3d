using UnityEngine;
using System.Collections;

using Cubiquity.Impl;

namespace Cubiquity
{
	public abstract class VolumeRenderer : MonoBehaviour
	{
		public Material material;
		public abstract Mesh BuildMeshFromNodeHandle(uint nodeHandle);
		
		public bool hasChanged = true;
		
		// Dummy start method rqured for the 'enabled' checkbox to show up in the inspector.
		void Start() { }
	}
}
