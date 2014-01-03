using UnityEngine;
using System.Collections;

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
		public byte m4;
		public byte m5;
		public byte m6;
		public byte m7;
		#pragma warning restore 0649
	}
	
	public class TerrainVolumeRenderer : VolumeRenderer
	{
		public override void BuildMeshFromNodeHandle(uint nodeHandle, out Mesh renderingMesh, out Mesh physicsMesh, bool UseCollisionMesh)
		{
			// At some point I should read this: http://forum.unity3d.com/threads/5687-C-plugin-pass-arrays-from-C
			
			// Create rendering and possible collision meshes.
			renderingMesh = new Mesh();		
			physicsMesh = UseCollisionMesh && Application.isPlaying ? new Mesh() : null;
			
			// Get the data from Cubiquity.
			int[] indices = CubiquityDLL.GetIndicesMC(nodeHandle);		
			CubiquitySmoothVertex[] cubiquityVertices = CubiquityDLL.GetVerticesMC(nodeHandle);			
			
			// Create the arrays which we'll copy the data to.
	        Vector3[] renderingVertices = new Vector3[cubiquityVertices.Length];		
			Vector3[] renderingNormals = new Vector3[cubiquityVertices.Length];		
			Color32[] renderingColors = new Color32[cubiquityVertices.Length];		
			Vector3[] physicsVertices = UseCollisionMesh && Application.isPlaying ? new Vector3[cubiquityVertices.Length] : null;
			
			for(int ct = 0; ct < cubiquityVertices.Length; ct++)
			{
				// Get the vertex data from Cubiquity.
				Vector3 position = new Vector3(cubiquityVertices[ct].x, cubiquityVertices[ct].y, cubiquityVertices[ct].z);
				Vector3 normal = new Vector3(cubiquityVertices[ct].nx, cubiquityVertices[ct].ny, cubiquityVertices[ct].nz);
				Color32 color = new Color32(cubiquityVertices[ct].m0, cubiquityVertices[ct].m1, cubiquityVertices[ct].m2, cubiquityVertices[ct].m3);
				//UInt32 color = cubiquityVertices[ct].color;
				
				// Pack it for efficient vertex buffer usage.
				//float packedPosition = packPosition(position);
				//float packedColor = packColor(color);
					
				// Copy it to the arrays.
				renderingVertices[ct] = position;	
				renderingNormals[ct] = normal;
				renderingColors[ct] = color;
				if(UseCollisionMesh && Application.isPlaying)
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
			
			if(UseCollisionMesh && Application.isPlaying)
			{
				physicsMesh.vertices = physicsVertices;
				physicsMesh.triangles = indices;
			}
		}
	}
}
