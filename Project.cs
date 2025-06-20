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
        private const string modVersion = "1.0.8.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static UnderCheatBase Instance;

        internal ManualLogSource mls;

        public static TMP_FontAsset fontAsset;
        
        public static BepInEx.Configuration.ConfigEntry<bool> displayMaxResource;
        
        public static BepInEx.Configuration.ConfigEntry<int> KeyAmountAdd, BombAmountAdd, GoldAmountAdd, ThoriumAmountAdd, NetherAmountAdd;

        void Awake()
        {

            string[] availableFonts = Font.GetOSInstalledFontNames();
            if (availableFonts.Contains("Arial"))
            {
                Font dynamicFont = Font.CreateDynamicFontFromOSFont("Arial", 16);
                fontAsset = TMP_FontAsset.CreateFontAsset(dynamicFont);
                Logger.LogInfo("Loaded Arial font.");
            }
            else
            {
                Font dynamicFont = Font.CreateDynamicFontFromOSFont("Liberation Sans", 16);
                fontAsset = TMP_FontAsset.CreateFontAsset(dynamicFont);
                Logger.LogInfo("Loaded Liberation Sans font.");
            }

            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo($"{modName} has loaded (ModVersion: {modVersion}, ModGUID: {modGUID})!");

            harmony.PatchAll(typeof(Cheats));
            harmony.PatchAll(typeof(HUDControl));
            harmony.PatchAll(typeof(API));
            ConfigCreate();
        }

        void ConfigCreate()
        {
            UnderCheatBase.displayMaxResource = this.Config.Bind<bool>("Settings", "Display max resource count", true, "Display max resource count next to the current resource amount in the resource giving cheat");
            UnderCheatBase.KeyAmountAdd = this.Config.Bind<int>("Settings", "Amount of Keys added", 1, "Changes the amount of keys given in the resource cheat.");
            UnderCheatBase.KeyAmountAdd = this.Config.Bind<int>("Settings", "Amount of Keys added", 1, "Changes the amount of keys given in the resource cheat.");
            UnderCheatBase.BombAmountAdd = this.Config.Bind<int>("Settings", "Amount of Bombs added", 1, "Changes the amount of bombs given in the resource cheat.");
            UnderCheatBase.GoldAmountAdd = this.Config.Bind<int>("Settings", "Amount of Gold added", 1000, "Changes the amount of gold given in the resource cheat.");
            UnderCheatBase.ThoriumAmountAdd = this.Config.Bind<int>("Settings", "Amount of Thorium added", 10, "Changes the amount of thorium given in the resource cheat.");
            UnderCheatBase.NetherAmountAdd = this.Config.Bind<int>("Settings", "Amount of Nether added", 1, "Changes the amount of nether given in the resource cheat.");
            ConfigFix();
            LogConfig();
        }
        
        void ConfigFix()
        {
            if (UnderCheatBase.displayMaxResource.Value.ToString() != "true" && UnderCheatBase.displayMaxResource.Value.ToString() != "false") { UnderCheatBase.displayMaxResource.Value = true; }
            
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
        }

        void LogConfig()
        {
            mls.LogInfo($"Loaded Config amount for resource 'Key', Amount: '{UnderCheatBase.KeyAmountAdd.Value}'");
            mls.LogInfo($"Loaded Config amount for resource 'Bomb', Amount: '{UnderCheatBase.BombAmountAdd.Value}'");
            mls.LogInfo($"Loaded Config amount for resource 'Gold', Amount: '{UnderCheatBase.GoldAmountAdd.Value}'");
            mls.LogInfo($"Loaded Config amount for resource 'Thorium', Amount: '{UnderCheatBase.ThoriumAmountAdd.Value}'");
            mls.LogInfo($"Loaded Config amount for resource 'Nether', Amount: '{UnderCheatBase.NetherAmountAdd.Value}'");
        }

        void Update()
        {
            HUDControl.Update();
        }

    }
}
