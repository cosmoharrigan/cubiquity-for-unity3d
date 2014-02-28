using UnityEngine;
using System.Collections;

using Cubiquity.Impl;

namespace Cubiquity
{
	public abstract class VolumeRenderer : MonoBehaviour
	{
		public Material material;
		public abstract Mesh BuildMeshFromNodeHandle(uint nodeHandle);
	}
}
