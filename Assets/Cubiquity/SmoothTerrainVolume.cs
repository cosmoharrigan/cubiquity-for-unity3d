using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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
public class SmoothTerrainVolume : MonoBehaviour
{		
	// The name of the dataset to load from disk. A folder with this name
	// should be found in the location specified by 'Cubiquity.volumesPath'.
	public string datasetName = null;
	
	// The side length of an extracted mesh for the most detailed LOD.
	// Bigger values mean fewer batches but slower surface extraction.
	// For some reason Unity won't serialize uints so it's stored as int.
	public int baseNodeSize = 0;
	
	// Determines whether collision data is generated as well as a
	// renderable mesh. This does not apply when in the Unity editor.
	public bool UseCollisionMesh = true;
	
	// The extents (dimensions in voxels) of the volume.
	public Region region = null;
	
	public TerrainMaterial[] materials = new TerrainMaterial[License.MaxNoOfMaterials];
	
	public Material material; //FIXME - should probably  be internal? Visible to the editor so it can set the brush params
	
	// If set, this identifies the volume to the Cubiquity DLL. It can
	// be tested against null to find if the volume is currently valid.
	[System.NonSerialized]
	internal uint? volumeHandle = null;
	
	// This corresponds to the root OctreeNode in Cubiquity.
	private GameObject rootGameObject;
	
	private int maxNodeSyncsPerFrame = 4;
	private int nodeSyncsThisFrame = 0;
	
	// It seems that we need to implement this function in order to make the volume pickable in the editor.
	// It's actually the gizmo which get's picked which is often bigger than than the volume (unless all
	// voxels are solid). So somtimes the volume will be selected by clicking on apparently empty space.
	// We shold try and fix this by using raycasting to check if a voxel is under the mouse cursor?
	void OnDrawGizmos()
	{
		// Compute the size of the volume.
		int width = (region.upperCorner.x - region.lowerCorner.x) + 1;
		int height = (region.upperCorner.y - region.lowerCorner.y) + 1;
		int depth = (region.upperCorner.z - region.lowerCorner.z) + 1;
		float offsetX = width / 2;
		float offsetY = height / 2;
		float offsetZ = depth / 2;
		
		// The origin is at the centre of a voxel, but we want this box to start at the corner of the voxel.
		Vector3 halfVoxelOffset = new Vector3(0.5f, 0.5f, 0.5f);
		
		// Draw an invisible box surrounding the olume. This is what actually gets picked.
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
		Gizmos.DrawCube (transform.position - halfVoxelOffset + new Vector3(offsetX, offsetY, offsetZ), new Vector3 (width, height, depth));
    }
	
	internal void Initialize()
	{	
		// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
		// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.
		if(volumeHandle == null)
		{	
			if(region != null)
			{
				// Create an empty region of the desired size.
				volumeHandle = CubiquityDLL.NewSmoothTerrainVolume(region.lowerCorner.x, region.lowerCorner.y, region.lowerCorner.z,
					region.upperCorner.x, region.upperCorner.y, region.upperCorner.z, Cubiquity.volumesPath + Path.DirectorySeparatorChar + datasetName + Path.DirectorySeparatorChar, (uint)baseNodeSize, 0, 0);
			}
		}
	}
	
	internal void InitializeWithFloor(uint floorDepth)
	{	
		// This function might get called multiple times. E.g the user might call it striaght after crating the volume (so
		// they can add some initial data to the volume) and it might then get called again by OnEnable(). Handle this safely.
		if(volumeHandle == null)
		{	
			if(region != null)
			{
				// Create an empty region of the desired size.
				volumeHandle = CubiquityDLL.NewSmoothTerrainVolume(region.lowerCorner.x, region.lowerCorner.y, region.lowerCorner.z,
					region.upperCorner.x, region.upperCorner.y, region.upperCorner.z, Cubiquity.volumesPath + Path.DirectorySeparatorChar + datasetName + Path.DirectorySeparatorChar, (uint)baseNodeSize, 1, floorDepth);
			}
		}
	}
	
	public void Synchronize()
	{
		nodeSyncsThisFrame = 0;
		
		if(volumeHandle.HasValue)
		{
			CubiquityDLL.UpdateVolumeMC(volumeHandle.Value);
			
			if(CubiquityDLL.HasRootOctreeNodeMC(volumeHandle.Value) == 1)
			{		
				uint rootNodeHandle = CubiquityDLL.GetRootOctreeNodeMC(volumeHandle.Value);
			
				if(rootGameObject == null)
				{					
					rootGameObject = BuildGameObjectFromNodeHandle(rootNodeHandle, null);	
				}
				syncNode(rootNodeHandle, rootGameObject);
			}
			
			//Is there some impact to doing this every frame?
			for(int i = 0; i < materials.Length; i++)
			{
				if(materials[i] != null)
				{
					string texName = "_Tex" + i;
					material.SetTexture(texName, materials[i].diffuseMap);
				}
			}
		}
	}
	
	public void Shutdown(bool saveChanges)
	{
		Debug.Log("In ColoredCubesVolume.Shutdown()");
		
		if(volumeHandle.HasValue)
		{
			CubiquityDLL.DeleteSmoothTerrainVolume(volumeHandle.Value);
			volumeHandle = null;
			
			// Now that we've destroyed the volume handle, and volume data will have been paged into the override folder. This
			// includes any potential changes to the volume. If the user wanted to save this then copy it to the main page folder
			if(saveChanges)
			{
				foreach(var file in Directory.GetFiles(Cubiquity.volumesPath + Path.DirectorySeparatorChar + datasetName + "/override"))
				{
					File.Copy(file, Path.Combine(Cubiquity.volumesPath + Path.DirectorySeparatorChar + datasetName + Path.DirectorySeparatorChar, Path.GetFileName(file)), true);
				}
			}
			
			// Delete all the data in override
			// FIXME - Should probably check for a file extension.
			System.IO.DirectoryInfo overrideDirectory = new DirectoryInfo(Cubiquity.volumesPath + Path.DirectorySeparatorChar + datasetName + "/override");
			foreach (FileInfo file in overrideDirectory.GetFiles())
			{
				file.Delete();
			}
			
			// Game objects in our tree are created with the 'DontSave' flag set, and according to the Unity docs this means
			// we have to destroy them manually. In the case of 'Destroy' the Unity docs explicitally say that it will destroy
			// transform children as well, so I'm assuming DestroyImmediate has the same behaviour.
			DestroyImmediate(rootGameObject);
		}
	}
	
	void OnEnable()
	{
		Debug.Log ("ColoredCubesVolume.OnEnable()");
		Shader shader = Shader.Find("SmoothTerrainVolume");
		material = new Material(shader);
		
		/*for(int i = 0; i < License.MaxNoOfMaterials; i++)
		{
			if(materials[i] == null)
			{
				materials[i] = new TerrainMaterial();
			}
		}*/
		
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
	
	public byte GetVoxel(int x, int y, int z, uint materialIndex)
	{
		byte materialStrength = 0;
		if(volumeHandle.HasValue)
		{
			CubiquityDLL.GetVoxelMC(volumeHandle.Value, x, y, z, materialIndex, out materialStrength);
		}
		return materialStrength;
	}
	
	public void SetVoxel(int x, int y, int z, uint materialIndex, byte materialStrength)
	{
		if(volumeHandle.HasValue)
		{
			if(x >= region.lowerCorner.x && y >= region.lowerCorner.y && z >= region.lowerCorner.z
				&& x <= region.upperCorner.x && y <= region.upperCorner.y && z <= region.upperCorner.z) // FIX THESE VALUES!
			{
				CubiquityDLL.SetVoxelMC(volumeHandle.Value, x, y, z, materialIndex, materialStrength);
			}
		}
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
