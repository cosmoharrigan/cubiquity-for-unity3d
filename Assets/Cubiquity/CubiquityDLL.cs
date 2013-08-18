using System;
using System.Runtime.InteropServices;
using System.Text;

public class CubiquityDLL
{
	private static void Validate(int returnCode)
	{
		if(returnCode < 0)
		{
			throw new CubiquityException("An exception occured inside Cubiquity. Please see the log file for details");
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////
	// Volume functions
	////////////////////////////////////////////////////////////////////////////////
	[DllImport ("CubiquityC")]
	private static extern int cuNewColouredCubesVolume(int lowerX, int lowerY, int lowerZ, int upperX, int upperY, int upperZ, StringBuilder datasetName, uint baseNodeSize, out uint result);
	public static uint NewColoredCubesVolume(int lowerX, int lowerY, int lowerZ, int upperX, int upperY, int upperZ, string datasetName, uint baseNodeSize)
	{
		uint result;
		Validate(cuNewColouredCubesVolume(lowerX, lowerY, lowerZ, upperX, upperY, upperZ, new StringBuilder(datasetName), baseNodeSize, out result));
		return result;
	}
	
	[DllImport ("CubiquityC")]
	private static extern int cuNewColouredCubesVolumeFromVolDat(StringBuilder voldatFolder, StringBuilder datasetName, uint baseNodeSize, out uint result);	
	public static uint NewColoredCubesVolumeFromVolDat(string voldatFolder, string datasetName, uint baseNodeSize)
	{
		uint result;
		Validate(cuNewColouredCubesVolumeFromVolDat(new StringBuilder(voldatFolder), new StringBuilder(datasetName), baseNodeSize, out result));
		return result;
	}
	
	[DllImport ("CubiquityC")]
	private static extern int cuNewColouredCubesVolumeFromHeightmap(StringBuilder heightmapFileName, StringBuilder colormapFileName, StringBuilder datasetName, uint baseNodeSize, out uint result);	
	public static uint NewColoredCubesVolumeFromHeightmap(string heightmapFileName, string colormapFileName, string datasetName, uint baseNodeSize)
	{
		uint result;
		Validate(cuNewColouredCubesVolumeFromHeightmap(new StringBuilder(heightmapFileName), new StringBuilder(colormapFileName), new StringBuilder(datasetName), baseNodeSize, out result));
		return result;
	}
	
	[DllImport ("CubiquityC")]
	private static extern int cuUpdateVolume(uint volumeHandle);
	public static void UpdateVolume(uint volumeHandle)
	{
		Validate(cuUpdateVolume(volumeHandle));
	}
	
	[DllImport ("CubiquityC")]
	private static extern int cuGetEnclosingRegion(uint volumeHandle, out int lowerX, out int lowerY, out int lowerZ, out int upperX, out int upperY, out int upperZ);	
	public static void GetEnclosingRegion(uint volumeHandle, out int lowerX, out int lowerY, out int lowerZ, out int upperX, out int upperY, out int upperZ)
	{		
		Validate(cuGetEnclosingRegion(volumeHandle, out lowerX, out lowerY, out lowerZ, out upperX, out upperY, out upperZ));
	}
	
	[DllImport ("CubiquityC")]
	private static extern int cuGetVoxel(uint volumeHandle, int x, int y, int z, out byte red, out byte green, out byte blue, out byte alpha);	
	public static void GetVoxel(uint volumeHandle, int x, int y, int z, out byte red, out byte green, out byte blue, out byte alpha)
	{		
		Validate(cuGetVoxel(volumeHandle, x, y, z, out red, out green, out blue, out alpha));
	}
	
	[DllImport ("CubiquityC")]
	private static extern int cuSetVoxel(uint volumeHandle, int x, int y, int z, byte red, byte green, byte blue, byte alpha);
	public static void SetVoxel(uint volumeHandle, int x, int y, int z, byte red, byte green, byte blue, byte alpha)
	{
		Validate(cuSetVoxel(volumeHandle, x, y, z, red, green, blue, alpha));
	}
	
	[DllImport ("CubiquityC")]
	private static extern int cuDeleteColouredCubesVolume(uint volumeHandle);
	public static void DeleteColoredCubesVolume(uint volumeHandle)
	{
		Validate(cuDeleteColouredCubesVolume(volumeHandle));
	}
	
	////////////////////////////////////////////////////////////////////////////////
	// Octree functions
	////////////////////////////////////////////////////////////////////////////////
	[DllImport ("CubiquityC")]
	private static extern int cuHasRootOctreeNode(uint volumeHandle, out uint result);
	public static uint HasRootOctreeNode(uint volumeHandle)
	{
		uint result;
		Validate(cuHasRootOctreeNode(volumeHandle, out result));
		return result;
	}
	
	[DllImport ("CubiquityC")]
	private static extern int cuGetRootOctreeNode(uint volumeHandle, out uint result);
	public static uint GetRootOctreeNode(uint volumeHandle)
	{
		uint result;
		Validate(cuGetRootOctreeNode(volumeHandle, out result));
		return result;
	}
	
	[DllImport ("CubiquityC")]
	private static extern int cuHasChildNode(uint nodeHandle, uint childX, uint childY, uint childZ, out uint result);
	public static uint HasChildNode(uint nodeHandle, uint childX, uint childY, uint childZ)
	{
		uint result;
		Validate(cuHasChildNode(nodeHandle, childX, childY, childZ, out result));
		return result;
	}
	
	[DllImport ("CubiquityC")]
	private static extern int cuGetChildNode(uint nodeHandle, uint childX, uint childY, uint childZ, out uint result);
	public static uint GetChildNode(uint nodeHandle, uint childX, uint childY, uint childZ)
	{
		uint result;
		Validate(cuGetChildNode(nodeHandle, childX, childY, childZ, out result));
		return result;
	}
	
	[DllImport ("CubiquityC")]
	private static extern int cuNodeHasMesh(uint nodeHandle, out uint result);
	public static uint NodeHasMesh(uint nodeHandle)
	{
		uint result;
		Validate(cuNodeHasMesh(nodeHandle, out result));
		return result;
	}
	
	[DllImport ("CubiquityC")]
	private static extern int cuGetNodePosition(uint nodeHandle, out int x, out int y, out int z);
	public static void GetNodePosition(uint nodeHandle, out int x, out int y, out int z)
	{
		Validate(cuGetNodePosition(nodeHandle, out x, out y, out z));
	}
	
	[DllImport ("CubiquityC")]
	private static extern int cuGetMeshLastUpdated(uint nodeHandle, out uint result);
	public static uint GetMeshLastUpdated(uint nodeHandle)
	{
		uint result;
		Validate(cuGetMeshLastUpdated(nodeHandle, out result));
		return result;
	}
	
	////////////////////////////////////////////////////////////////////////////////
	// Mesh functions
	////////////////////////////////////////////////////////////////////////////////
	[DllImport ("CubiquityC")]
	private static extern int cuGetNoOfIndices(uint octreeNodeHandle, out uint result);
	[DllImport ("CubiquityC")]
	private static extern int cuGetIndices(uint octreeNodeHandle, out int[] result);
	public static int[] GetIndices(uint octreeNodeHandle)
	{
		uint noOfIndices;
		Validate(cuGetNoOfIndices(octreeNodeHandle, out noOfIndices));
		
		int[] result = new int[noOfIndices];
		Validate(cuGetIndices(octreeNodeHandle, out result));
		
		return result;
	}
		
	[DllImport ("CubiquityC")]
	private static extern int cuGetNoOfVertices(uint octreeNodeHandle, out uint result);
	[DllImport ("CubiquityC")]
	private static extern int cuGetVertices(uint octreeNodeHandle, out CubiquityVertex[] result);
	public static CubiquityVertex[] GetVertices(uint octreeNodeHandle)
	{
		// Based on http://stackoverflow.com/a/1318929
		uint noOfVertices;
		Validate(cuGetNoOfVertices(octreeNodeHandle, out noOfVertices));
		
		CubiquityVertex[] result = new CubiquityVertex[noOfVertices];
		Validate(cuGetVertices(octreeNodeHandle, out result));
		
		return result;
	}
	
	////////////////////////////////////////////////////////////////////////////////
	// Clock functions
	////////////////////////////////////////////////////////////////////////////////
	[DllImport ("CubiquityC")]
	private static extern int cuGetCurrentTime(out uint result);
	public static uint GetCurrentTime()
	{
		uint result;
		Validate(cuGetCurrentTime(out result));
		return result;
	}
	
	////////////////////////////////////////////////////////////////////////////////
	// Raycasting functions
	////////////////////////////////////////////////////////////////////////////////
	[DllImport ("CubiquityC")]
	private static extern int cuPickFirstSolidVoxel(uint volumeHandle, float rayStartX, float rayStartY, float rayStartZ, float rayDirX, float rayDirY, float rayDirZ, out int resultX, out int resultY, out int resultZ, out uint result);
	public static uint PickFirstSolidVoxel(uint volumeHandle, float rayStartX, float rayStartY, float rayStartZ, float rayDirX, float rayDirY, float rayDirZ, out int resultX, out int resultY, out int resultZ)
	{
		uint result;
		Validate(cuPickFirstSolidVoxel(volumeHandle, rayStartX, rayStartY, rayStartZ, rayDirX, rayDirY, rayDirZ, out resultX, out resultY, out resultZ, out result));
		return result;
	}
	
	[DllImport ("CubiquityC")]
	private static extern int cuPickLastEmptyVoxel(uint volumeHandle, float rayStartX, float rayStartY, float rayStartZ, float rayDirX, float rayDirY, float rayDirZ, out int resultX, out int resultY, out int resultZ, out uint result);
	public static uint PickLastEmptyVoxel(uint volumeHandle, float rayStartX, float rayStartY, float rayStartZ, float rayDirX, float rayDirY, float rayDirZ, out int resultX, out int resultY, out int resultZ)
	{
		uint result;
		Validate(cuPickLastEmptyVoxel(volumeHandle, rayStartX, rayStartY, rayStartZ, rayDirX, rayDirY, rayDirZ, out resultX, out resultY, out resultZ, out result));
		return result;
	}
}
