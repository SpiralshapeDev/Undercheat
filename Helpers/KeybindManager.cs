using Thor;
using UnityEngine;

namespace UnderCheat.Helpers
{
    internal static class KeybindManager
    {
        private static SimulationPlayer player => Game.Instance.Simulation.PrimaryPlayer;

        public static void Initialize()
        {
            // Always present
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "toggle_ui", new KeybindLib.KeybindManager.KeyDetails(KeyCode.T, KeybindLib.KeybindManager.KeyEnvironment.Any, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Toggle UI","All pages")).AddListener(() =>
            {
                if (Game.Instance.State != Game.GameState.Playing) return;
                HudManager.Hidden = !HudManager.Hidden;
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "next_page", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F1, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Next Page","All pages")).AddListener(() =>
            {
                PageManager.Next();
            });

            // Page 1
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "toggle_damage_reducer", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F2, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Toggle Damage Reducer","Page 1")).AddListener(() =>
            {
                if (PageManager.currentPage != 1) return;

                Debug.Log($"{UnderCheatBase.ModGuid}: {(!CheatManager.playerImmunity ? "Enabling" : "Disabling")} player damage reducer.");
                CheatManager.playerImmunity = !CheatManager.playerImmunity;
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "toggle_attack_damage_booster", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F3, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Toggle Attack Damage Booster","Page 1")).AddListener(() =>
            {
                if (PageManager.currentPage != 1) return;

                CheatManager.CheatDamage();
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "toggle_closed_doors", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F4, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Toggle closed doors","Page 1")).AddListener(() =>
            {
                if (PageManager.currentPage != 1) return;

                CheatManager.ToggleDoors();
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "unlock_all_items", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F5, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Unlock All Items","Page 1")).AddListener(() =>
            {
                if (PageManager.currentPage != 1) return;

                CheatManager.UnlockAll();
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "max_pet_level", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F6, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Max Pet Level","Page 1")).AddListener(() =>
            {
                if (PageManager.currentPage != 1) return;

                CheatManager.MaxPetLevel();
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "refresh_config", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F7, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Refresh Config","Page 1")).AddListener(() =>
            {
                if (PageManager.currentPage != 1) return;

                UnderCheatBase.Instance.ReloadConfig();
                Debug.Log($"{UnderCheatBase.ModGuid}: Refreshing config...");
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "open_config_file", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F8, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Open Config File","Page 1")).AddListener(() =>
            {
                if (PageManager.currentPage != 1) return;
                if (FileManager.isProton) return;

                FileManager.OpenConfig($"{UnderCheatBase.ModGuid}.cfg");
            });

            // Page 2
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "mod_keys", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F2, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Modify Keys","Page 2")).AddListener(() =>
            {
                if (PageManager.currentPage != 2) return;

                CheatManager.AddResource(GameData.Instance.KeyResource, UnderCheatBase.KeyAmountAdd.Value);
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "mod_bombs", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F3, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Modify Bombs","Page 2")).AddListener(() =>
            {
                if (PageManager.currentPage != 2) return;

                CheatManager.AddResource(GameData.Instance.BombResource, UnderCheatBase.BombAmountAdd.Value);
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "mod_gold", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F4, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Modify Gold","Page 2")).AddListener(() =>
            {
                if (PageManager.currentPage != 2) return;

                CheatManager.AddResource(GameData.Instance.GoldResource, UnderCheatBase.GoldAmountAdd.Value);
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "mod_thorium", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F5, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Modify Thorium","Page 2")).AddListener(() =>
            {
                if (PageManager.currentPage != 2) return;

                CheatManager.AddResource(GameData.Instance.ThoriumResource, UnderCheatBase.ThoriumAmountAdd.Value);
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "mod_nether", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F6, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Modify Nether","Page 2")).AddListener(() =>
            {
                if (PageManager.currentPage != 2) return;

                CheatManager.AddResource(GameData.Instance.NetherResource, UnderCheatBase.NetherAmountAdd.Value);
            });

            // Page 3
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "item_previous", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F2, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Previous Item","Page 3")).AddListener(() =>
            {
                if (PageManager.currentPage != 3) return;

                PageManager.discoverPageItemIndex = PageManager.WrapIndex(PageManager.discoverPageItemIndex - 1,0, GameData.Instance.RelicCollection.Count-1);
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "item_next", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F3, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Next Item","Page 3")).AddListener(() =>
            {
                if (PageManager.currentPage != 3) return;

                PageManager.discoverPageItemIndex = PageManager.WrapIndex(PageManager.discoverPageItemIndex + 1,0, GameData.Instance.RelicCollection.Count-1);
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "discover_relic", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F4, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Discover Relic","Page 3")).AddListener(() =>
            {
                if (PageManager.currentPage != 3) return;

                ItemData item = CheatManager.GetItemDataIndex(PageManager.discoverPageItemIndex);
                if (!(item is ItemData itemData)) return;

                GameData.Instance.Discover(itemData);
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "spawn_relic", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F5, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Spawn Relic","Page 3")).AddListener(() =>
            {
                if (PageManager.currentPage != 3) return;

                ItemData item = CheatManager.GetItemDataIndex(PageManager.discoverPageItemIndex);
                if (!(item is ItemData itemData)) return;

                CheatManager.SpawnRelic(itemData, player.Avatar.Position);
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "spawn_random_relic", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F6, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Spawn Random Relic","Page 3")).AddListener(() =>
            {
                if (PageManager.currentPage != 3) return;

                DataObjectCollection relics = GameData.Instance.RelicCollection;
                int index = Random.Range(0, relics.Count);

                DataObject randomItem = relics[index];
                if (!(randomItem is ItemData itemData)) return;

                CheatManager.SpawnRelic(itemData, player.Avatar.Position);
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "spawn_all_discovered_relics", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F7, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Spawn All Known Relics","Page 3")).AddListener(() =>
            {
                if (PageManager.currentPage != 3) return;

                CheatManager.SummonAllRelics(false);
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "spawn_all_relics", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F8, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Spawn All Relics","Page 3")).AddListener(() =>
            {
                if (PageManager.currentPage != 3) return;

                CheatManager.SummonAllRelics(true);
            });

            // Page 4
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "add_minor_curse", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F2, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Add Minor Curse","Page 4")).AddListener(() =>
            {
                if (PageManager.currentPage != 4) return;

                HealthExt.CurseType curseType = HealthExt.CurseType.Minor;
                CheatManager.ModifyCurses(false, curseType);
                Debug.Log($"{UnderCheatBase.ModGuid}: Added {curseType} curse to player.");
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "remove_minor_curse", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F3, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Remove Minor Curse","Page 4")).AddListener(() =>
            {
                if (PageManager.currentPage != 4) return;

                HealthExt.CurseType curseType = HealthExt.CurseType.Minor;
                CheatManager.ModifyCurses(true, curseType);
                Debug.Log($"{UnderCheatBase.ModGuid}: Removed {curseType} curse to player.");
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "add_major_curse", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F4, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Add Major Curse","Page 4")).AddListener(() =>
            {
                if (PageManager.currentPage != 4) return;

                HealthExt.CurseType curseType = HealthExt.CurseType.Major;
                CheatManager.ModifyCurses(false, curseType);
                Debug.Log($"{UnderCheatBase.ModGuid}: Added {curseType} curse to player.");
            });
            KeybindLib.KeybindManager.RegisterEvent(UnderCheatBase.ModGuid, "remove_major_curse", new KeybindLib.KeybindManager.KeyDetails(KeyCode.F5, KeybindLib.KeybindManager.KeyEnvironment.InGame, KeybindLib.KeybindManager.KeyPressType.OnKeyDown, "Remove Major Curse","Page 4")).AddListener(() =>
            {
                if (PageManager.currentPage != 4) return;

                HealthExt.CurseType curseType = HealthExt.CurseType.Major;
                CheatManager.ModifyCurses(true, curseType);
                Debug.Log($"{UnderCheatBase.ModGuid}: Removed {curseType} curse to player.");
            });
        }
    }
}