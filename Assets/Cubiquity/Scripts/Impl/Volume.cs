using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	public class Volume : MonoBehaviour
	{
		public int maxNodesPerSync = 4;
		
		// This corresponds to the root OctreeNode in Cubiquity.
		protected GameObject rootGameObject;
		protected GameObject ghostGameObject;
		
		private void Awake()
		{
			ghostGameObject = new GameObject("Ghost");
			ghostGameObject.hideFlags = HideFlags.HideAndDontSave;
			ghostGameObject.AddComponent<GhostObjectSource>();
			ghostGameObject.GetComponent<GhostObjectSource>().sourceGameObject = gameObject;
		}
		
		private void OnDestroy()
		{		
			Debug.Log ("Volume.OnDestroy()");
			
			// Game objects in our tree are created with the 'DontSave' flag set, and according to the Unity docs this means
			// we have to destroy them manually. In the case of 'Destroy' the Unity docs explicitally say that it will destroy
			// transform children as well, so I'm assuming DestroyImmediate has the same behaviour.
			DestroyImmediate(ghostGameObject);
		}
	}
}