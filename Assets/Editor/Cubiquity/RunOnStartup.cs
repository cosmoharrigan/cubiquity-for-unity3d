using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
 
[InitializeOnLoad]
public class RunOnStartup
{
    static RunOnStartup()
    {
        Installation.ValidateAndFix();
    }
}