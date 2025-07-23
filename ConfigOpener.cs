using BepInEx;
using System.Diagnostics;
using System.IO;

namespace UnderCheat
{
    public static class ConfigOpener
    {
        public static void OpenConfig(string configFileName)
        {
            string configPath = Path.Combine(Paths.ConfigPath, configFileName);

            if (!File.Exists(configPath))
            {
                UnityEngine.Debug.LogError($"Config file not found at {configPath}");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = configPath,
                    UseShellExecute = true
                });
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"Failed to open config file: {ex}");
            }
        }
    }

}
