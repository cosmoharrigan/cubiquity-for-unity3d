using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Cubiquity.Impl;

namespace Cubiquity
{	
	/// Allows environments to be built from millions of colored cubes
	/**
	 * The ColoredCubesVolume can be used to achieve retro-style environents which capture the visual essence of pixel art but extended into three
	 * dimensions. Large numbers of cubes can be added or removed each frame in order to create highly dynamic worlds.
	 * 
	 * \image html cubic-terrain.jpg
	 * 
	 * Conceptually this kind of volume is easy to understand. Access to the underlying voxel data can be obtained through the 'data' property,
	 * and this returns a instance of ColoredCubesVolumeData with each voxel of the data being a QuantizedColor. The ColoredCubesVolume and
	 * ColoredCubesVolumeData are used in conjunction with the ColoredCubesRenderer and ColoredCubesCollider. Please see the documentation of
	 * the Volume class for more details and a diagram showing how these components are related.
	 */
	[ExecuteInEditMode]
	public class ColoredCubesVolume : Volume
	{
		/**
		 * \copydoc Volume::data
		 */
		public new ColoredCubesVolumeData data
	    {
	        get { return (ColoredCubesVolumeData)base.data; }
			set { base.data = value; }
	    }
		
		/// Convinience method for creating a GameObject with a set of colored cubes components attached.
		/**
		 * Adding a volume to a scene requires creating a GameObject and then attching the required Cubiquity components such a renderer and a
		 * collider. This method simply automates the process and also attaches the provided volume data.
		 * 
		 * \param data The volume data which should be attached to the construced volume.
		 * \param addRenderer Specifies whether a renderer component should be added so that the volume is displayed.
		 * \param addCollider Specifies whether a collider component should be added so that the volume can participate in collisions.
		 */
		public static GameObject CreateGameObject(ColoredCubesVolumeData data, bool addRenderer, bool addCollider)
		{
			// Create our main game object representing the volume.
			GameObject coloredCubesVolumeGameObject = new GameObject("Colored Cubes Volume");
			
			//Add the required volume component.
			ColoredCubesVolume coloredCubesVolume = coloredCubesVolumeGameObject.GetOrAddComponent<ColoredCubesVolume>();
			
			// Set the provided data.
			coloredCubesVolume.data = data;
			
			// Add the renderer and collider if desired.
			if(addRenderer) { coloredCubesVolumeGameObject.AddComponent<ColoredCubesVolumeRenderer>(); }
			if(addCollider) { coloredCubesVolumeGameObject.AddComponent<ColoredCubesVolumeCollider>(); }
			
			// Return the created object
			return coloredCubesVolumeGameObject;
		}
		
		// It seems that we need to implement this function in order to make the volume pickable in the editor.
		// It's actually the gizmo which get's picked which is often bigger than than the volume (unless all
		// voxels are solid). So somtimes the volume will be selected by clicking on apparently empty space.
		// We shold try and fix this by using raycasting to check if a voxel is under the mouse cursor?
		void OnDrawGizmos()
		{
			if(data != null)
			{
				// Compute the size of the volume.
				int width = (data.enclosingRegion.upperCorner.x - data.enclosingRegion.lowerCorner.x) + 1;
				int height = (data.enclosingRegion.upperCorner.y - data.enclosingRegion.lowerCorner.y) + 1;
				int depth = (data.enclosingRegion.upperCorner.z - data.enclosingRegion.lowerCorner.z) + 1;
				float offsetX = width / 2;
				float offsetY = height / 2;
				float offsetZ = depth / 2;
				
				// The origin is at the centre of a voxel, but we want this box to start at the corner of the voxel.
				Vector3 halfVoxelOffset = new Vector3(0.5f, 0.5f, 0.5f);
				
				// Draw an invisible box surrounding the olume. This is what actually gets picked.
		        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
				Gizmos.DrawCube (transform.position - halfVoxelOffset + new Vector3(offsetX, offsetY, offsetZ), new Vector3 (width, height, depth));
			}
	    }
		
		/// \cond
		protected override void Synchronize()
		{			
			base.Synchronize();
			
			ColoredCubesVolumeRenderer volumeRenderer = gameObject.GetComponent<ColoredCubesVolumeRenderer>();
			if(volumeRenderer != null)
			{
				if(volumeRenderer.material != null)
				{		
					// We compute surface normals using derivative operations in the fragment shader, but for some reason
					// these are backwards on Linux. We can correct for this in the shader by setting the multiplier below.
					#if UNITY_STANDALONE_LINUX && !UNITY_EDITOR
						float normalMultiplier = -1.0f;
					#else
						float normalMultiplier = 1.0f;
					#endif					
					volumeRenderer.material.SetFloat("normalMultiplier", normalMultiplier);
				}
			}
			
			// Syncronize the mesh data.
			if(data != null)
			{
				// Syncronize the mesh data.
				if(data.volumeHandle.HasValue)
				{
					CubiquityDLL.UpdateVolume(data.volumeHandle.Value);
					
					if(CubiquityDLL.HasRootOctreeNode(data.volumeHandle.Value) == 1)
					{		
						uint rootNodeHandle = CubiquityDLL.GetRootOctreeNode(data.volumeHandle.Value);
						
						if(rootOctreeNodeGameObject == null)
						{
							rootOctreeNodeGameObject = OctreeNode.CreateOctreeNode(rootNodeHandle, gameObject);	
						}
						
						OctreeNode rootOctreeNode = rootOctreeNodeGameObject.GetComponent<OctreeNode>();
						int nodeSyncsPerformed = rootOctreeNode.syncNode(maxNodesPerSync, gameObject);
						
						// If no node were syncronized then the mesh data is up to
						// date and we can set the flag to convey this to the user.
						isMeshSyncronized = (nodeSyncsPerformed == 0);
					}
				}
			}
		}
		/// \endcond
	}
}
