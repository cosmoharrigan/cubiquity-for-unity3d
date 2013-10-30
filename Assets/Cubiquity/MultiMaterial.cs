using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

// Interop principle from: http://stackoverflow.com/a/12268531
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct MultiMaterial
{
	public const uint NoOfMaterials = 4;
	
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NoOfMaterials)]
	public byte[] materials;
}
