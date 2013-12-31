using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Cubiquity
{	
	[ExecuteInEditMode]
	public class TerrainVolume : MonoBehaviour
	{		
		// The name of the dataset to load from disk.
		[SerializeField]
		private TerrainVolumeData mData = null;
		public TerrainVolumeData data
	    {
	        get { return this.mData; }
	    }
		
		// Determines whether collision data is generated as well as a
		// renderable mesh. This does not apply when in the Unity editor.
		public bool UseCollisionMesh = true;
		
		// This corresponds to the root OctreeNode in Cubiquity.
		private GameObject rootGameObject;
		private GameObject ghostGameObject;
		
		private int maxNodeSyncsPerFrame = 4;
		private int nodeSyncsThisFrame = 0;
		
		public static GameObject CreateGameObject(TerrainVolumeData data)
		{			
			GameObject terrainVolumeGameObject = new GameObject("Terrain Volume");
			
			terrainVolumeGameObject.AddComponent<TerrainVolume>();
			terrainVolumeGameObject.AddComponent<TerrainVolumeRenderer>();
			
			TerrainVolume terrainVolume = terrainVolumeGameObject.GetComponent<TerrainVolume>();
			
			terrainVolume.mData = data;
			
			return terrainVolumeGameObject;
		}
		
		// It seems that we need to implement this function in order to make the volume pickable in the editor.
		// It's actually the gizmo which get's picked which is often bigger than than the volume (unless all
		// voxels are solid). So somtimes the volume will be selected by clicking on apparently empty space.
		// We shold try and fix this by using raycasting to check if a voxel is under the mouse cursor?
		void OnDrawGizmos()
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
			
			// Draw an invisible box surrounding the volume. This is what actually gets picked.
	        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
			Gizmos.DrawCube (transform.position - halfVoxelOffset + new Vector3(offsetX, offsetY, offsetZ), new Vector3 (width, height, depth));
	    }
		
		public void Synchronize()
		{
			Material material = gameObject.GetComponent<TerrainVolumeRenderer>().material;
			
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
			material.SetMatrix("_World2Volume", transform.worldToLocalMatrix);
			
			ghostGameObject.GetComponent<TerrainVolumeRenderer>().material = material;
			
			nodeSyncsThisFrame = 0;
			
			if(data.volumeHandle.HasValue)
			{
				CubiquityDLL.UpdateVolumeMC(data.volumeHandle.Value);
				
				if(CubiquityDLL.HasRootOctreeNodeMC(data.volumeHandle.Value) == 1)
				{		
					uint rootNodeHandle = CubiquityDLL.GetRootOctreeNodeMC(data.volumeHandle.Value);
					
					if(rootGameObject == null)
					{
						rootGameObject = OctreeNode.CreateOctreeNode(rootNodeHandle, ghostGameObject);	
					}
					
					OctreeNode rootOctreeNode = rootGameObject.GetComponent<OctreeNode>();
					int i = maxNodeSyncsPerFrame;
					rootOctreeNode.syncNode(ref i, UseCollisionMesh);
				}
			}
		}
		
		void OnEnable()
		{
			Debug.Log ("TerrainVolume.OnEnable()");
			
			if(ghostGameObject == null)
			{				
				ghostGameObject = new GameObject("Ghost");
				ghostGameObject.hideFlags = HideFlags.HideAndDontSave;
				ghostGameObject.AddComponent<TerrainVolumeRenderer>();
			}			
		}
		
		// Update is called once per frame
		void Update()
		{		
			Synchronize();
			
			if(transform.hasChanged)
			{
				ghostGameObject.transform.localPosition = transform.localPosition;
				ghostGameObject.transform.localRotation = transform.localRotation;
				ghostGameObject.transform.localScale = transform.localScale;
				transform.hasChanged = false;
			}
		}
		
		public void OnDisable()
		{
			// Game objects in our tree are created with the 'DontSave' flag set, and according to the Unity docs this means
			// we have to destroy them manually. In the case of 'Destroy' the Unity docs explicitally say that it will destroy
			// transform children as well, so I'm assuming DestroyImmediate has the same behaviour.
			DestroyImmediate(rootGameObject);
		}
	}
}
