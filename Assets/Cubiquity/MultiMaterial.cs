using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

// Interop principle from: http://stackoverflow.com/a/12268531
//[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct MultiMaterial
{
	public const uint NoOfMaterials = 4;
	
	//[MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)NoOfMaterials)]
	//public byte[] materials;
	
	private uint data;
	
	public uint m0
    {
        get
		{
			return getEightBitsAt(0);
		}
		set
		{		
			setEightBitsAt(0, value);
		}
    }
	
	private uint getEightBitsAt(int offset)
	{
		uint mask = 0x000000FF;
		uint result = data;
		result >>= offset;
		result &= mask;
		return result;
	}
	
	private void setEightBitsAt(int offset, uint val)
	{
		uint mask = 0x000000FF;
		int shift = offset;
		mask <<= shift;
		
		data = (uint)(data & (uint)(~mask));
		
		val <<= shift;
		val &= mask;
		
		data |= val;
	}
}
