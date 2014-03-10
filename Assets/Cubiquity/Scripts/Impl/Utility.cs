using UnityEngine;
using System.Collections;

namespace Cubiquity.Impl
{
	public class Utility
	{
		// We use a static Random for making filenames, as Randoms are seeded by timestamp
		// and client code could potentially create a number of filenames in quick sucession.  
		private static System.Random randomIntGenerator = new System.Random();
			
		public static string GenerateRandomVoxelDatabaseName()
		{
			// Generate a random filename from an integer
			return randomIntGenerator.Next().ToString("X8") + ".vdb";
		}
	}
}
