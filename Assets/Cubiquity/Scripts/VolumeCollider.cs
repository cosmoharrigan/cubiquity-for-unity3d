using UnityEngine;
using System.Collections;

using Cubiquity.Impl;

namespace Cubiquity
{
	/// Causes the volume to have a collision mesh and allows it to participate in collisions.
	/**
	 * The role of the VolumeCollider component for volumes is conceptually similar to the role of Unity's MeshCollider class for meshes.
	 * Specifically, it can be attached to a GameObject which also has a Volume component to cause that Volume component to be able to
	 * collide with other colliders.
	 * 
	 * The VolumeCollider does not cuurently expose any properties - it is enough for it to simply be present. In the future it will probably
	 * expose additional properties which will be copied to the underlying MeshCollider.
	 * 
	 * \sa VolumeRenderer
	 */
	public abstract class VolumeCollider : MonoBehaviour
	{
		public abstract Mesh BuildMeshFromNodeHandle(uint nodeHandle);
		
		public uint lastModified = Clock.timestamp;
		
		// Dummy start method rqured for the 'enabled' checkbox to show up in the inspector.
		void Start() { }
	}
}
