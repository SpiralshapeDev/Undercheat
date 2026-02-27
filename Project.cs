using BepInEx;
using HarmonyLib;
using UnityEngine;
using TMPro;
using Undercheat;
using System.Linq;
using BepInEx.Configuration;
using Thor;
using UnderCheat.UI;
using UnderCheat.Cheats;

namespace UnderCheat
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    public class UnderCheatBase : BaseUnityPlugin
    {
        public const string ModGuid = "SpiralMods." + ModName;
        private const string ModName = "UnderCheat";
        private const string ModVersion = "1.2.3";

        private readonly Harmony _harmony = new Harmony(ModGuid);

        public static UnderCheatBase Instance;

        public static TMP_FontAsset FontAsset;

        public static ConfigEntry<bool> HideConfigHints;

        public static ConfigEntry<int> KeyAmountAdd;
        public static ConfigEntry<int> BombAmountAdd;
        public static ConfigEntry<int> GoldAmountAdd;
        public static ConfigEntry<int> ThoriumAmountAdd;
        public static ConfigEntry<int> NetherAmountAdd;

        public static ConfigEntry<float> DamageReduceHackPercentage;
        public static ConfigEntry<float> DamageBoostAmount;
        public static ConfigEntry<float> DamageAttackSpeed;

        void Awake()
        {
            Font font;
            string[] availableFonts = Font.GetOSInstalledFontNames();
            if (availableFonts.Contains("Arial"))
            {
                font = Font.CreateDynamicFontFromOSFont("Arial", 16);
                Debug.Log($"{UnderCheatBase.ModGuid}: Loaded Arial font.");
            }
            else
            {
                font = Font.CreateDynamicFontFromOSFont("Liberation Sans", 16);
                Debug.Log($"{UnderCheatBase.ModGuid}: Loaded Liberation Sans font.");
            }
            FontAsset = TMP_FontAsset.CreateFontAsset(font);

            if (Instance == null)
            {
                Instance = this;
            }

            Debug.Log($"{UnderCheatBase.ModGuid}: {ModName} has loaded (ModVersion: {ModVersion}, ModGUID: {ModGuid})!");

            _harmony.PatchAll(typeof(CheatManager));
            _harmony.PatchAll(typeof(HUDControl));
            _harmony.PatchAll(typeof(API));
            _harmony.PatchAll(typeof(Damage));
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
            UnderCheatBase.DamageBoostAmount = this.Config.Bind<float>("Settings", "Damage Boost Amount", 9999, "Amount of damage added in damage boosting hack.");
            UnderCheatBase.DamageAttackSpeed = this.Config.Bind<float>("Settings", "Attack Speed Boost Amount", 3, "(default ingame is 1) Range (0.1 to 5) Attack speed in damage boosting hack.");
            LogConfig();
        }

        void LogConfig()
        {
            Debug.Log($"{UnderCheatBase.ModGuid}: Loaded Config for hint hiding, Value: '{UnderCheatBase.HideConfigHints.Value}'");
            Debug.Log($"{UnderCheatBase.ModGuid}: Loaded Config for resource 'Key', Value: '{UnderCheatBase.KeyAmountAdd.Value}'");
            Debug.Log($"{UnderCheatBase.ModGuid}: Loaded Config for resource 'Bomb', Value: '{UnderCheatBase.BombAmountAdd.Value}'");
            Debug.Log($"{UnderCheatBase.ModGuid}: Loaded Config for resource 'Gold', Value: '{UnderCheatBase.GoldAmountAdd.Value}'");
            Debug.Log($"{UnderCheatBase.ModGuid}: Loaded Config for resource 'Thorium', Value: '{UnderCheatBase.ThoriumAmountAdd.Value}'");
            Debug.Log($"{UnderCheatBase.ModGuid}: Loaded Config for resource 'Nether', Value: '{UnderCheatBase.NetherAmountAdd.Value}'");
            Debug.Log($"{UnderCheatBase.ModGuid}: Loaded Config for damage reduce percentage, Value: '{UnderCheatBase.DamageReduceHackPercentage.Value}%'");
            Debug.Log($"{UnderCheatBase.ModGuid}: Loaded Config for damage boost hack, Value: '{UnderCheatBase.DamageBoostAmount.Value}'");
            Debug.Log($"{UnderCheatBase.ModGuid}: Loaded Config for attack speed boost amount, Value: '{UnderCheatBase.DamageBoostAmount.Value}'");
        }

        public void ReloadConfig()
        {
            Config.Reload();

            foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
            {
                if (!(UnityEngine.Object)player.Avatar) { break; }
                if (!player.Avatar.HasModifier("CheatMeleeDamage")) { break; }

                CheatManager.CheatDamage();
                CheatManager.CheatDamage();
            }
        }
    }
}
