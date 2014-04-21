using UnityEngine;
using System.Collections;

using Cubiquity.Impl;

namespace Cubiquity
{
	public abstract class VolumeRenderer : MonoBehaviour
	{
		public Material material;
		
		private bool previouslyEnabled;
		
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
		private bool mCastShadows;
		
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
		private bool mReceiveShadows;
		
		public uint lastModified = Clock.timestamp;
		
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
