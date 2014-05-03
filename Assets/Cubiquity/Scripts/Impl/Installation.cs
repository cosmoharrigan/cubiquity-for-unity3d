using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Cubiquity
{
	namespace Impl
	{
		public class Installation
		{		
			public static void ValidateAndFix()
			{	
				// Get the name of the library we will copy (different per platform).
				string fileName = "";
				switch(Application.platform)
				{
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer:
					fileName = "CubiquityC.dll";
					break;
				case RuntimePlatform.OSXEditor:
				case RuntimePlatform.OSXPlayer:
					fileName = "libCubiquityC.dylib";
					break;
				case RuntimePlatform.LinuxPlayer:
					fileName = "libCubiquityC.so";
					break;
				default:
					Debug.LogError("We're sorry, but Cubiquity for Unity3D is not currently supported on your platform");
					return;
				}
				
				// Copy the native code library from the SDK to the working directory.
		        string sourcePath = Paths.SDK;
				string destPath = System.IO.Directory.GetCurrentDirectory();
		
		        // Use Path class to manipulate file and directory paths. 
		        string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
		        string destFile = System.IO.Path.Combine(destPath, fileName);
				
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
						Debug.Log("Updating " + fileName + " in the project root folder as it doesn't match the version in the Cubiquity SDK.");
						
						try
						{
							// The target file exists (it's just the wrong version) so we set the flag to overwrite.
							System.IO.File.Copy(sourceFile, destFile, true);
						}
						catch(Exception e)
						{
							Debug.LogException(e);
							Debug.LogError("Failed to copy '" + fileName + "'");
						}
							
					}
				}
				else
				{
					Debug.Log("Setting up Cubiquity for Unity3D by copying " + fileName + " to the project root folder.");
					
					try
					{
						// The target file doesn't exist so we don't need to set the flag to overwrite.
						System.IO.File.Copy(sourceFile, destFile, false);
					}
					catch(Exception e)
					{
						Debug.LogException(e);
						Debug.LogError("Failed to copy '" + fileName + "'");
					}
				}
				
				if(System.IO.File.Exists(destFile) == false)
				{
					Debug.LogError("The Cubiquity DLL was not found in the project root folder, and this problem was not resolved.");
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
	}
}
