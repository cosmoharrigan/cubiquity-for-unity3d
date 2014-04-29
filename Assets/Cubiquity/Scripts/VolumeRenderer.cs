using UnityEngine;
using System.Collections;

using Cubiquity.Impl;

namespace Cubiquity
{
	/// Controls some visual aspects of the volume and allows it to be rendered.
	/**
	 * The role of the VolumeRenderer component for volumes is conceptually similar to the role of Unity's MeshRenderer class for meshes.
	 * Specifically, it can be attached to a GameObject which also has a Volume component to cause that Volume component to be drawn. It 
	 * also exposes a number of properties such as whether a volume should cast and receive shadows.
	 * 
	 * Remember that Cubiquity acctually draws the volume by creating standard Mesh objects. Internally Cubiquity will copy the properties
	 * of the VolumeRenderer to the MeshRenderers which are generated.
	 * 
	 * \sa VolumeCollider
	 */
	public abstract class VolumeRenderer : MonoBehaviour
	{
		public Material material;
		
		/// Controls whether this volume casts shadows.
		public bool castShadows
		{
			get
			{
				return mCastShadows;
			}
			set
			{
				if(mCastShadows != value)
				{
					mCastShadows = value;
					lastModified = Clock.timestamp;
				}
			}
		}
		[SerializeField]
		private bool mCastShadows = true;
		
		/// Controls whether this volume receives shadows.
		public bool receiveShadows
		{
			get
			{
				return mReceiveShadows;
			}
			set
			{
				if(mReceiveShadows != value)
				{
					mReceiveShadows = value;
					lastModified = Clock.timestamp;
				}
			}
		}
		[SerializeField]
		private bool mReceiveShadows = true;
		
		/// \cond
		public uint lastModified = Clock.timestamp;
		/// \endcond
		
		// Dummy start method rqured for the 'enabled' checkbox to show up in the inspector.
		void Start() { }
	}
}
