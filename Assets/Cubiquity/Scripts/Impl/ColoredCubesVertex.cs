namespace Cubiquity
{
	namespace Impl
	{
		public struct ColoredCubesVertex 
		{
			// Disable 'Field ... is never assigned to'
			// warnings as this structure is just for interop
			#pragma warning disable 0649
			public float x;
			public float y;
			public float z;
			public QuantizedColor color;
			#pragma warning restore 0649
		}
	}
}
