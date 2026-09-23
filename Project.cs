using BepInEx;
using HarmonyLib;
using UnityEngine;
using TMPro;
using System.Linq;
using BepInEx.Configuration;
using Thor;
using UnderCheat.Helpers;

namespace UnderCheat
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    public class UnderCheatBase : BaseUnityPlugin
    {
        public const string ModGuid = "SpiralMods." + ModName;
        private const string ModName = "UnderCheat";
        private const string ModVersion = "1.2.5";

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
            string[] availableFonts = Font.GetOSInstalledFontNames();
            Font font = availableFonts.Contains("Arial") ? Font.CreateDynamicFontFromOSFont("Arial", 16) : Font.CreateDynamicFontFromOSFont("Liberation Sans", 16);
            Debug.Log($"{ModGuid}: Loaded {font.name} font.");
            FontAsset = TMP_FontAsset.CreateFontAsset(font);

            if (Instance == null)
            {
                Instance = this;
            }

            Debug.Log($"{ModGuid}: {ModName} has loaded (ModVersion: {ModVersion}, ModGUID: {ModGuid})!");

            _harmony.PatchAll(typeof(FileManager));
            _harmony.PatchAll(typeof(KeybindManager));
            _harmony.PatchAll(typeof(PageManager));
            _harmony.PatchAll(typeof(CheatManager));
            _harmony.PatchAll(typeof(HudManager));
            ConfigCreate();
        }

        void ConfigCreate()
        {
            HideConfigHints = Config.Bind<bool>("Settings", "Hide config hints", false, "Hide hints in the mod UI");
            KeyAmountAdd = Config.Bind<int>("Settings", "Amount of Keys added", 1, "Changes the amount of keys given in the resource cheat.");
            KeyAmountAdd = Config.Bind<int>("Settings", "Amount of Keys added", 1, "Changes the amount of keys given in the resource cheat.");
            BombAmountAdd = Config.Bind<int>("Settings", "Amount of Bombs added", 1, "Changes the amount of bombs given in the resource cheat.");
            GoldAmountAdd = Config.Bind<int>("Settings", "Amount of Gold added", 1000, "Changes the amount of gold given in the resource cheat.");
            ThoriumAmountAdd = Config.Bind<int>("Settings", "Amount of Thorium added", 10, "Changes the amount of thorium given in the resource cheat.");
            NetherAmountAdd = Config.Bind<int>("Settings", "Amount of Nether added", 1, "Changes the amount of nether given in the resource cheat.");
            DamageReduceHackPercentage = Config.Bind<float>("Settings", "Percentage of damage reduced", 100f, "Amount of damage reduced in damage reducing hack.");
            DamageBoostAmount = Config.Bind<float>("Settings", "Damage Boost Amount", 9999f, "Amount of damage added in damage boosting hack.");
            DamageAttackSpeed = Config.Bind<float>("Settings", "Attack Speed Boost Amount", 3f, "(default in game is 1) Range (0.1 to 5) Attack speed in damage boosting hack.");
            LogConfig();
        }

        void LogConfig()
        {
            Debug.Log($"{ModGuid}: Loaded Config for hint hiding, Value: '{HideConfigHints.Value}'");
            Debug.Log($"{ModGuid}: Loaded Config for resource 'Key', Value: '{KeyAmountAdd.Value}'");
            Debug.Log($"{ModGuid}: Loaded Config for resource 'Bomb', Value: '{BombAmountAdd.Value}'");
            Debug.Log($"{ModGuid}: Loaded Config for resource 'Gold', Value: '{GoldAmountAdd.Value}'");
            Debug.Log($"{ModGuid}: Loaded Config for resource 'Thorium', Value: '{ThoriumAmountAdd.Value}'");
            Debug.Log($"{ModGuid}: Loaded Config for resource 'Nether', Value: '{NetherAmountAdd.Value}'");
            Debug.Log($"{ModGuid}: Loaded Config for damage reduce percentage, Value: '{DamageReduceHackPercentage.Value}%'");
            Debug.Log($"{ModGuid}: Loaded Config for damage boost hack, Value: '{DamageBoostAmount.Value}'");
            Debug.Log($"{ModGuid}: Loaded Config for attack speed boost amount, Value: '{DamageBoostAmount.Value}'");
        }

        public void ReloadConfig()
        {
            Config.Reload();

            if (!Game.Instance.Simulation.PrimaryPlayer.Avatar) return;
            if (Game.Instance.Simulation.PrimaryPlayer.Avatar.HasModifier("UnderCheat.meleeDamageBoost"))
            {
                CheatManager.CheatDamage();
                CheatManager.CheatDamage();
            }
        }
    }
}
