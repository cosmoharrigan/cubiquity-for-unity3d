using UnityEngine;

using System;
using System.IO;
using System.Collections;

namespace Cubiquity
{
	[System.Serializable]
	public abstract class VolumeData : ScriptableObject
	{		
		// We need to explicitly serialize the private field because
		// Unity3D doesn't automatically serialize the public property
		[SerializeField]
		protected Region _region;
	    public Region region
	    {
	        get { return this._region; }
	    }
		
		// If set, this identifies the volume to the Cubiquity DLL. It can
		// be tested against null to find if the volume is currently valid.
		[System.NonSerialized] // Internal variables aren't serialized anyway?
		internal uint? volumeHandle = null;
		
		[SerializeField]
		protected string pathToVoxels;
		
		// Don't really like having this defined here. The base node size should be a rendering property rather than a
		// property of the actual volume data. Need to make this change in the underlying Cubiquity library as well though.
		protected static uint DefaultBaseNodeSize = 32;
		
		// We use a static Random for making filenames, as Randoms are seeded by timestamp
		// and client code could potentially create a number of volumes on quick sucession.  
		protected static System.Random randomIntGenerator = new System.Random();
		
		private void OnEnable()
		{			
			InitializeCubiquityVolume();
		}
		
		private void OnDisable()
		{
			ShutdownCubiquityVolume();
		}
		
		protected abstract void InitializeCubiquityVolume();
		protected abstract void ShutdownCubiquityVolume();
		
		protected string GeneratePathToVoxels()
		{
			// Generate a random filename from an integer
			string filename = randomIntGenerator.Next().ToString("X8") + ".vol";
			return Application.streamingAssetsPath + Path.DirectorySeparatorChar + filename;
		}
	}
}
