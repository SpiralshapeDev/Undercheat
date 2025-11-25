using BepInEx;
using System.Diagnostics;
using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UnderCheat
{
    public static class ConfigOpener
    {
        public static void OpenConfig(string configFileName)
        {
            string configPath = Path.Combine(Paths.ConfigPath, configFileName);

            if (!File.Exists(configPath))
            {
                UnityEngine.Debug.LogError($"{UnderCheatBase.ModGuid}: Config file not found at {configPath}");
                return;
            }

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
                }
                else if (isMac)
                {
                    UnityEngine.Debug.Log("Detected Mac-OS, attempting to open config");
                    Process.Start("open", configPath);
                }
                else
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