using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	public abstract class VolumeRenderer : MonoBehaviour
	{
		public abstract void BuildMeshFromNodeHandle(uint nodeHandle, out Mesh renderingMesh, out Mesh physicsMesh, bool UseCollisionMesh);
	}
}
