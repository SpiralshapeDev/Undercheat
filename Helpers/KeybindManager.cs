using HarmonyLib;
using Rewired;
using Thor;
using UnityEngine;

namespace UnderCheat.Helpers
{
    public static class KeybindManager
    {
        private static SimulationPlayer player => Game.Instance.Simulation.PrimaryPlayer;
        [HarmonyPatch(typeof(Simulation))]
        [HarmonyPatch("Process")]
        [HarmonyPostfix]
        static void Update()
        {
            if (!Game.Instance.Simulation.PrimaryPlayer.Avatar || Game.Instance.Simulation.IsPaused) return;
            
            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.T))
            {
                HudManager.Hidden = !HudManager.Hidden;
            }
            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F1))
            {
                PageManager.Next();
            }
            
            HudManager.UpdateText();
            switch (PageManager.currentPage)
            {
                case 1:
                {
                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F2))
                    {
                        Debug.Log($"{UnderCheatBase.ModGuid}: {(!CheatManager.playerImmunity ? "Enabling" : "Disabling")} player damage reducer.");
                        CheatManager.playerImmunity = !CheatManager.playerImmunity;
                    }
                    
                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F3))
                    {
                        CheatManager.CheatDamage();
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F4))
                    {
                        CheatManager.ToggleDoors();
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F5))
                    {
                        CheatManager.UnlockAll();
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F6))
                    {
                        CheatManager.MaxPetLevel();
                    }
                    
                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F7))
                    {
                        UnderCheatBase.Instance.ReloadConfig();
                        Debug.Log($"{UnderCheatBase.ModGuid}: Refreshing config...");
                    }
                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F8) && !FileManager.isProton)
                    {
                        FileManager.OpenConfig($"{UnderCheatBase.ModGuid}.cfg");
                    }
                    break;
                }
                
                case 2:
                {
                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F2))
                    {
                        ResourceData resource = GameData.Instance.KeyResource;
                        CheatManager.AddResource(resource, UnderCheatBase.KeyAmountAdd.Value);
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F3))
                    {
                        ResourceData resource = GameData.Instance.BombResource;
                        CheatManager.AddResource(resource, UnderCheatBase.BombAmountAdd.Value);
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F4))
                    {
                        ResourceData resource = GameData.Instance.GoldResource;
                        CheatManager.AddResource(resource, UnderCheatBase.GoldAmountAdd.Value);
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F5))
                    {
                        ResourceData resource = GameData.Instance.ThoriumResource;
                        CheatManager.AddResource(resource, UnderCheatBase.ThoriumAmountAdd.Value);
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F6))
                    {
                        ResourceData resource = GameData.Instance.NetherResource;
                        CheatManager.AddResource(resource, UnderCheatBase.NetherAmountAdd.Value);
                    }
                    break;
                }

                case 3:
                {
                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F2))
                    {
                        PageManager.discoverPageItemIndex = PageManager.WrapIndex(PageManager.discoverPageItemIndex - 1,0, GameData.Instance.RelicCollection.Count-1);
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F3))
                    {
                        PageManager.discoverPageItemIndex = PageManager.WrapIndex(PageManager.discoverPageItemIndex + 1,0, GameData.Instance.RelicCollection.Count-1);
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F4))
                    {
                        var item = CheatManager.GetItemDataIndex(PageManager.discoverPageItemIndex);
                        if (!(item is ItemData itemData)) break;

                        GameData.Instance.Discover(itemData);
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F5))
                    {
                        var item = CheatManager.GetItemDataIndex(PageManager.discoverPageItemIndex);
                        if (!(item is ItemData itemData)) break;

                        CheatManager.SpawnRelic(itemData, player.Avatar.Position);
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F6))
                    {
                        System.Random rand = new System.Random();

                        var relics = GameData.Instance.RelicCollection;
                        int index = rand.Next(relics.Count);
                        var randomItem = relics[index];
                        if (!(randomItem is ItemData itemData)) break;

                        CheatManager.SpawnRelic(itemData, player.Avatar.Position);
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F7))
                    {
                        bool discoveredAndUnlockedOnly = false;
                        CheatManager.SummonAllRelics(discoveredAndUnlockedOnly);
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F8))
                    {
                        bool discoveredAndUnlockedOnly = true;
                        CheatManager.SummonAllRelics(discoveredAndUnlockedOnly);
                    }
                    break;
                }

                case 4:
                {
                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F2))
                    {
                        bool removeCurse = false;
                        HealthExt.CurseType curseType = HealthExt.CurseType.Minor;
                        CheatManager.ModifyCurses(removeCurse, curseType);
                        Debug.Log($"{UnderCheatBase.ModGuid}: Added {curseType} curse to player.");
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F3))
                    {
                        bool removeCurse = true;
                        HealthExt.CurseType curseType = HealthExt.CurseType.Minor;
                        CheatManager.ModifyCurses(removeCurse, curseType);
                        Debug.Log($"{UnderCheatBase.ModGuid}: Removed {curseType} curse from player.");
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F4))
                    {
                        bool removeCurse = false;
                        HealthExt.CurseType curseType = HealthExt.CurseType.Major;
                        CheatManager.ModifyCurses(removeCurse, curseType);
                        Debug.Log($"{UnderCheatBase.ModGuid}: Added {curseType} curse to player.");
                    }

                    if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F5))
                    {
                        bool removeCurse = true;
                        HealthExt.CurseType curseType = HealthExt.CurseType.Major;
                        CheatManager.ModifyCurses(removeCurse, curseType);
                        Debug.Log($"{UnderCheatBase.ModGuid}: Removed {curseType} curse from player.");
                    }
                    break;
                }
                default:
                {
                    Debug.LogWarning($"{UnderCheatBase.ModGuid}: Failed to load keybinds for unknown page {PageManager.currentPage}.");
                    break;
                }
            }
            HudManager.UpdateText();
        }
    }
}