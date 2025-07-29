using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using TMPro;
using Undercheat;
using System.Linq;
using Thor;

namespace UnderCheat
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class UnderCheatBase : BaseUnityPlugin
    {
        public const string modGUID = "SpiralMods." + modName;
        private const string modName = "UnderCheat";
        private const string modVersion = "1.2";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static UnderCheatBase Instance;

        internal ManualLogSource mls;

        public static TMP_FontAsset fontAsset;

        public static BepInEx.Configuration.ConfigEntry<bool> HideConfigHints;

        public static BepInEx.Configuration.ConfigEntry<int> KeyAmountAdd, BombAmountAdd, GoldAmountAdd, ThoriumAmountAdd, NetherAmountAdd;

        public static BepInEx.Configuration.ConfigEntry<float> DamageReduceHackPercentage, DamageBoostAmount;

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
            UnderCheatBase.DamageBoostAmount = this.Config.Bind<float>("Settings", "Damage Boost Amount", 999, "Amount of damage added in damage boosting hack.");
            LogConfig();
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
            mls.LogInfo($"Loaded Config for damage boost hack, Value: '{UnderCheatBase.DamageBoostAmount.Value}'");
        }

        void Update()
        {
            HUDControl.Update();
        }

        public void reloadConfig()
        {
            Config.Reload();

            foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
            {
                if ((UnityEngine.Object)player.Avatar != (UnityEngine.Object)null)
                {
                    if (player.Avatar.HasModifier("CheatDamageMelee"))
                    {
                        Cheats.CheatDamage();
                        Cheats.CheatDamage();
                    }
                }
            }
        }
    }
}
