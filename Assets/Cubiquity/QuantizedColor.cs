using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	// FIXME - The color channels below should have their orderchanged both here and in cubiquity.
	// Red should be in the most significant four bits and alpha in the lease significant four bits.
	public struct QuantizedColor
	{
	    public uint color;
	
	    public uint red
	    {
	        get
			{
				//return getFourBitsAt(0);
				//return getBits(3, 0);
				return getBits(15, 12);
			}
			set
			{		
				//setFourBitsAt(0, value);
				//setBits(3, 0, value);
				setBits(15, 12, value);
			}
	    }
		
		public uint green
	    {
	        get
			{
				//return getFourBitsAt(4);
				//return getBits(7, 4);
				return getBits(11, 8);
			}
			set
			{		
				//setFourBitsAt(4, value);
				//setBits(7, 4, value);
				setBits(11, 8, value);
			}
	    }
		
		public uint blue
	    {
	        get
			{
				//return getFourBitsAt(8);
				//return getBits(11, 8);
				return getBits(7, 4);
			}
			set
			{		
				//setFourBitsAt(8, value);
				//setBits(11, 8, value);
				setBits(7, 4, value);
			}
	    }
		
		public uint alpha
	    {
	        get
			{
				//return getFourBitsAt(12);
				//return getBits(15, 12);
				return getBits(3, 0);
			}
			set
			{		
				//setFourBitsAt(12, value);
				//setBits(15, 12, value);
				setBits(3, 0, value);
			}
	    }
		
		uint getBits(uint MSB, uint LSB)
		{
			int noOfBitsToGet = (int)((MSB - LSB) + 1);

			// Build a mask containing all '0's except for the least significant bits (which are '1's).
			uint mask = uint.MaxValue; //Set to all '1's
			mask = mask << noOfBitsToGet; // Insert the required number of '0's for the lower bits
			mask = ~mask; // And invert

			// Move the desired bits into the LSBs and mask them off
			uint result = (color >> (int)LSB) & mask;

			return result;
		}
		
		void setBits(uint MSB, uint LSB, uint bitsToSet)
		{
			int noOfBitsToSet = (int)((MSB - LSB) + 1);

			uint mask = uint.MaxValue; //Set to all '1's
			mask = mask << noOfBitsToSet; // Insert the required number of '0's for the lower bits
			mask = ~mask; // And invert
			mask = mask << (int)LSB;

			bitsToSet = (bitsToSet << (int)LSB) & mask;

			color = (color & ~mask) | bitsToSet;
		}
		
		/*private uint getFourBitsAt(ushort offset)
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
		}*/
	}
}
