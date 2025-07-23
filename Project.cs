using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using TMPro;
using Undercheat;
using System.Linq;

namespace UnderCheat
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class UnderCheatBase : BaseUnityPlugin
    {
        public const string modGUID = "SpiralMods." + modName;
        private const string modName = "UnderCheat";
        private const string modVersion = "1.1.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static UnderCheatBase Instance;

        internal ManualLogSource mls;

        public static TMP_FontAsset fontAsset;

        public static BepInEx.Configuration.ConfigEntry<bool> HideConfigHints;

        public static BepInEx.Configuration.ConfigEntry<int> KeyAmountAdd, BombAmountAdd, GoldAmountAdd, ThoriumAmountAdd, NetherAmountAdd;

        public static BepInEx.Configuration.ConfigEntry<float> DamageReduceHackPercentage;

        void Awake()
        {
            Font font;
            string[] availableFonts = Font.GetOSInstalledFontNames();
            if (availableFonts.Contains("Arial"))
            {
                font = Font.CreateDynamicFontFromOSFont("Arial", 16);
                Logger.LogInfo("Loaded Arial font.");
            }
            else
            {
                font = Font.CreateDynamicFontFromOSFont("Liberation Sans", 16);
                Logger.LogInfo("Loaded Liberation Sans font.");
            }
            fontAsset = TMP_FontAsset.CreateFontAsset(font);

            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo($"{modName} has loaded (ModVersion: {modVersion}, ModGUID: {modGUID})!");

            harmony.PatchAll(typeof(Cheats));
            harmony.PatchAll(typeof(HUDControl));
            harmony.PatchAll(typeof(API));
            harmony.PatchAll(typeof(Damage));
            ConfigCreate();
        }

        void ConfigCreate()
        {
            UnderCheatBase.HideConfigHints = this.Config.Bind<bool>("Settings", "Hide config hints", false, "Hide hints in the mod UI");
            UnderCheatBase.KeyAmountAdd = this.Config.Bind<int>("Settings", "Amount of Keys added", 1, "Changes the amount of keys given in the resource cheat.");
            UnderCheatBase.KeyAmountAdd = this.Config.Bind<int>("Settings", "Amount of Keys added", 1, "Changes the amount of keys given in the resource cheat.");
            UnderCheatBase.BombAmountAdd = this.Config.Bind<int>("Settings", "Amount of Bombs added", 1, "Changes the amount of bombs given in the resource cheat.");
            UnderCheatBase.GoldAmountAdd = this.Config.Bind<int>("Settings", "Amount of Gold added", 1000, "Changes the amount of gold given in the resource cheat.");
            UnderCheatBase.ThoriumAmountAdd = this.Config.Bind<int>("Settings", "Amount of Thorium added", 10, "Changes the amount of thorium given in the resource cheat.");
            UnderCheatBase.NetherAmountAdd = this.Config.Bind<int>("Settings", "Amount of Nether added", 1, "Changes the amount of nether given in the resource cheat.");
            UnderCheatBase.DamageReduceHackPercentage = this.Config.Bind<float>("Settings", "Percentage of damage reduced", 100, "Amount of damage reduced in damage reducing hack.");
            ConfigFix();
            LogConfig();
        }
        
        void ConfigFix()
        {
            if (UnderCheatBase.KeyAmountAdd.Value.ToString().Contains(".")) { UnderCheatBase.KeyAmountAdd.Value = (int)Mathf.Round((float)UnderCheatBase.KeyAmountAdd.Value); }
            if (UnderCheatBase.KeyAmountAdd.Value <= -2147483647) { UnderCheatBase.KeyAmountAdd.Value = -2147483647; }
            if (UnderCheatBase.KeyAmountAdd.Value >= 2147483647) { UnderCheatBase.KeyAmountAdd.Value = 2147483647; }
            if (UnderCheatBase.KeyAmountAdd.Value == 0) { UnderCheatBase.KeyAmountAdd.Value = (int)UnderCheatBase.KeyAmountAdd.DefaultValue; }
            
            if (UnderCheatBase.BombAmountAdd.Value.ToString().Contains(".")) { UnderCheatBase.BombAmountAdd.Value = (int)Mathf.Round((float)UnderCheatBase.BombAmountAdd.Value); }
            if (UnderCheatBase.BombAmountAdd.Value <= -2147483647) { UnderCheatBase.BombAmountAdd.Value = -2147483647; }
            if (UnderCheatBase.BombAmountAdd.Value >= 2147483647) { UnderCheatBase.BombAmountAdd.Value = 2147483647; }
            if (UnderCheatBase.BombAmountAdd.Value == 0) { UnderCheatBase.BombAmountAdd.Value = (int)UnderCheatBase.BombAmountAdd.DefaultValue; }
            
            if (UnderCheatBase.GoldAmountAdd.Value.ToString().Contains(".")) { UnderCheatBase.GoldAmountAdd.Value = (int)Mathf.Round((float)UnderCheatBase.GoldAmountAdd.Value); }
            if (UnderCheatBase.GoldAmountAdd.Value <= -2147483647) { UnderCheatBase.GoldAmountAdd.Value = -2147483647; }
            if (UnderCheatBase.GoldAmountAdd.Value >= 2147483647) { UnderCheatBase.GoldAmountAdd.Value = 2147483647; }
            if (UnderCheatBase.GoldAmountAdd.Value == 0) { UnderCheatBase.GoldAmountAdd.Value = (int)UnderCheatBase.GoldAmountAdd.DefaultValue; }
            
            if (UnderCheatBase.ThoriumAmountAdd.Value.ToString().Contains(".")) { UnderCheatBase.ThoriumAmountAdd.Value = (int)Mathf.Round((float)UnderCheatBase.ThoriumAmountAdd.Value); }
            if (UnderCheatBase.ThoriumAmountAdd.Value <= -2147483647) { UnderCheatBase.ThoriumAmountAdd.Value = -2147483647; }
            if (UnderCheatBase.ThoriumAmountAdd.Value >= 2147483647) { UnderCheatBase.ThoriumAmountAdd.Value = 2147483647; }
            if (UnderCheatBase.ThoriumAmountAdd.Value == 0) { UnderCheatBase.ThoriumAmountAdd.Value = (int)UnderCheatBase.ThoriumAmountAdd.DefaultValue; }
            
            if (UnderCheatBase.NetherAmountAdd.Value.ToString().Contains(".")) { UnderCheatBase.NetherAmountAdd.Value = (int)Mathf.Round((float)UnderCheatBase.NetherAmountAdd.Value); }
            if (UnderCheatBase.NetherAmountAdd.Value <= -2147483647) { UnderCheatBase.NetherAmountAdd.Value = -2147483647; }
            if (UnderCheatBase.NetherAmountAdd.Value >= 2147483647) { UnderCheatBase.NetherAmountAdd.Value = 2147483647; }
            if (UnderCheatBase.NetherAmountAdd.Value == 0) { UnderCheatBase.NetherAmountAdd.Value = (int)UnderCheatBase.NetherAmountAdd.DefaultValue; }

            if (UnderCheatBase.DamageReduceHackPercentage.Value < 0) { UnderCheatBase.DamageReduceHackPercentage.Value = 0; }
            if (UnderCheatBase.DamageReduceHackPercentage.Value > 100) { UnderCheatBase.DamageReduceHackPercentage.Value = 100; }
        }

        void LogConfig()
        {
            mls.LogInfo($"Loaded Config for hint hiding, Value: '{UnderCheatBase.HideConfigHints.Value}'");
            mls.LogInfo($"Loaded Config for resource 'Key', Value: '{UnderCheatBase.KeyAmountAdd.Value}'");
            mls.LogInfo($"Loaded Config for resource 'Bomb', Value: '{UnderCheatBase.BombAmountAdd.Value}'");
            mls.LogInfo($"Loaded Config for resource 'Gold', Value: '{UnderCheatBase.GoldAmountAdd.Value}'");
            mls.LogInfo($"Loaded Config for resource 'Thorium', Value: '{UnderCheatBase.ThoriumAmountAdd.Value}'");
            mls.LogInfo($"Loaded Config for resource 'Nether', Value: '{UnderCheatBase.NetherAmountAdd.Value}'");
            mls.LogInfo($"Loaded Config for damage reduce hack percentage, Value: '{UnderCheatBase.DamageReduceHackPercentage.Value}%'");
        }

        void Update()
        {
            HUDControl.Update();
        }

        public void reloadConfig()
        {
            Config.Reload();
        }
    }
}
