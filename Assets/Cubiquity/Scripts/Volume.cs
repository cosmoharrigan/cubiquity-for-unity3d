using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Cubiquity.Impl;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cubiquity
{
	/// Base class representing a three-dimensional grid of voxels. 
	/**
	 * Volumes are probably the single most fundamental concept of %Cubiquity. They essentially provide a 3D array of voxels which the user can
	 * modify at will, and they take care of synchronizing and maintaining a mesh representation of the voxels for rendering and collision detection.
	 *
	 * The Volume class itself is actually an abstract base class which cannot be instantiated by user code directly. Instead is serves as a base
	 * for the ColoredCubesVolume and TerrainVolume and encapsulates some of the common behaviour which such derived classes need. It is used in
	 * conjunction with the VolumeData, VolumeRenderer and VolumeCollider to form a structure as given below:
	 *
	 * Diagram of volume structure here.
	 *
	 * Note how this is conceptually similar to the way that Unity's mesh classes are structured, where the MeshFilter works in conjunction with 
	 * the Mesh, MeshRenderer and MeshCollider classes.
	 */
	[ExecuteInEditMode]
	public abstract class Volume : MonoBehaviour
	{		
		// Indicates whether the mesh representation is currently up to date with the volume data. Note that this property may
		// fluctuate rapidly during real-time editing as the system tries to keep up with the users modifications, and also that
		// it may lag a few frames behind the true syncronization state.
		public bool isMeshSyncronized
		{
			get { return mIsMeshSyncronized; }
			protected set
			{
				// Check if the state of the mesh sync variable has actually changed.
				if(mIsMeshSyncronized != value)
				{
					// If so update it.
					mIsMeshSyncronized = value;
					
					// And fire the appropriate event.
					if(mIsMeshSyncronized)
					{
						if(OnMeshSyncComplete != null) { OnMeshSyncComplete(); }
					}
					else
					{
						if(OnMeshSyncLost != null) { OnMeshSyncLost(); }
					}
				}
			}
		} private bool mIsMeshSyncronized = false;
		
		/// Delegate type used by OnMeshSyncComplete and OnMeshSyncLost
		public delegate void MeshSyncAction();
		/// Description here
		public event MeshSyncAction OnMeshSyncComplete;
		/// Description here
		public event MeshSyncAction OnMeshSyncLost;
		
		/// Sets an upper limit on the rate at which the mesh representation is updated to match the volume data.
		/**
		 * %Cubiquity continuously checks whether the the mesh representation (used for rendering and physics) is synchronized with the underlying
		 * volume data. Such synchronization can be lost whenever the volume data is modified, and %Cubiquity will then regenerate the mesh. This
		 * regeneration process can take some time, and so typically you want to spread the regeneration over a number of frames.
		 *
		 * Internally %Cubiquity breaks down the volume into a number regions each corresponding to an octree node, and these can be resynchronized
		 * individually. Therefore this property controls how many of the octree nodes will be resynchronized each frame. A small value will result
		 * in a better frame rate when modifications are being performed, but at the possible expense of the rendered mesh noticeably lagging behind 
		 * the modifications which are being performed.
		 * 
		 * NOTE: This property is currently hidden from the user until we have a better understanding of how it should behave. For example, should
		 * that same value be used in edit mode vs. play mode? What if there is/isn't a collision mesh? Or what if we want to syncronize every 'x'
		 * updates rather than 'x' times per update?
		 */
		/// \cond
		protected int maxNodesPerSync = 4;
		/// \endcond

		// The root node of our octree. It is protected so that derived classes can use it, but users
		// are not supposed to create derived classes themselves so we hide this property from the docs.
		/// \cond
		protected GameObject rootOctreeNodeGameObject;
		/// \endcond
		
		private bool flushRequested;
		
		private int previousLayer = -1;
		
		void Awake()
		{
			if(rootOctreeNodeGameObject != null)
			{
				// This should not happen because the rootOctreeNodeGameObject should have been set to null before being serialized.
				Debug.LogWarning("Root octree node is already set. This is probably a bug in Cubiquity for Unity3D, but it is not serious.");
				FlushInternalData();
			}
		}
		
		void OnEnable()
		{			
			// When switching to MonoDevelop, editing code, and then switching back to Unity, some kind of scene reload is performed.
			// It's actually a bit unclear, but it results in a new octree being built without the old one being destroyed first. It
			// seems Awake/OnDestroy() are not called as part of this process, and we are not allowed to modify the scene graph from
			// OnEnable()/OnDisable(). Therefore we just set a flag to say that the root node shot be deleted at the next update cycle.
			//
			// We set the flag here (rather than OnDisable() where it might make more sense) because the flag doesn't survive the
			// script reload, and we don't really want to serialize it.
			RequestFlushInternalData();
			
			// When in edit mode a component's Update() function is not normally called, and even if the 'ExecuteInEditMode' attribute
			// is set then it executes only when something changes. For our purposes we need a continuous stream of updates in order to
			// handle background loading of the volume. Therefore we define a new function 'EditModeUpdate' and connect it to the editor's
			// update delegate.
			#if UNITY_EDITOR
				if(!EditorApplication.isPlaying)
				{
					EditorApplication.update += EditModeUpdate;
				}
				else
				{
					StartCoroutine(Synchronization());
				}
			#else
				StartCoroutine(Synchronization());
			#endif
		}
		
		void OnDisable()
		{			
			// Disconnect the edit-mode update. It will be reconnected in OnEnable() if we are in edit mode.
			// We don't need to stop the Syncronization() coroutine as this happens automatically:
			// http://answers.unity3d.com/questions/34169/does-deactivating-a-gameobject-automatically-stop.html
			#if UNITY_EDITOR
				EditorApplication.update -= EditModeUpdate;
			#endif
		}
		
		#if UNITY_EDITOR
		void EditModeUpdate()
		{
			// Just a sanity check to make sure our understanding of edit/play mode behaviour is correct.
			DebugUtils.Assert(!EditorApplication.isPlaying, "EditModeUpdate() is not expected to be executing in play mode!");
			
			Synchronize();
		}
		#endif
		
		// Protected so that derived classes can access it, but users derive their own classes so we hide it from the docs.
		/// \cond
		protected void RequestFlushInternalData()
		{
			flushRequested = true;
		}
		/// \endcond
		
		// We do not serialize the root octree node but in practice we have still seen some issues. It seems that Unity does
		// still serialize other data (meshes, etc) in the scene even though the root game object which they are a child of
		// is not serialize. Actually this needs more investigation. Problematic scenarios include when saving the scene, 
		// switching from edit mode to play mode (which includes implicit serialization), or when changing and recompiling scripts.
		//
		// To handle thee scenarios we need the ability to explicitly destroy the root node, rather than just not serializing it.
		private void FlushInternalData()
		{
			DestroyImmediate(rootOctreeNodeGameObject);
			rootOctreeNodeGameObject = null;
		}
		
		private IEnumerator Synchronization()
		{			
			// Perform the syncronization.
			while(true)
			{
				#if UNITY_EDITOR
					// Just a sanity check to make sure our understanding of edit/play mode behaviour is correct.
					DebugUtils.Assert(EditorApplication.isPlaying, "Synchronization coroutine is not expected to be executing in edit mode!");
				#endif
				
				Synchronize();
				yield return null;
			}
		}
		
		// Protected so that derived classes can access it, but users derive their own classes so we hide it from the docs.
		/// \cond
		protected virtual void Synchronize()
		{
			if(flushRequested)
			{
				FlushInternalData();
				flushRequested = false;
			}
			
			// Check whether the gameObject has been moved to a new layer.
			if(gameObject.layer != previousLayer)
			{
				// If so we update the children to match and then clear the flag.
				gameObject.SetLayerRecursively(gameObject.layer);
				previousLayer = gameObject.layer;
			}
			
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
		/// \endcond
		
		#if UNITY_EDITOR
			private class OnSaveHandler : UnityEditor.AssetModificationProcessor
			{
			    public static void OnWillSaveAssets( string[] assets )
			    {
					Object[] volumes = Object.FindObjectsOfType(typeof(Volume));
					foreach(Object volume in volumes)
					{
						((Volume)volume).FlushInternalData();
					}
			    }
			}
			
			[InitializeOnLoad]
			private class OnPlayHandler
			{
			    static OnPlayHandler()
			    {
					// Catch the event which occurs when switching modes.
			        EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
			    }
			 
			    static void OnPlaymodeStateChanged ()
			    {
					// We only need to discard the octree in edit mode, beacause when leaving play mode serialization is not
					// performed. This event occurs both when leaving the old mode and again when entering the new mode, but 
					// when entering edit mode the root null should already be null and ddiscarding it again is harmless.
					if(!EditorApplication.isPlaying)
					{
						Object[] volumes = Object.FindObjectsOfType(typeof(Volume));
						foreach(Object volume in volumes)
						{
							((Volume)volume).FlushInternalData();
						}
					}
			    }
			}
		#endif
	}
}