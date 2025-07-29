using HarmonyLib;
using System.Collections.Generic;
using System;
using Thor;
using Rewired;
using UnityEngine;
using Undercheat;
using System.Reflection;
using BepInEx.Configuration;
using static Thor.ResourceManager;

namespace UnderCheat
{
    [HarmonyPatch(typeof(Game))]
    internal class Cheats
    {
        public static bool playerReducingDamage = false;
        static Simulation sim;

        static List<KeyCode> keyList = new List<KeyCode> {
            KeyCode.T,
            KeyCode.F1,
            KeyCode.F2,
            KeyCode.F3,
            KeyCode.F4,
            KeyCode.F5,
            KeyCode.F6,
            KeyCode.F7,
            KeyCode.F8
        };

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void patchUpdate()
        {
            // Allow Hacks Only When Needed
            if (HUDControl.guiActive && !sim.IsPaused)
            {
                // T Keybind
                if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.T))
                {
                    HUDControl.hidden = !HUDControl.hidden;
                }

                if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F1))
                {
                    API.current_page = API.next_page();
                }

                bool cheat_key_down = false;

                foreach(KeyCode key in keyList)
                {
                    if (ReInput.controllers.Keyboard.GetKeyDown(key))
                    {
                        cheat_key_down = true;
                    }
                }

                if (cheat_key_down)
                {
                    HUDControl.updateText();
                    switch (API.current_page)
                    {
                        case 1:

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F2))
                            {
                                playerReducingDamage = !playerReducingDamage;
                                Debug.Log($"{UnderCheatBase.modGUID}: {(playerReducingDamage ? "Enabling" : "Disabling")} player damage reducer.");
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F3))
                            {
                                CheatDamage();
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F4))
                            {
                                ToggleDoors();
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F5))
                            {
                                UnlockAll();
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F6))
                            {
                                MaxPetLevel();
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F7))
                            {
                                UnderCheatBase.Instance.reloadConfig();
                                Debug.Log("Refreshing config...");
                            }
                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F8))
                            {
                                ConfigOpener.OpenConfig($"{UnderCheatBase.modGUID}.cfg");
                            }

                            break;

                        case 2:

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F2))
                            {
                                ResourceData resource = GameData.Instance.KeyResource;
                                AddResource(resource, UnderCheatBase.KeyAmountAdd.Value);
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F3))
                            {
                                ResourceData resource = GameData.Instance.BombResource;
                                AddResource(resource, UnderCheatBase.BombAmountAdd.Value);
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F4))
                            {
                                ResourceData resource = GameData.Instance.GoldResource;
                                AddResource(resource, UnderCheatBase.GoldAmountAdd.Value);
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F5))
                            {
                                ResourceData resource = GameData.Instance.ThoriumResource;
                                AddResource(resource, UnderCheatBase.ThoriumAmountAdd.Value);
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F6))
                            {
                                ResourceData resource = GameData.Instance.NetherResource;
                                AddResource(resource, UnderCheatBase.NetherAmountAdd.Value);
                            }

                            break;

                        case 3:

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F2))
                            {
                                API.discover_tab_item_index = API.wrap_index(API.discover_tab_item_index - 1, API.Data.RelicCollection.Count);
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F3))
                            {
                                API.discover_tab_item_index = API.wrap_index(API.discover_tab_item_index + 1, API.Data.RelicCollection.Count);
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F4))
                            {
                                var item = API.GetItemDataIndex(API.discover_tab_item_index);
                                if (item is ItemData itemData)
                                {
                                    foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
                                    {
                                        GameData.Instance.Discover(itemData);
                                    }
                                }
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F5))
                            {
                                var item = API.GetItemDataIndex(API.discover_tab_item_index);
                                if (item is ItemData itemData)
                                {
                                    foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
                                    {
                                        Entity relic = API.SpawnRelic(itemData, player.Avatar.Position);
                                    }
                                }
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F6))
                            {
                                System.Random rand = new System.Random();

                                var relics = API.Data.RelicCollection;
                                int index = rand.Next(relics.Count);
                                var randomItem = relics[index];
                                if (randomItem is ItemData itemData)
                                {
                                    foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
                                    {
                                        Entity relic = API.SpawnRelic(itemData, player.Avatar.Position);
                                    }
                                }
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F7))
                            {
                                bool discoveredAndUnlockedOnly = false;
                                SummonAllRelics(discoveredAndUnlockedOnly);
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F8))
                            {
                                bool discoveredAndUnlockedOnly = true;
                                SummonAllRelics(discoveredAndUnlockedOnly);
                            }
                            break;

                        case 4:
                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F2))
                            {
                                bool removeCurse = false;
                                HealthExt.CurseType curseType = HealthExt.CurseType.Minor;
                                bool succeeded = ModifyCurses(removeCurse, curseType);
                                if (succeeded)
                                {
                                    Debug.Log($"{UnderCheatBase.modGUID}: Succesfully added {curseType} curse to player.");
                                } else
                                {
                                    Debug.LogError($"{UnderCheatBase.modGUID}: Failed to add {curseType} curse to player.");
                                }
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F3))
                            {
                                bool removeCurse = true;
                                HealthExt.CurseType curseType = HealthExt.CurseType.Minor;
                                bool succeeded = ModifyCurses(removeCurse, curseType);
                                if (succeeded)
                                {
                                    Debug.Log($"{UnderCheatBase.modGUID}: Succesfully removed {curseType} curse to player.");
                                }
                                else
                                {
                                    Debug.LogError($"{UnderCheatBase.modGUID}: Failed to remove {curseType} curse to player.");
                                }
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F4))
                            {
                                bool removeCurse = false;
                                HealthExt.CurseType curseType = HealthExt.CurseType.Major;
                                bool succeeded = ModifyCurses(removeCurse, curseType);
                                if (succeeded)
                                {
                                    Debug.Log($"{UnderCheatBase.modGUID}: Succesfully added {curseType} curse to player.");
                                }
                                else
                                {
                                    Debug.LogError($"{UnderCheatBase.modGUID}: Failed to add {curseType} curse to player.");
                                }
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F5))
                            {
                                bool removeCurse = true;
                                HealthExt.CurseType curseType = HealthExt.CurseType.Major;
                                bool succeeded = ModifyCurses(removeCurse, curseType);
                                if (succeeded)
                                {
                                    Debug.Log($"{UnderCheatBase.modGUID}: Succesfully removed {curseType} curse to player.");
                                }
                                else
                                {
                                    Debug.LogError($"{UnderCheatBase.modGUID}: Failed to remove {curseType} curse to player.");
                                }
                            }
                            break;
                    }
                    HUDControl.updateText();
                }
            }
        }

        public static void CheatDamage()
        {
            foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
            {
                if ((UnityEngine.Object)player.Avatar != (UnityEngine.Object)null)
                {
                    if (player.Avatar.HasModifier("CheatDamageMelee"))
                    {
                        player.Avatar.RemoveModifier("CheatDamageMelee");
                        player.Avatar.RemoveModifier("CheatDamageThrow");
                    }
                    else
                    {
                        player.Avatar.AddModifier(new Modifier()
                        {
                            id = "CheatDamageMelee",
                            typeName = "Thor.DamageExt",
                            memberName = "damageCategory1",
                            operatorName = Modifier.Assign.name,
                            floatAmount = UnderCheatBase.DamageBoostAmount.Value
                        });
                        player.Avatar.AddModifier(new Modifier()
                        {
                            id = "CheatDamageThrow",
                            typeName = "Thor.DamageExt",
                            memberName = "damageCategory3",
                            operatorName = Modifier.Assign.name,
                            floatAmount = UnderCheatBase.DamageBoostAmount.Value
                        });

                    }
                }
            }
        }

        static bool ModifyCurses(bool RemoveCurse, HealthExt.CurseType curseType)
        {
            foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
            {
                if ((UnityEngine.Object)player.Avatar != null)
                {
                    HealthExt healthExt = player.Avatar.GetExtension<HealthExt>();

                    if (RemoveCurse)
                    {
                        Entity curseEntity;
                        healthExt.RemoveRandomCurse(curseType, out curseEntity);
                        return true;
                    }
                    else
                    {
                        healthExt.AddRandomCurse(curseType, player.Avatar);
                        return true;
                    }
                }
            }
            return false;
        }

        static void SummonAllRelics(bool discoveredAndUnlockedOnly)
        {
            Debug.Log($"{UnderCheatBase.modGUID}: Attempting to spawn all {(discoveredAndUnlockedOnly ? "discovered" : "")} relics.");
            int spawned_relic_count = 0;
            foreach (var item in API.Data.RelicCollection)
            {
                if (item is ItemData itemData)
                {
                    void SpawnRelic()
                    {
                        foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
                        {
                            Entity relic = API.SpawnRelic(itemData, player.Avatar.Position);
                            spawned_relic_count++;
                        }
                    }

                    if (discoveredAndUnlockedOnly)
                    {
                        if (itemData.IsDiscovered && itemData.IsUnlocked)
                        {
                            SpawnRelic();
                        }
                    } else
                    {
                        SpawnRelic();
                    }
                }
            }
            Debug.Log($"{UnderCheatBase.modGUID}: Successfully spawned {spawned_relic_count} relics.");
        }

        static void ToggleDoors()
        {
            if (Game.Instance.Simulation.Zone.CurrentRoom.DoorState == Room.DoorStateType.Open)
            {
                Debug.Log($"{UnderCheatBase.modGUID}: Closing Doors");
                Game.Instance.Simulation.Zone.CurrentRoom.CloseDoors();
            }
            else
            {
                Debug.Log($"{UnderCheatBase.modGUID}: Opening Doors");
                Game.Instance.Simulation.Zone.CurrentRoom.OpenDoors();
            }
        }

        static void UnlockAll()
        {
            foreach (ItemData itemData in GameData.Instance.Items)
            {
                GameData.Instance.Unlock(itemData);
                GameData.Instance.Discover((DataObject)itemData);
            }

            Debug.Log($"{UnderCheatBase.modGUID}: Unlocking All Items");
        }

        static void AddResource(ResourceData resource, int changeInt)
        {
                foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
                {
                    if ((UnityEngine.Object)player.Avatar != (UnityEngine.Object)null)
                    {
                        InventoryExt extension = player.Avatar.GetExtension<InventoryExt>();
                        if ((UnityEngine.Object)extension != (UnityEngine.Object)null)
                        {
                            if (!sim.IsPaused)
                            {
                                Dictionary<ResourceData, string> resourceDict = new Dictionary<ResourceData, string>
                                {
                                    { GameData.Instance.KeyResource, "key" },
                                    { GameData.Instance.BombResource, "bomb" },
                                    { GameData.Instance.GoldResource, "gold" },
                                    { GameData.Instance.ThoriumResource, "thorium" },
                                    { GameData.Instance.NetherResource, "nether" },
                                };

                                extension.ChangeResource(resource, changeInt, (List<string>)null, false, (Entity)null);
                                Debug.Log($"{UnderCheatBase.modGUID}: Attempted to change `{resourceDict[resource]}` by {changeInt} from {extension.GetResource(resource) - changeInt} to {extension.GetResource(resource)}");
                            }
                        }
                    }
                }
        }
        static void MaxPetLevel()
        {
            PetOwnerExt extension1 = Game.Instance.Simulation.Avatars[0].GetExtension<PetOwnerExt>();
            if (UnityEngine.Object.Equals((UnityEngine.Object)extension1, (UnityEngine.Object)null))
            {
                return;
            }
            foreach (PetOwnerExt.PetSlot petSlot in extension1.PetSlots)
            {
                if (!UnityEngine.Object.Equals((UnityEngine.Object)petSlot.pet, (UnityEngine.Object)null))
                {
                    InventoryExt extension2 = petSlot.pet.GetExtension<InventoryExt>();
                    if (!UnityEngine.Object.Equals((UnityEngine.Object)extension2, (UnityEngine.Object)null))
                    {
                        extension2.ChangeResource(GameData.Instance.XPResource, extension2.GetMaxResource(GameData.Instance.XPResource) - extension2.GetResource(GameData.Instance.XPResource), (List<string>)null, false, (Entity)null);
                    }
                    Debug.Log($"{UnderCheatBase.modGUID}: Set pet's level to max");
                }
            }
        }

        [HarmonyPatch(typeof(Game))]
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void Awake(ref Simulation ___m_simulation)
        {
            sim = ___m_simulation;

        }
    }
}
