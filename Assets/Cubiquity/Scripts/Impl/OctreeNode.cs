using UnityEngine;
using System.Collections;
using System.Text;

namespace Cubiquity
{
	public class OctreeNode : MonoBehaviour
	{
		[System.NonSerialized]
		public int meshLastSyncronised;
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
			newGameObject.AddComponent<MeshFilter>();
			newGameObject.AddComponent<MeshRenderer>();
			newGameObject.AddComponent<MeshCollider>();
			
			OctreeNode octreeNode = newGameObject.GetComponent<OctreeNode>();
			octreeNode.lowerCorner = new Vector3(xPos, yPos, zPos);
			octreeNode.nodeHandle = nodeHandle;
			
			if(parentGameObject)
			{
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
			
			newGameObject.hideFlags = HideFlags.DontSave;
			
			return newGameObject;
		}
		
		public void syncNode(ref int availableNodeSyncs, bool UseCollisionMesh)
		{
			if(availableNodeSyncs <= 0)
			{
				return;
			}
			
			uint meshLastUpdated = CubiquityDLL.GetMeshLastUpdated(nodeHandle);		
			
			if(meshLastSyncronised < meshLastUpdated)
			{			
				if(CubiquityDLL.NodeHasMesh(nodeHandle) == 1)
				{				
					Mesh renderingMesh;
					Mesh physicsMesh;
					
					BuildMeshFromNodeHandle(out renderingMesh, out physicsMesh, UseCollisionMesh);
			
			        MeshFilter mf = (MeshFilter)gameObject.GetComponent(typeof(MeshFilter));
			        MeshRenderer mr = (MeshRenderer)gameObject.GetComponent(typeof(MeshRenderer));
					
					if(mf.sharedMesh != null)
					{
						DestroyImmediate(mf.sharedMesh);
					}
					
			        mf.sharedMesh = renderingMesh;				
					
					mr.material = new Material(Shader.Find("ColoredCubesVolume"));
					
					if(UseCollisionMesh)
					{
						MeshCollider mc = (MeshCollider)gameObject.GetComponent(typeof(MeshCollider));
						mc.sharedMesh = physicsMesh;
					}
				}
				
				uint currentTime = CubiquityDLL.GetCurrentTime();
				meshLastSyncronised = (int)(currentTime);
				availableNodeSyncs--;
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
							childOctreeNode.syncNode(ref availableNodeSyncs, UseCollisionMesh);
						}
					}
				}
			}
		}
		
		void BuildMeshFromNodeHandle(out Mesh renderingMesh, out Mesh physicsMesh, bool UseCollisionMesh)
		{
			// At some point I should read this: http://forum.unity3d.com/threads/5687-C-plugin-pass-arrays-from-C
			
			// Create rendering and possible collision meshes.
			renderingMesh = new Mesh();		
			physicsMesh = UseCollisionMesh ? new Mesh() : null;
			
			// Get the data from Cubiquity.
			int[] indices = CubiquityDLL.GetIndices(nodeHandle);		
			CubiquityVertex[] cubiquityVertices = CubiquityDLL.GetVertices(nodeHandle);			
			
			// Create the arrays which we'll copy the data to.
	        Vector3[] renderingVertices = new Vector3[cubiquityVertices.Length];	
			Color32[] renderingColors = new Color32[cubiquityVertices.Length];
			Vector3[] physicsVertices = UseCollisionMesh ? new Vector3[cubiquityVertices.Length] : null;
			
			for(int ct = 0; ct < cubiquityVertices.Length; ct++)
			{
				// Get the vertex data from Cubiquity.
				Vector3 position = new Vector3(cubiquityVertices[ct].x, cubiquityVertices[ct].y, cubiquityVertices[ct].z);
				QuantizedColor color = cubiquityVertices[ct].color;
					
				// Copy it to the arrays.
				renderingVertices[ct] = position;
				renderingColors[ct] = (Color32)color;
				
				if(UseCollisionMesh)
				{
					physicsVertices[ct] = position;
				}
			}
			
			// Assign vertex data to the meshes.
			renderingMesh.vertices = renderingVertices; 
			renderingMesh.colors32 = renderingColors;
			renderingMesh.triangles = indices;
			
			// FIXME - Get proper bounds
			renderingMesh.bounds = new Bounds(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(500.0f, 500.0f, 500.0f));
			
			//if(UseCollisionMesh)
			{
				physicsMesh.vertices = physicsVertices;
				physicsMesh.triangles = indices;
			}
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
		
		/*public void OnDisable()
		{
			Debug.Log ("OctreeNode.OnDisable()");
			gameObject.transform.parent = null;
		}
		
		public void OnDestroy()
		{
			gameObject.transform.parent = null;
		}*/
	}
}
