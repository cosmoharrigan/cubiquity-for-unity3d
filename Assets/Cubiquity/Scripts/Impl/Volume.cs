using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cubiquity
{
	public class Volume : MonoBehaviour
	{
		public int maxNodesPerSync = 4;
		
		// We are currently serializing the rootGameObject and then just discarding it on load. It's already set to null,
		// but we've seen warnings about leaking resources if we don't serialize it. We should come back to this - I suspect
		// it may fix itself once we tidy some other aspects of the system. Ideally we just wouldn't serialize this object
		// and that would remove all the discarding octree nonsense we have going on.
		//
		// See also this issue () but be aware it is slightly different, as that refers to not serializing components with
		// 'DontSave' whereas here we are talking about not serializing the game object by making it private/[[NonSerialzed].
		[SerializeField]
		protected GameObject rootGameObject;
		
		protected void Awake()
		{
			Debug.Log("In Volume.Awake()");
			if(rootGameObject != null)
			{
				Debug.LogWarning("Root octree node is already set.");
				DestroyImmediate(rootGameObject);
			}
			
			StartCoroutine(Synchronization());
		}
		
		// I don't understand why we need to do this. In the pst we've seen a Unity bug with the DontSave flag but this is
		// different in that the [System.NonSerialized] flag is what should be preventing the saving. However, a whole bunch
		// of mesh data gets saved to disk unless we call this before serialization.
		public void DiscardOctree()
		{
			DestroyImmediate(rootGameObject);
			rootGameObject = null;
		}
		
		IEnumerator Synchronization()
		{
			while(true)
			{
				Synchronize();
				yield return null;
			}
		}
		
		public virtual void Synchronize()
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
			VolumeRenderer volumeRenderer = gameObject.GetComponent<VolumeRenderer>();
			if(volumeRenderer != null)
			{
				if(volumeRenderer.material != null)
				{
					volumeRenderer.material.SetMatrix("_World2Volume", transform.worldToLocalMatrix);
				}
			}
		}
	}
}