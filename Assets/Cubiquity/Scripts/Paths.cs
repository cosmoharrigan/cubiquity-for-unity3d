using UnityEngine;

using System;
using System.Collections;
using System.IO;

namespace Cubiquity
{
	/// Defines a number of commonly used paths.
	/**
	 * Be aware that these paths may depend on underlying Unity paths such as 'Application.streamingAssetsPath', and as such they may differ between editor
	 * and standalone builds as well as between platforms.
	 */
	public class Paths
	{
		/// Locatoion of the Cubiquity SDK containing the native code libraries and additional executables (converters, etc). 
		public static string SDK
		{
			get { return Application.streamingAssetsPath + "/Cubiquity/SDK"; }
		}
		
		/// Location of the Cubiquity '.vdb' files.
		/**
		 * If you create your own voxel databases (e.g. by using a converter) then you should place them in this folder. You will then be able to use them 
		 * to construct a VolumeData from code, or to create a volume data asset through the Cubiquity menus or volume inspectors (see the user manual).
		 */
		public static string voxelDatabases
		{
			get { return Application.streamingAssetsPath + "/Cubiquity/VoxelDatabases"; }
		}
		
		/// Location of the '.vdb' files which are created for new volume data assets.
		/**
		 * When you create a new asset (i.e. not from an existing voxel database) Cubiquity for Unity3D will create a new .vdb file to store the data. This
		 * is placed in a subdirectory to keep it seperate from any other voxel databases you might have, as this keeps things tidier. Note that .vdb's 
		 * generated in this way will have a random filename, and also that they will not be automatically removed if you later delete the asset.
		 */
		public static string voxelDatabasesCreatedForAssets
		{
			get { return Application.streamingAssetsPath + "/Cubiquity/VoxelDatabases/CreatedForAssets"; }
		}
		
		/// Utility method to construct a relative path between its two inputs.
		/**
		 * \param fromPath The path to start from
		 * \param toPath The path to finish at
		 * \return A string containing a relative path between the two inputs.
		 */
		// Implementation taken from here: http://stackoverflow.com/a/340454
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
