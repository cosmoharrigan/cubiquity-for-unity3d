using UnityEngine;
using System.Collections;
using System.IO;

namespace Cubiquity
{
	public static class TerrainVolumeEditor
	{
		public static void SculptTerrainVolume(TerrainVolume volume, float centerX, float centerY, float centerZ, float brushInnerRadius, float brushOuterRadius, float amount)
		{
			CubiquityDLL.SculptTerrainVolume((uint)volume.volumeHandle, centerX, centerY, centerZ, brushInnerRadius, brushOuterRadius, amount);
		}
		
		public static void BlurTerrainVolume(TerrainVolume volume, float centerX, float centerY, float centerZ, float brushInnerRadius, float brushOuterRadius, float amount)
		{
			CubiquityDLL.BlurTerrainVolume((uint)volume.volumeHandle, centerX, centerY, centerZ, brushInnerRadius, brushOuterRadius, amount);
		}
		
		public static void PaintTerrainVolume(TerrainVolume volume, float centerX, float centerY, float centerZ, float brushInnerRadius, float brushOuterRadius, float amount, uint materialIndex)
		{
			CubiquityDLL.PaintTerrainVolume((uint)volume.volumeHandle, centerX, centerY, centerZ, brushInnerRadius, brushOuterRadius, amount, materialIndex);
		}
	}
}
