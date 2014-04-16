using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Text;

namespace Cubiquity
{
	namespace Impl
	{
		public class OctreeNode : MonoBehaviour
		{
			[System.NonSerialized]
			public uint meshLastSyncronised;
			[System.NonSerialized]
			public uint volumeRendererLastSyncronised;
			[System.NonSerialized]
			public Vector3 lowerCorner;
			[System.NonSerialized]
			public GameObject[,,] children;
			
			[System.NonSerialized]
			public uint nodeHandle;
			
			public static GameObject CreateOctreeNode(uint nodeHandle, GameObject parentGameObject)
			{			
				int xPos, yPos, zPos;
				//Debug.Log("Getting position for node handle = " + nodeHandle);
				CubiquityDLL.GetNodePosition(nodeHandle, out xPos, out yPos, out zPos);
				
				StringBuilder name = new StringBuilder("(" + xPos + ", " + yPos + ", " + zPos + ")");
				
				GameObject newGameObject = new GameObject(name.ToString ());
				newGameObject.AddComponent<OctreeNode>();
				
				OctreeNode octreeNode = newGameObject.GetComponent<OctreeNode>();
				octreeNode.lowerCorner = new Vector3(xPos, yPos, zPos);
				octreeNode.nodeHandle = nodeHandle;
				
				if(parentGameObject)
				{
					newGameObject.layer = parentGameObject.layer;
						
					newGameObject.transform.parent = parentGameObject.transform;
					newGameObject.transform.localPosition = new Vector3();
					newGameObject.transform.localRotation = new Quaternion();
					newGameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
					
					OctreeNode parentOctreeNode = parentGameObject.GetComponent<OctreeNode>();
					
					if(parentOctreeNode != null)
					{
						Vector3 parentLowerCorner = parentOctreeNode.lowerCorner;
						newGameObject.transform.localPosition = octreeNode.lowerCorner - parentLowerCorner;
					}
					else
					{
						newGameObject.transform.localPosition = octreeNode.lowerCorner;
					}
				}
				else
				{
					newGameObject.transform.localPosition = octreeNode.lowerCorner;
				}
				
				newGameObject.hideFlags = HideFlags.HideInHierarchy;
				
				return newGameObject;
			}
			
			public int syncNode(int availableNodeSyncs, GameObject voxelTerrainGameObject)
			{
				int nodeSyncsPerformed = 0;
				
				if(availableNodeSyncs <= 0)
				{
					return nodeSyncsPerformed;
				}
				
				uint meshLastUpdated = CubiquityDLL.GetMeshLastUpdated(nodeHandle);		
				
				if(meshLastSyncronised < meshLastUpdated)
				{			
					if(CubiquityDLL.NodeHasMesh(nodeHandle) == 1)
					{					
						// Set up the rendering mesh											
						Mesh renderingMesh = null;
						if(voxelTerrainGameObject.GetComponent<Volume>().GetType() == typeof(TerrainVolume))
						{
							renderingMesh = BuildMeshFromNodeHandleForTerrainVolume(nodeHandle);
						}
						else if(voxelTerrainGameObject.GetComponent<Volume>().GetType() == typeof(ColoredCubesVolume))
						{
							renderingMesh = BuildMeshFromNodeHandleForColoredCubesVolume(nodeHandle);
						}
				
				        MeshFilter meshFilter = gameObject.GetOrAddComponent<MeshFilter>() as MeshFilter;
						
						if(meshFilter.sharedMesh != null)
						{
							DestroyImmediate(meshFilter.sharedMesh);
						}
						
				        meshFilter.sharedMesh = renderingMesh;
						
						// Set up the collision mesh
						VolumeCollider volumeCollider = voxelTerrainGameObject.GetComponent<VolumeCollider>();					
						if((volumeCollider != null) && (Application.isPlaying))
						{
							Mesh collisionMesh = volumeCollider.BuildMeshFromNodeHandle(nodeHandle);
							MeshCollider meshCollider = gameObject.GetOrAddComponent<MeshCollider>() as MeshCollider;
							meshCollider.sharedMesh = collisionMesh;
						}
					}
					// If there is no mesh in Cubiquity then we make sure there isn't on in Unity.
					else
					{
						MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>() as MeshCollider;
						if(meshCollider)
						{
							DestroyImmediate(meshCollider);
						}
						
						MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>() as MeshRenderer;
						if(meshRenderer)
						{
							DestroyImmediate(meshRenderer);
						}
						
						MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>() as MeshFilter;
						if(meshFilter)
						{
							DestroyImmediate(meshFilter);
						}
					}
					
					meshLastSyncronised = CubiquityDLL.GetCurrentTime();
					availableNodeSyncs--;
					nodeSyncsPerformed++;
					
				}
				
				VolumeRenderer volumeRenderer = voxelTerrainGameObject.GetComponent<VolumeRenderer>();
				if(volumeRenderer != null)
				{	
					if(volumeRendererLastSyncronised < volumeRenderer.lastModified)
					{
				        MeshRenderer meshRenderer = gameObject.GetOrAddComponent<MeshRenderer>() as MeshRenderer;			
						
						meshRenderer.sharedMaterial = volumeRenderer.material;
						
						#if UNITY_EDITOR
						EditorUtility.SetSelectedWireframeHidden(meshRenderer, true);
						#endif
						
						volumeRendererLastSyncronised = Clock.timestamp;
					}
				}
				
				//Now syncronise any children
				for(uint z = 0; z < 2; z++)
				{
					for(uint y = 0; y < 2; y++)
					{
						for(uint x = 0; x < 2; x++)
						{
							if(CubiquityDLL.HasChildNode(nodeHandle, x, y, z) == 1)
							{					
							
								uint childNodeHandle = CubiquityDLL.GetChildNode(nodeHandle, x, y, z);					
								
								GameObject childGameObject = GetChild(x,y,z);
								
								if(childGameObject == null)
								{							
									childGameObject = OctreeNode.CreateOctreeNode(childNodeHandle, gameObject);
									
									SetChild(x, y, z, childGameObject);
								}
								
								//syncNode(childNodeHandle, childGameObject);
								
								OctreeNode childOctreeNode = childGameObject.GetComponent<OctreeNode>();
								int syncs = childOctreeNode.syncNode(availableNodeSyncs, voxelTerrainGameObject);
								availableNodeSyncs -= syncs;
								nodeSyncsPerformed += syncs;
							}
						}
					}
				}
				
				return nodeSyncsPerformed;
			}
			
			public GameObject GetChild(uint x, uint y, uint z)
			{
				if(children != null)
				{
					return children[x, y, z];
				}
				else
				{
					return null;
				}
			}
			
			public void SetChild(uint x, uint y, uint z, GameObject gameObject)
			{
				if(children == null)
				{
					children = new GameObject[2, 2, 2];
				}
				
				children[x, y, z] = gameObject;
			}
			
			public Mesh BuildMeshFromNodeHandleForTerrainVolume(uint nodeHandle)
			{
				// At some point I should read this: http://forum.unity3d.com/threads/5687-C-plugin-pass-arrays-from-C
				
				// Create rendering and possible collision meshes.
				Mesh renderingMesh = new Mesh();		
				renderingMesh.hideFlags = HideFlags.DontSave;
	
				// Get the data from Cubiquity.
				int[] indices = CubiquityDLL.GetIndicesMC(nodeHandle);		
				TerrainVertex[] cubiquityVertices = CubiquityDLL.GetVerticesMC(nodeHandle);			
				
				// Create the arrays which we'll copy the data to.
		        Vector3[] renderingVertices = new Vector3[cubiquityVertices.Length];		
				Vector3[] renderingNormals = new Vector3[cubiquityVertices.Length];		
				Color32[] renderingColors = new Color32[cubiquityVertices.Length];	
				//Vector4[] renderingTangents = new Vector4[cubiquityVertices.Length];		
				Vector2[] renderingUV = new Vector2[cubiquityVertices.Length];
				Vector2[] renderingUV2 = new Vector2[cubiquityVertices.Length];
				
				
				for(int ct = 0; ct < cubiquityVertices.Length; ct++)
				{
					// Get the vertex data from Cubiquity.
					Vector3 position = new Vector3(cubiquityVertices[ct].x, cubiquityVertices[ct].y, cubiquityVertices[ct].z);
					Vector3 normal = new Vector3(cubiquityVertices[ct].nx, cubiquityVertices[ct].ny, cubiquityVertices[ct].nz);
					Color32 color = new Color32(cubiquityVertices[ct].m0, cubiquityVertices[ct].m1, cubiquityVertices[ct].m2, cubiquityVertices[ct].m3);
					//Vector4 tangents = new Vector4(cubiquityVertices[ct].m4 / 255.0f, cubiquityVertices[ct].m5 / 255.0f, cubiquityVertices[ct].m6 / 255.0f, cubiquityVertices[ct].m7 / 255.0f);
					Vector2 uv = new Vector2(cubiquityVertices[ct].m4 / 255.0f, cubiquityVertices[ct].m5 / 255.0f);
					Vector2 uv2 = new Vector2(cubiquityVertices[ct].m6 / 255.0f, cubiquityVertices[ct].m7 / 255.0f);
						
					// Copy it to the arrays.
					renderingVertices[ct] = position;	
					renderingNormals[ct] = normal;
					renderingColors[ct] = color;
					//renderingTangents[ct] = tangents;
					renderingUV[ct] = uv;
					renderingUV2[ct] = uv2;
				}
				
				// Assign vertex data to the meshes.
				renderingMesh.vertices = renderingVertices; 
				renderingMesh.normals = renderingNormals;
				renderingMesh.colors32 = renderingColors;
				//renderingMesh.tangents = renderingTangents;
				renderingMesh.uv = renderingUV;
				renderingMesh.uv2 = renderingUV2;
				renderingMesh.triangles = indices;
				
				// FIXME - Get proper bounds
				renderingMesh.bounds.SetMinMax(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(32.0f, 32.0f, 32.0f));
				
				return renderingMesh;
			}
			
			public Mesh BuildMeshFromNodeHandleForColoredCubesVolume(uint nodeHandle)
			{
				// At some point I should read this: http://forum.unity3d.com/threads/5687-C-plugin-pass-arrays-from-C
					
				// Create rendering and possible collision meshes.
				Mesh renderingMesh = new Mesh();		
				renderingMesh.hideFlags = HideFlags.DontSave;
	
				// Get the data from Cubiquity.
				int[] indices = CubiquityDLL.GetIndices(nodeHandle);		
				ColoredCubesVertex[] cubiquityVertices = CubiquityDLL.GetVertices(nodeHandle);			
				
				// Create the arrays which we'll copy the data to.
		        Vector3[] renderingVertices = new Vector3[cubiquityVertices.Length];	
				Color32[] renderingColors = new Color32[cubiquityVertices.Length];
				
				for(int ct = 0; ct < cubiquityVertices.Length; ct++)
				{
					// Get the vertex data from Cubiquity.
					Vector3 position = new Vector3(cubiquityVertices[ct].x, cubiquityVertices[ct].y, cubiquityVertices[ct].z);
					QuantizedColor color = cubiquityVertices[ct].color;
						
					// Copy it to the arrays.
					renderingVertices[ct] = position;
					renderingColors[ct] = (Color32)color;
				}
				
				// Assign vertex data to the meshes.
				renderingMesh.vertices = renderingVertices; 
				renderingMesh.colors32 = renderingColors;
				renderingMesh.triangles = indices;
				
				// FIXME - Get proper bounds
				renderingMesh.bounds.SetMinMax(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(32.0f, 32.0f, 32.0f));
				
				return renderingMesh;
			}
		}
	}
}
