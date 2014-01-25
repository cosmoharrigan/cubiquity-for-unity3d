using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	public static class GameObjectExtensions
	{
		// This is a convieniance function because we found we were often calling 'AddComponent' followed by 'GetComponent'.
		// This wraps it into a single line of code, which returns the component if it exists or creates it if it doesn't exist.
	    public static ComponentType GetOrAddComponent<ComponentType>(this GameObject gameObject) where ComponentType : Component
	    {
	        ComponentType component = gameObject.GetComponent<ComponentType>();
			if(component == null)
			{
				component = gameObject.AddComponent<ComponentType>();
			}
			return component;
	    }
	}
}
