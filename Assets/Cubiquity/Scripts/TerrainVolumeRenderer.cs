using UnityEngine;
using System.Collections;

using Cubiquity.Impl;

namespace Cubiquity
{	
	[ExecuteInEditMode]
	public class TerrainVolumeRenderer : VolumeRenderer
	{
		void Awake()
		{
			if(material == null)
			{
				// Triplanar textuing seems like a good default material for the terrain volume.
				material = new Material(Shader.Find("TriplanarTexturing"));
			}
		}
		
		public override Mesh BuildMeshFromNodeHandle(uint nodeHandle)
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
	}
}
