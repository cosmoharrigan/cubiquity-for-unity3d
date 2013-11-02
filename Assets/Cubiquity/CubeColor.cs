using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	// FIXME - The color channels below should have their orderchanged both here and in cubiquity.
	// Red should be in the most significant four bits and alpha in the lease significant four bits.
	public struct CubeColor
	{
	    ushort color;
	
	    public ushort red
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
		
		public ushort green
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
		
		public ushort blue
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
		
		public ushort alpha
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
		
		private ushort getFourBitsAt(ushort offset)
		{
			ushort mask = 0x000F;
			ushort result = color;
			result >>= offset;
			result &= mask;
			return result;
		}
		
		private void setFourBitsAt(ushort offset, ushort val)
		{
			ushort mask = 0x000F;
			ushort shift = offset;
			mask <<= shift;
			
			color = (ushort)(color & (ushort)(~mask));
			
			val <<= shift;
			val &= mask;
			
			color |= val;
		}
	}
}
