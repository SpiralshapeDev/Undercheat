using BepInEx;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

namespace UnderCheat
{
    public static class ConfigOpener
    {
        public static void OpenConfig(string configFileName)
        {
            string configPath = Path.Combine(Paths.ConfigPath, configFileName);

            if (!File.Exists(configPath))
            {
                Debug.LogError($"{UnderCheatBase.modGUID}: Config file not found at {configPath}");
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
                Debug.LogError($"{UnderCheatBase.modGUID}: Failed to open config file: {ex}");
            }
        }
    }

}
