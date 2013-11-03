using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	// FIXME - The color channels below should have their orderchanged both here and in cubiquity.
	// Red should be in the most significant four bits and alpha in the lease significant four bits.
	public struct CubeColor
	{
	    public uint color;
	
	    public uint red
	    {
	        get
			{
				return getFourBitsAt(0);
			}
			set
			{		
				setFourBitsAt(0, value);
			}
	    }
		
		public uint green
	    {
	        get
			{
				return getFourBitsAt(4);
			}
			set
			{		
				setFourBitsAt(4, value);
			}
	    }
		
		public uint blue
	    {
	        get
			{
				return getFourBitsAt(8);
			}
			set
			{		
				setFourBitsAt(8, value);
			}
	    }
		
		public uint alpha
	    {
	        get
			{
				return getFourBitsAt(12);
			}
			set
			{		
				setFourBitsAt(12, value);
			}
	    }
		
		private uint getFourBitsAt(ushort offset)
		{
			uint mask = 0x000F;
			uint result = color;
			result >>= offset;
			result &= mask;
			return result;
		}
		
		private void setFourBitsAt(ushort offset, uint val)
		{
			uint mask = 0x000F;
			int shift = (int)offset;
			mask <<= shift;
			
			color = (ushort)(color & (ushort)(~mask));
			
			val <<= shift;
			val &= mask;
			
			color |= val;
		}
	}
}
