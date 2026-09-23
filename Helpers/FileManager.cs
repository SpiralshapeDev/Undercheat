using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using BepInEx;

namespace UnderCheat.Helpers
{
    public static class FileManager
    {
        public static readonly bool isProton = Paths.ConfigPath.Contains("Z:\\");
        
        public static void OpenConfig(string configFileName)
        {
            string configPath = Path.Combine(Paths.ConfigPath, configFileName);
            
            try
            {
                UnityEngine.Debug.Log($"Opening config file {configPath}");
                bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                bool isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
                
                if (isWindows)
                {
                    UnityEngine.Debug.Log("Detected Windows, attempting to open config");
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = configPath,
                        UseShellExecute = true
                    });
                } else if (isMac)
                {
                    UnityEngine.Debug.Log("Detected Mac-OS, attempting to open config");
                    Process.Start("open", configPath);
                } else
                {
                    UnityEngine.Debug.Log("Detected Unix-like OS, attempting to open config");
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "xdg-open",
                        Arguments = $"\"{configPath}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"{UnderCheatBase.ModGuid}: Failed to open config file: {ex}");
            }
        }
    }
}