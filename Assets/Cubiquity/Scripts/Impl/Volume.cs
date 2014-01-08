using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cubiquity
{
	public class Volume : MonoBehaviour
	{
		public static List<Volume> all = new List<Volume>();
		
		public int maxNodesPerSync = 4;
		
		// This corresponds to the root OctreeNode in Cubiquity.
		public GameObject rootGameObject;
		
		protected void Awake()
		{
			Debug.Log("In Volume.Awake()");
			all.Add(this);
			if(rootGameObject != null)
			{
				Debug.LogWarning("Root octree node is already set.");
				DestroyImmediate(rootGameObject);
			}
			
			Debug.Log("Starting Coroutine");
			StartCoroutine("SynchronizationCoroutine");
		}
		
		void OnEnable()
		{
			Debug.Log("Starting Coroutine");
			StartCoroutine("SynchronizationCoroutine");
		}
		
		void OnDisable()
		{
			Debug.Log("Stoping Coroutine");
			StopCoroutine("SynchronizationCoroutine");
		}
		
		protected void OnDestroy()
		{		
			Debug.Log ("Volume.OnDestroy()");
			all.Remove(this);
		}
		
		IEnumerator SynchronizationCoroutine()
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