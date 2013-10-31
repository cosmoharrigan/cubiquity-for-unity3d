using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Cubiquity
{
	public struct ByteArray
	{
		private uint data;
		
		public uint this[int i]
		{
			get
			{
				return getEightBitsAt(i * 8);
			}
			set
			{
				setEightBitsAt(i * 8, value);
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
}