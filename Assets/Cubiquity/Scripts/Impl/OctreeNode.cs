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
			
			newGameObject.hideFlags = HideFlags.HideAndDontSave;
			
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
					
					GameObject ghostGameObject = gameObject;
					do
					{
						ghostGameObject = ghostGameObject.transform.parent.gameObject;
						
					}while(ghostGameObject.GetComponent<GhostObjectSource>() == null);
						
					GameObject sourceGameObject = ghostGameObject.GetComponent<GhostObjectSource>().sourceGameObject;
					VolumeRenderer volumeRenderer = sourceGameObject.GetComponent<VolumeRenderer>();
					volumeRenderer.BuildMeshFromNodeHandle(nodeHandle, out renderingMesh, out physicsMesh, UseCollisionMesh);
			
			        MeshFilter mf = (MeshFilter)gameObject.GetComponent(typeof(MeshFilter));
			        MeshRenderer mr = (MeshRenderer)gameObject.GetComponent(typeof(MeshRenderer));
					
					if(mf.sharedMesh != null)
					{
						DestroyImmediate(mf.sharedMesh);
					}
					
			        mf.sharedMesh = renderingMesh;				
					
					//mr.material = new Material(Shader.Find("ColoredCubesVolume"));
					
					mr.material = volumeRenderer.material;
					
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
	}
}
