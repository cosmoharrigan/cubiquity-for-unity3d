using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
 
[InitializeOnLoad]
public class RunOnStartup
{
    static RunOnStartup()
    {
        string fileName = "CubiquityC.dll";
        string sourcePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Cubiquity");
        string targetPath =  @".";

        // Use Path class to manipulate file and directory paths. 
        string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
        string destFile = System.IO.Path.Combine(targetPath, fileName);
		
		if(System.IO.File.Exists(destFile))
		{
			byte[] sourceChecksum = GetChecksum(sourceFile);
			byte[] destChecksum = GetChecksum(destFile);
			
			bool checksumsMatch = true;
			for(int i = 0; i < sourceChecksum.Length; i++)
			{
				if(sourceChecksum[i] != destChecksum[i])
				{
					checksumsMatch = false;
					break;
				}
			}
			
			if(!checksumsMatch)
			{
				if(EditorUtility.DisplayDialog("Cubiquity DLL in project root appears to be the wrong version", "This project is using the Cubiquity voxel terrain engine but the DLL in the root of the project folder appears to be the wrong version (or corrupt). \n\nThis can be fixed automatically because we have a copy of the required DLL in the StreamingAssets/Cubiquity/NativeCode folder. Would you like this file to be copied to the root of the project folder?", "Yes, please fix this!", "No, I know what I'm doing..."))
				{
					System.IO.File.Copy(sourceFile, destFile, true);
				}
			}
		}
		else
		{
			if(EditorUtility.DisplayDialog("Cubiquity DLL not found in project root", "This project is using the Cubiquity voxel terrain engine but the required DLL has not been found in the root of the project folder. \n\nThis can be fixed automatically because we have a copy of the required DLL in the StreamingAssets/Cubiquity/NativeCode folder. Would you like this file to be copied to the root of the project folder?", "Yes, please fix this!", "No, I know what I'm doing..."))
			{
				System.IO.File.Copy(sourceFile, destFile, false);
			}
		}
		
		if(System.IO.File.Exists(destFile) == false)
		{
			Debug.LogWarning("The Cubiquity DLL was not found on startup, and this problem was not resolved.");
		}
    }
	
	// From http://stackoverflow.com/q/1177607
	private static byte[] GetChecksum(string file)
	{
		using (FileStream stream = File.OpenRead(file))
		{
			SHA256Managed sha = new SHA256Managed();
			return sha.ComputeHash(stream);
		}
	}
}