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
		
		protected void Awake()
		{
			ghostGameObject = new GameObject("Ghost");
			ghostGameObject.hideFlags = HideFlags.HideAndDontSave;
		}
		
		protected void OnDestroy()
		{		
			Debug.Log ("Volume.OnDestroy()");
			
			// Game objects in our tree are created with the 'DontSave' flag set, and according to the Unity docs this means
			// we have to destroy them manually. In the case of 'Destroy' the Unity docs explicitally say that it will destroy
			// transform children as well, so I'm assuming DestroyImmediate has the same behaviour.
			DestroyImmediate(ghostGameObject);
		}
		
		public void Synchronize()
		{
			// NOTE - The following line passes transform.worldToLocalMatrix as a shader parameter. This is explicitly
			// forbidden by the Unity docs which say:
			//
			//   IMPORTANT: If you're setting shader parameters you MUST use Renderer.worldToLocalMatrix instead.
			//
			// However, we don't have a renderer on this game object as the rendering is handled by the child OctreeNodes.
			// The Unity doc's do not say why this is the case, but my best guess is that it is related to secret scaling 
			// which Unity may perform before sending data to the GPU (probably to avoid precision problems). See here:
			//
			//   http://forum.unity3d.com/threads/153328-How-to-reproduce-_Object2World
			//
			// It seems to work in our case, even with non-uniform scaling applied to the volume. Perhaps we are just geting
			// lucky, pehaps it just works on our platform, or perhaps it is actually valid for some other reason. Just be aware.
			gameObject.GetComponent<VolumeRenderer>().material.SetMatrix("_World2Volume", transform.worldToLocalMatrix);
			
			// Update the transform on the ghost game object to match the real game object.
			if(transform.hasChanged)
			{
				ghostGameObject.transform.localPosition = transform.localPosition;
				ghostGameObject.transform.localRotation = transform.localRotation;
				ghostGameObject.transform.localScale = transform.localScale;
				transform.hasChanged = false;
			}
		}
	}
}