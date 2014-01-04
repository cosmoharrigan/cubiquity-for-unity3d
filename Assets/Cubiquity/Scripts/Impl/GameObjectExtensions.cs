using UnityEngine;
using System.Collections;

namespace Cubiquity
{
	public static class GameObjectExtensions
	{
	    public static Component GetOrAddComponent<ComponentType>(this GameObject gameObject) where ComponentType : Component
	    {
	        Component component = gameObject.GetComponent<ComponentType>();
			if(component == null)
			{
				component = gameObject.AddComponent<ComponentType>();
			}
			return component;
	    }
	}
}
