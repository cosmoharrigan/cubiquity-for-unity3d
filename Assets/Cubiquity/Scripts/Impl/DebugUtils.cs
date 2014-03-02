using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	namespace Impl
	{
		public class DebugUtils
		{
		    [System.Diagnostics.Conditional("UNITY_EDITOR")]
		    public static void Assert(bool condition, string message)
		    {
		        if (!condition) Debug.LogError("Assertion failed: " + message);
		    }
		}
	}
}
