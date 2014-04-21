using UnityEngine;
using System.Collections;

using Cubiquity.Impl;

namespace Cubiquity
{
	public abstract class VolumeCollider : MonoBehaviour
	{
		public abstract Mesh BuildMeshFromNodeHandle(uint nodeHandle);
		
		public uint lastModified = Clock.timestamp;
		private bool previouslyEnabled;
		
		// A Start/Update method causes the 'enabled' checkbox to show up in the inspector.
		// We also use these functions to make sure that changing the flag updates the timestamp.
		void Start()
		{
			previouslyEnabled = base.enabled;
			lastModified = Clock.timestamp;
		}
		
		void Update()
		{
			if(enabled != previouslyEnabled)
			{
				lastModified = Clock.timestamp;
			}
		}
	}
}
