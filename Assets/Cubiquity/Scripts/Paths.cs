using UnityEngine;

using System;
using System.Collections;
using System.IO;

namespace Cubiquity
{
	public class Paths
	{
		public static string SDK
		{
			get { return Application.streamingAssetsPath + "/Cubiquity/SDK"; }
		}
		
		public static string voxelDatabases
		{
			get { return Application.streamingAssetsPath + "/Cubiquity/VoxelDatabases"; }
		}
		
		public static string voxelDatabasesCreatedForAssets
		{
			get { return Application.streamingAssetsPath + "/Cubiquity/VoxelDatabases/CreatedForAssets"; }
		}
		
		public static String MakeRelativePath(String fromPath, String toPath)
	    {
	        if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
	        if (String.IsNullOrEmpty(toPath))   throw new ArgumentNullException("toPath");
	
	        Uri fromUri = new Uri(fromPath);
	        Uri toUri = new Uri(toPath);
	
	        if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.
	
	        Uri relativeUri = fromUri.MakeRelativeUri(toUri);
	        String relativePath = Uri.UnescapeDataString(relativeUri.ToString());
	
	        if (toUri.Scheme.ToUpperInvariant() == "FILE")
	        {
	            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
	        }
	
	        return relativePath;
	    }
	}
}
