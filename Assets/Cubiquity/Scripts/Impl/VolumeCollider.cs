using UnityEngine;
using System.Collections;

using Cubiquity.Impl;

namespace Cubiquity
{
	public abstract class VolumeCollider : MonoBehaviour
	{
		public abstract Mesh BuildMeshFromNodeHandle(uint nodeHandle);
	}
}
