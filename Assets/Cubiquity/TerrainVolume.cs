﻿using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Cubiquity
{
	public struct CubiquitySmoothVertex 
	{
		// Disable 'Field ... is never assigned to'
		// warnings as this structure is just for interop
		#pragma warning disable 0649
		public float x;
		public float y;
		public float z;
		public float nx;
		public float ny;
		public float nz;
		public byte m0;
		public byte m1;
		public byte m2;
		public byte m3;
		#pragma warning restore 0649
	}
	
	[ExecuteInEditMode]
	public class TerrainVolume : MonoBehaviour
	{		
		// The name of the dataset to load from disk.
		public TerrainVolumeData data = null;
		
		// The side length of an extracted mesh for the most detailed LOD.
		// Bigger values mean fewer batches but slower surface extraction.
		// For some reason Unity won't serialize uints so it's stored as int.
		//public int baseNodeSize = 0;
		
		// Determines whether collision data is generated as well as a
		// renderable mesh. This does not apply when in the Unity editor.
		public bool UseCollisionMesh = true;
		
		public Material material; //FIXME - should probably be internal? Visible to the editor so it can set the brush params
		
		// This corresponds to the root OctreeNode in Cubiquity.
		private GameObject rootGameObject;
		
		private int maxNodeSyncsPerFrame = 4;
		private int nodeSyncsThisFrame = 0;
		
		public static GameObject CreateGameObject(TerrainVolumeData data)
		{
			// Make sure the Cubiquity library is installed.
			Installation.ValidateAndFix();
			
			GameObject VoxelTerrainRoot = new GameObject("Terrain Volume");
			VoxelTerrainRoot.AddComponent<TerrainVolume>();
			
			TerrainVolume terrainVolume = VoxelTerrainRoot.GetComponent<TerrainVolume>();
			//terrainVolume.baseNodeSize = DefaultBaseNodeSize;
			
			terrainVolume.data = data;
			
			terrainVolume.Initialize();
			
			return VoxelTerrainRoot;
		}
		
		// It seems that we need to implement this function in order to make the volume pickable in the editor.
		// It's actually the gizmo which get's picked which is often bigger than than the volume (unless all
		// voxels are solid). So somtimes the volume will be selected by clicking on apparently empty space.
		// We shold try and fix this by using raycasting to check if a voxel is under the mouse cursor?
		void OnDrawGizmos()
		{
			// Compute the size of the volume.
			int width = (data.region.upperCorner.x - data.region.lowerCorner.x) + 1;
			int height = (data.region.upperCorner.y - data.region.lowerCorner.y) + 1;
			int depth = (data.region.upperCorner.z - data.region.lowerCorner.z) + 1;
			float offsetX = width / 2;
			float offsetY = height / 2;
			float offsetZ = depth / 2;
			
			// The origin is at the centre of a voxel, but we want this box to start at the corner of the voxel.
			Vector3 halfVoxelOffset = new Vector3(0.5f, 0.5f, 0.5f);
			
			// Draw an invisible box surrounding the volume. This is what actually gets picked.
	        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
			Gizmos.DrawCube (transform.position - halfVoxelOffset + new Vector3(offsetX, offsetY, offsetZ), new Vector3 (width, height, depth));
	    }
		
		internal void Initialize()
		{	
			// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
			// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.
			/*if(data != null)
			{	
				if(data.volumeHandle == null)
				{
					// Create an empty region of the desired size.
					data.volumeHandle = CubiquityDLL.NewTerrainVolume(data.region.lowerCorner.x, data.region.lowerCorner.y, data.region.lowerCorner.z,
						data.region.upperCorner.x, data.region.upperCorner.y, data.region.upperCorner.z, data.pathToVoxels, (uint)baseNodeSize, 0, 0);
				}
			}*/
		}
		
		internal void InitializeWithFloor(uint floorDepth)
		{	
			// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
			// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.
			/*if(data.volumeHandle == null)
			{	
				if(data != null)
				{
					// Create an empty region of the desired size.
					data.volumeHandle = CubiquityDLL.NewTerrainVolume(data.region.lowerCorner.x, data.region.lowerCorner.y, data.region.lowerCorner.z,
						data.region.upperCorner.x, data.region.upperCorner.y, data.region.upperCorner.z, data.pathToVoxels, (uint)baseNodeSize, 1, floorDepth);
					
					CubiquityDLL.GenerateFloor(data.volumeHandle.Value, (int)floorDepth - 2, (uint)0, (int)floorDepth, (uint)1);
				}
			}*/
		}
		
		public void Synchronize()
		{
			nodeSyncsThisFrame = 0;
			
			if(data.volumeHandle.HasValue)
			{
				CubiquityDLL.UpdateVolumeMC(data.volumeHandle.Value);
				
				if(CubiquityDLL.HasRootOctreeNodeMC(data.volumeHandle.Value) == 1)
				{		
					uint rootNodeHandle = CubiquityDLL.GetRootOctreeNodeMC(data.volumeHandle.Value);
				
					if(rootGameObject == null)
					{					
						rootGameObject = BuildGameObjectFromNodeHandle(rootNodeHandle, null);	
					}
					syncNode(rootNodeHandle, rootGameObject);
				}
				
				// We syncronise all the material properties every time. If we find this has some
				// performance overhead then we could add an 'isModified' flag to each terrain material.
				for(int i = 0; i < data.materials.Length; i++)
				{
					material.SetTexture("_Tex" + i, data.materials[i].diffuseMap);
					
					Vector3 invScale;
					invScale.x = 1.0f / data.materials[i].scale.x;
					invScale.y = 1.0f / data.materials[i].scale.y;
					invScale.z = 1.0f / data.materials[i].scale.z;
					material.SetVector("_TexInvScale" + i, invScale);
					
					material.SetVector("_TexOffset" + i, data.materials[i].offset);
				}
			}
		}
		
		public void Shutdown(bool saveChanges)
		{
			Debug.Log("In ColoredCubesVolume.Shutdown()");
			
			if(data.volumeHandle.HasValue)
			{
				if(saveChanges)
				{
					CubiquityDLL.AcceptOverrideBlocksMC(data.volumeHandle.Value);
				}
				CubiquityDLL.DiscardOverrideBlocksMC(data.volumeHandle.Value);
				
				CubiquityDLL.DeleteTerrainVolume(data.volumeHandle.Value);
				data.volumeHandle = null;
				
				// Game objects in our tree are created with the 'DontSave' flag set, and according to the Unity docs this means
				// we have to destroy them manually. In the case of 'Destroy' the Unity docs explicitally say that it will destroy
				// transform children as well, so I'm assuming DestroyImmediate has the same behaviour.
				DestroyImmediate(rootGameObject);
			}
		}
		
		void OnEnable()
		{
			Debug.Log ("ColoredCubesVolume.OnEnable()");
			Shader shader = Shader.Find("TerrainVolume");
			material = new Material(shader);
			
			Initialize();
		}
		
		// Use this for initialization
		/*void Start()
		{		
			
		}*/
		
		// Update is called once per frame
		void Update()
		{
			Synchronize();
		}
		
		public void OnDisable()
		{
			Debug.Log ("ColoredCubesVolume.OnDisable()");
			
			// We only save if we are in editor mode, not if we are playing.
			bool saveChanges = !Application.isPlaying;
			
			Shutdown(saveChanges);
		}
		
		public void syncNode(uint nodeHandle, GameObject gameObjectToSync)
		{
			if(nodeSyncsThisFrame >= maxNodeSyncsPerFrame)
			{
				return;
			}
			
			uint meshLastUpdated = CubiquityDLL.GetMeshLastUpdatedMC(nodeHandle);		
			OctreeNodeData octreeNodeData = (OctreeNodeData)(gameObjectToSync.GetComponent<OctreeNodeData>());
			
			if(octreeNodeData.meshLastSyncronised < meshLastUpdated)
			{			
				Debug.Log ("Syncronising mesh");
				if(CubiquityDLL.NodeHasMeshMC(nodeHandle) == 1)
				{				
					Mesh renderingMesh;
					Mesh physicsMesh;
					
					BuildMeshFromNodeHandle(nodeHandle, out renderingMesh, out physicsMesh);
			
			        MeshFilter mf = (MeshFilter)gameObjectToSync.GetComponent(typeof(MeshFilter));
			        MeshRenderer mr = (MeshRenderer)gameObjectToSync.GetComponent(typeof(MeshRenderer));
					
					if(mf.sharedMesh != null)
					{
						DestroyImmediate(mf.sharedMesh);
					}
					
			        mf.sharedMesh = renderingMesh;				
					
					mr.material = material;
					mr.sharedMaterial = material;
					/*for(int i = 0; i < materials.Length; i++)
					{
						if(materials[i] != null)
						{
							string texName = "_Tex" + i;
							material.SetTexture(texName, materials[i].diffuseMap);
						}
					}*/
					
					/*if(UseCollisionMesh)
					{
						MeshCollider mc = (MeshCollider)gameObjectToSync.GetComponent(typeof(MeshCollider));
						mc.sharedMesh = physicsMesh;
					}*/
				}
				
				uint currentTime = CubiquityDLL.GetCurrentTime();
				octreeNodeData.meshLastSyncronised = (int)(currentTime);
				
				nodeSyncsThisFrame++;
			}		
			
			//Now syncronise any children
			for(uint z = 0; z < 2; z++)
			{
				for(uint y = 0; y < 2; y++)
				{
					for(uint x = 0; x < 2; x++)
					{
						if(CubiquityDLL.HasChildNodeMC(nodeHandle, x, y, z) == 1)
						{					
						
							uint childNodeHandle = CubiquityDLL.GetChildNodeMC(nodeHandle, x, y, z);					
							
							GameObject childGameObject = octreeNodeData.GetChild(x,y,z);
							
							if(childGameObject == null)
							{							
								childGameObject = BuildGameObjectFromNodeHandle(childNodeHandle, gameObjectToSync);
								
								octreeNodeData.SetChild(x, y, z, childGameObject);
							}
							
							syncNode(childNodeHandle, childGameObject);
						}
					}
				}
			}
		}
		
		GameObject BuildGameObjectFromNodeHandle(uint nodeHandle, GameObject parentGameObject)
		{
			int xPos, yPos, zPos;
			//Debug.Log("Getting position for node handle = " + nodeHandle);
			CubiquityDLL.GetNodePositionMC(nodeHandle, out xPos, out yPos, out zPos);
			
			StringBuilder name = new StringBuilder("(" + xPos + ", " + yPos + ", " + zPos + ")");
			
			GameObject newGameObject = new GameObject(name.ToString ());
			newGameObject.AddComponent<OctreeNodeData>();
			//FIXME - Should we really add these here? Or once we determine we actually have meshes?
			newGameObject.AddComponent<MeshFilter>();
			newGameObject.AddComponent<MeshRenderer>();
			newGameObject.AddComponent<MeshCollider>();
			
			OctreeNodeData octreeNodeData = newGameObject.GetComponent<OctreeNodeData>();
			octreeNodeData.lowerCorner = new Vector3(xPos, yPos, zPos);
			
			if(parentGameObject)
			{
				newGameObject.transform.parent = parentGameObject.transform;
				
				Vector3 parentLowerCorner = parentGameObject.GetComponent<OctreeNodeData>().lowerCorner;
				newGameObject.transform.localPosition = octreeNodeData.lowerCorner - parentLowerCorner;
			}
			else
			{
				newGameObject.transform.localPosition = octreeNodeData.lowerCorner;
			}
			
			newGameObject.hideFlags = HideFlags.DontSave;
			
			return newGameObject;
		}
		
		void BuildMeshFromNodeHandle(uint nodeHandle, out Mesh renderingMesh, out Mesh physicsMesh)
		{
			// At some point I should read this: http://forum.unity3d.com/threads/5687-C-plugin-pass-arrays-from-C
			
			// Create rendering and possible collision meshes.
			renderingMesh = new Mesh();		
			physicsMesh = UseCollisionMesh ? new Mesh() : null;
			
			// Get the data from Cubiquity.
			int[] indices = CubiquityDLL.GetIndicesMC(nodeHandle);		
			CubiquitySmoothVertex[] cubiquityVertices = CubiquityDLL.GetVerticesMC(nodeHandle);			
			
			// Create the arrays which we'll copy the data to.
	        Vector3[] renderingVertices = new Vector3[cubiquityVertices.Length];		
			Vector3[] renderingNormals = new Vector3[cubiquityVertices.Length];		
			Color32[] renderingColors = new Color32[cubiquityVertices.Length];		
			Vector3[] physicsVertices = UseCollisionMesh ? new Vector3[cubiquityVertices.Length] : null;
			
			Debug.Log ("Got " + cubiquityVertices.Length + " vertices");
			
			for(int ct = 0; ct < cubiquityVertices.Length; ct++)
			{
				// Get the vertex data from Cubiquity.
				Vector3 position = new Vector3(cubiquityVertices[ct].x, cubiquityVertices[ct].y, cubiquityVertices[ct].z);
				Vector3 normal = new Vector3(cubiquityVertices[ct].nx, cubiquityVertices[ct].ny, cubiquityVertices[ct].nz);
				Color32 color = new Color32(cubiquityVertices[ct].m0, cubiquityVertices[ct].m1, cubiquityVertices[ct].m2, cubiquityVertices[ct].m3);
				//UInt32 colour = cubiquityVertices[ct].colour;
				
				// Pack it for efficient vertex buffer usage.
				//float packedPosition = packPosition(position);
				//float packedColor = packColor(colour);
					
				// Copy it to the arrays.
				renderingVertices[ct] = position;	
				renderingNormals[ct] = normal;
				renderingColors[ct] = color;
				if(UseCollisionMesh)
				{
					physicsVertices[ct] = position;
				}
			}
			
			// Assign vertex data to the meshes.
			renderingMesh.vertices = renderingVertices; 
			renderingMesh.normals = renderingNormals;
			renderingMesh.colors32 = renderingColors;
			renderingMesh.triangles = indices;
			
			// FIXME - Get proper bounds
			renderingMesh.bounds = new Bounds(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(500.0f, 500.0f, 500.0f));
			
			if(UseCollisionMesh)
			{
				physicsMesh.vertices = physicsVertices;
				physicsMesh.triangles = indices;
			}
		}
	}
}
