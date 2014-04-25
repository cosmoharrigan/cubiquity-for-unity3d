using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	/// Thrown to indicate an error has occured inside the %Cubiquity native code library.
	/**
	 * %Cubiquity for Unity3D provides a C# wrapper around the %Cubiquity native code library. Whenever a function call to this library
	 * returns an error code Cubiquity for Unity3D retrieves the error message, packages it up into a CubiquityException, and throws it.
	 * You can find out more about the problem by reading the error message and consulting the %Cubiquity log file.
	 */
	public class CubiquityException: System.Exception
	{
	   public CubiquityException()
	   {
	   }
		
	   public CubiquityException(string message)
			:base(message)
	   {
	   }
		
		public CubiquityException(string message, CubiquityException innerException)
			:base(message, innerException)
	   {
	   }
	}
}
