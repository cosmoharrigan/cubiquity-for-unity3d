using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Cubiquity
{
	public struct CubiquityVertex 
	{
		// Disable 'Field ... is never assigned to'
		// warnings as this structure is just for interop
		#pragma warning disable 0649
		public float x;
		public float y;
		public float z;
		public QuantizedColor color;
		#pragma warning restore 0649
	}
	
	[ExecuteInEditMode]
	public class ColoredCubesVolume : MonoBehaviour
	{	
		// The name of the dataset to load from disk.
		[SerializeField]
		private ColoredCubesVolumeData mData = null;
		public ColoredCubesVolumeData data
	    {
	        get { return this.mData; }
			set
			{
				this.mData = value;
				//OnDisable();
			}
	    }
		
		// Determines whether collision data is generated as well as a
		// renderable mesh. This does not apply when in the Unity editor.
		public bool UseCollisionMesh = true;
		
		// This corresponds to the root OctreeNode in Cubiquity.
		private GameObject rootGameObject;
		private GameObject ghostGameObject;
		
		private int maxNodeSyncsPerFrame = 4;
		
		public static GameObject CreateGameObject(ColoredCubesVolumeData data)
		{			
			GameObject VoxelTerrainRoot = new GameObject("Colored Cubes Volume");
			VoxelTerrainRoot.AddComponent<ColoredCubesVolume>();
			
			ColoredCubesVolume coloredCubesVolume = VoxelTerrainRoot.GetComponent<ColoredCubesVolume>();
			
			coloredCubesVolume.mData = data;
			
			return VoxelTerrainRoot;
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
			
			// Draw an invisible box surrounding the olume. This is what actually gets picked.
	        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
			Gizmos.DrawCube (transform.position - halfVoxelOffset + new Vector3(offsetX, offsetY, offsetZ), new Vector3 (width, height, depth));
	    }
		
		public void Synchronize()
		{			
			if(data.volumeHandle.HasValue)
			{
				CubiquityDLL.UpdateVolume(data.volumeHandle.Value);
				
				if(CubiquityDLL.HasRootOctreeNode(data.volumeHandle.Value) == 1)
				{		
					uint rootNodeHandle = CubiquityDLL.GetRootOctreeNode(data.volumeHandle.Value);
				
					if(ghostGameObject == null)
					{				
						ghostGameObject = new GameObject("Ghost");
						ghostGameObject.hideFlags = HideFlags.HideAndDontSave;
					}
					
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
			Debug.Log ("ColoredCubesVolume.OnEnable()");
		}
		
		// Update is called once per frame
		void Update()
		{
			Synchronize();
			
			ghostGameObject.transform.localPosition = transform.localPosition;
			ghostGameObject.transform.localRotation = transform.localRotation;
			ghostGameObject.transform.localScale = transform.localScale;
		}
		
		public void OnDisable()
		{
			Debug.Log ("ColoredCubesVolume.OnDisable()");
			
			
		}
		
		public void OnDestroy()
		{
			Debug.Log ("ColoredCubesVolume.OnDestroy()");
			
			// Game objects in our tree are created with the 'DontSave' flag set, and according to the Unity docs this means
			// we have to destroy them manually. In the case of 'Destroy' the Unity docs explicitally say that it will destroy
			// transform children as well, so I'm assuming DestroyImmediate has the same behaviour.
			DestroyImmediate(ghostGameObject);
		}
	}
}
