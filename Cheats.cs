using HarmonyLib;
using System.Collections.Generic;
using System;
using Thor;
using Rewired;
using UnityEngine;
using Undercheat;
using System.Reflection;

namespace UnderCheat
{

    [HarmonyPatch(typeof(Game))]
    internal class Cheats
    {
        public static bool playerInvincible = false;
        static Simulation sim;

        static List<KeyCode> keyList = new List<KeyCode> {
            KeyCode.T,
            KeyCode.F1,
            KeyCode.F2,
            KeyCode.F3,
            KeyCode.F4,
            KeyCode.F5,
            KeyCode.F6,
            KeyCode.F7
        };

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void patchUpdate()
        {
            foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
            {
                if ((UnityEngine.Object)player.Avatar != (UnityEngine.Object)null)
                {
                    HealthExt extension = player.Avatar.GetExtension<HealthExt>();
                    if ((UnityEngine.Object)extension != (UnityEngine.Object)null)
                    {
                        extension.Invulnerable = playerInvincible;
                    }
                }
            }
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
                                SetInvulnerable();
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F3))
                            {
                                ToggleDoors();
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F4))
                            {
                                UnlockAll();
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F5))
                            {
                                MaxPetLevel();
                            }

                            break;

                        case 2:

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F2))
                            {
                                AddResource("key", UnderCheatBase.KeyAmountAdd.Value);
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F3))
                            {
                                AddResource("bomb", UnderCheatBase.BombAmountAdd.Value);
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F4))
                            {
                                AddResource("gold", UnderCheatBase.GoldAmountAdd.Value);
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F5))
                            {
                                AddResource("thorium", UnderCheatBase.ThoriumAmountAdd.Value);
                            }

                            if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.F6))
                            {
                                AddResource("nether", UnderCheatBase.NetherAmountAdd.Value);
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
                                foreach (var item in API.Data.RelicCollection)
                                {
                                    if (item is ItemData itemData)
                                    {
                                        foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
                                        {
                                            Entity relic = API.SpawnRelic(itemData, player.Avatar.Position);
                                        }
                                    }
                                }
                            }
                            break;
                    }
                    HUDControl.updateText();
                }
            }
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

        static void SetInvulnerable()
        {
            foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
            {
                if ((UnityEngine.Object)player.Avatar != (UnityEngine.Object)null)
                {
                    HealthExt extension = player.Avatar.GetExtension<HealthExt>();
                    if ((UnityEngine.Object)extension != (UnityEngine.Object)null)
                    {
                        if (playerInvincible) 
                        {
                            Debug.Log($"{UnderCheatBase.modGUID}: Disabling Infinite HP");
                        }
                        else
                        {
                            extension.SetCurrentHP(extension.MaxHP);
                            Debug.Log($"{UnderCheatBase.modGUID}: Enabling Infinite HP");
                        }
                        playerInvincible = !playerInvincible;
                    }
                }
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

        static void AddResource(string resource, int changeInt)
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
                                switch (resource)
                                {
                                    case "key":

                                        extension.ChangeResource(GameData.Instance.KeyResource, changeInt, (List<string>)null, false, (Entity)null);
                                        Debug.Log($"{UnderCheatBase.modGUID}: Changed {resource} by {changeInt} from {extension.GetResource(GameData.Instance.KeyResource) - changeInt} to {extension.GetResource(GameData.Instance.KeyResource)}");
                                        break;

                                    case "bomb":

                                        extension.ChangeResource(GameData.Instance.BombResource, changeInt, (List<string>)null, false, (Entity)null);
                                    Debug.Log($"{UnderCheatBase.modGUID}: Changed {resource} by {changeInt} from {extension.GetResource(GameData.Instance.BombResource) - changeInt} to {extension.GetResource(GameData.Instance.BombResource)}");
                                    break;

                                    case "gold":

                                        extension.ChangeResource(GameData.Instance.GoldResource, changeInt, (List<string>)null, false, (Entity)null);
                                    Debug.Log($"{UnderCheatBase.modGUID}: Changed {resource} by {changeInt} from {extension.GetResource(GameData.Instance.GoldResource) - changeInt} to {extension.GetResource(GameData.Instance.GoldResource)}");
                                    break;

                                    case "thorium":

                                        extension.ChangeResource(GameData.Instance.ThoriumResource, changeInt, (List<string>)null, false, (Entity)null);
                                        Debug.Log($"{UnderCheatBase.modGUID}: Changed {resource} by {changeInt} from {extension.GetResource(GameData.Instance.ThoriumResource) - changeInt} to {extension.GetResource(GameData.Instance.ThoriumResource)}");
                                        break;

                                    case "nether":

                                        extension.ChangeResource(GameData.Instance.NetherResource, changeInt, (List<string>)null, false, (Entity)null);
                                        Debug.Log($"{UnderCheatBase.modGUID}: Changed {resource} by {changeInt} from {extension.GetResource(GameData.Instance.NetherResource) - changeInt} to {extension.GetResource(GameData.Instance.NetherResource)}");
                                        break;

                                    default:
                                        Debug.LogError($"{UnderCheatBase.modGUID}: Error occured whilst changing '{resource}' count by `{changeInt}`");
                                        break;
                                }
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
        static void OnDestroysAvatar(PlayerEvent playerEvent)
        {
            foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
            {
                if ((UnityEngine.Object)player.Avatar != (UnityEngine.Object)null)
                {
                    HealthExt extension = player.Avatar.GetExtension<HealthExt>();
                    if ((UnityEngine.Object)extension != (UnityEngine.Object)null)
                    {
                        playerInvincible = extension.Invulnerable;
                    }
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
        [HarmonyPatch(typeof(HUD))]
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void Initialize()
        {
            foreach (SimulationPlayer player in Game.Instance.Simulation.Players)
            {
                player.RegisterEvent(PlayerEvent.EventType.DestroysAvatar, OnDestroysAvatar);
            }
        }
    }
}
