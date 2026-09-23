using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Thor;
using UnityEngine;

namespace UnderCheat.Helpers
{
    public static class CheatManager
    {
        public static bool playerImmunity = false;
        private static SimulationPlayer player => Game.Instance.Simulation.PrimaryPlayer;
        [HarmonyPatch(typeof(HealthExt), nameof(HealthExt.ChangeHP))]
        static void Prefix(HealthExt __instance, ref HealthExt.ChangeHPArgs args)
        {
            if (player.Avatar != __instance.Entity) return;
            if (!playerImmunity) return;
            
            float percentageMultiplier = (100 - UnderCheatBase.DamageReduceHackPercentage.Value) / 100;
            float result = args.delta * percentageMultiplier;
            int deltaOut = (int)Mathf.Round(result);

            Debug.Log($"{UnderCheatBase.ModGuid}: Reduced player incoming damage by {args.delta - deltaOut}");

            if (args.delta < 0)
            {
                args.delta = deltaOut;
            }
        }
        
        public static void CheatDamage()
        {
            if (!player.Avatar) return;
            
            if (player.Avatar.HasModifier("UnderCheat.meleeDamageBoost"))
            {
                player.Avatar.RemoveModifier("UnderCheat.meleeDamageBoost");
                player.Avatar.RemoveModifier("UnderCheat.meleeSpeedBoost");
                player.Avatar.RemoveModifier("UnderCheat.throwDamageBoost");
            }
            else
            {
                player.Avatar.AddModifier(new Modifier()
                {
                    id = "UnderCheat.meleeDamageBoost",
                    typeName = "Thor.DamageExt",
                    memberName = "damageCategory1",
                    operatorName = Modifier.Assign.name,
                    floatAmount = UnderCheatBase.DamageBoostAmount.Value
                });
                player.Avatar.AddModifier(new Modifier()
                {
                    id = "UnderCheat.meleeSpeedBoost",
                    typeName = "Thor.DamageExt",
                    memberName = "AttackSpeed",
                    operatorName = Modifier.Assign.name,
                    floatAmount = UnderCheatBase.DamageAttackSpeed.Value
                });
                player.Avatar.AddModifier(new Modifier()
                {
                    id = "UnderCheat.throwDamageBoost",
                    typeName = "Thor.DamageExt",
                    memberName = "damageCategory3",
                    operatorName = Modifier.Assign.name,
                    floatAmount = UnderCheatBase.DamageBoostAmount.Value
                });
            }
        }

        public static void ModifyCurses(bool removeCurse, HealthExt.CurseType curseType)
        {
            if (!player.Avatar) return;
            
            HealthExt healthExt = player.Avatar.GetExtension<HealthExt>();
            if (removeCurse)
            {
                healthExt.RemoveRandomCurse(curseType, out _);
            }
            else
            {
                healthExt.AddRandomCurse(curseType, player.Avatar);
            }
        }
        
        public static ItemData GetItemDataIndex(int index)
        {
            index = PageManager.WrapIndex(index,0,GameData.Instance.RelicCollection.Count-1);
            return GameData.Instance.RelicCollection.ElementAt(index) is ItemData itemData ? itemData : null;
        }
        
        public static Entity SpawnRelic(ItemData data, Vector3 position)
        {
            try
            {
                if (data == null)
                {
                    Debug.LogWarning($"{UnderCheatBase.ModGuid}: Item's Data cannot be null!");
                    return null;
                }

                var prefab = GameData.Instance.GetItemTemplate(data);
                if (prefab == null)
                {
                    Debug.LogWarning($"{UnderCheatBase.ModGuid}: Could not find item template for: " + data.name);
                    return null;
                }

                using (new ItemExt.ItemDataScope(data))
                {
                    var entity = Game.Instance.Simulation.SpawnEntity(prefab, position, Quaternion.identity, -1, null);
                    Debug.Log($"{UnderCheatBase.ModGuid}: Created new relic with name : `{data.name}`");
                    var mover = entity.GetExtension<MoverExt>();
                    if (mover == null) return entity;

                    entity.transform.position += Vector3.up;
                    var normalized = Rand.InsideUnitCircle.normalized;
                    var point = new Vector3(normalized.x * Rand.Range(2f, 6f), 0.0f, normalized.y * Rand.Range(2f, 4f));
                    var walkable = Simulation.GetNearestWalkablePosition(entity.transform.LocalToWorldPoint(point), entity.AgentTypeID);
                    mover.Launch(walkable, Rand.Range(4, 5), 75f, false);
                    return entity;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{UnderCheatBase.ModGuid}: Error occurred while spawning relic: " + e);
                return null;
            }
        }
        
        private static void SpawnRelicAtPlayers(ItemData itemData)
        {
            SpawnRelic(itemData, player.Avatar.Position);
        }

        public static void SummonAllRelics(bool discoveredAndUnlockedOnly)
        {
            Debug.Log($"{UnderCheatBase.ModGuid}: Attempting to spawn all {(discoveredAndUnlockedOnly ? "discovered" : "")} relics.");
            int spawnedRelicCount = 0;
            foreach (var item in GameData.Instance.RelicCollection)
            {
                if (!(item is ItemData itemData)) continue;

                if (!discoveredAndUnlockedOnly)
                {
                    spawnedRelicCount++;
                    SpawnRelicAtPlayers(itemData);
                    continue;
                }
                
                if (!itemData.IsDiscovered || !itemData.IsUnlocked) continue;
                spawnedRelicCount++;
                SpawnRelicAtPlayers(itemData);
            }
            Debug.Log($"{UnderCheatBase.ModGuid}: Successfully spawned {spawnedRelicCount} relics.");
        }

        public static void ToggleDoors()
        {
            if (Game.Instance.Simulation.Zone.CurrentRoom.DoorState == Room.DoorStateType.Open)
            {
                Debug.Log($"{UnderCheatBase.ModGuid}: Closing Doors");
                Game.Instance.Simulation.Zone.CurrentRoom.CloseDoors();
            }
            else
            {
                Debug.Log($"{UnderCheatBase.ModGuid}: Opening Doors");
                Game.Instance.Simulation.Zone.CurrentRoom.OpenDoors();
            }
        }

        public static void UnlockAll()
        {
            foreach (ItemData itemData in GameData.Instance.Items)
            {
                GameData.Instance.Unlock(itemData);
                GameData.Instance.Discover(itemData);
            }

            Debug.Log($"{UnderCheatBase.ModGuid}: Unlocking All Items");
        }

        public static void AddResource(ResourceData resource, int changeInt)
        {
            if (!player.Avatar) return;
            
            InventoryExt inventoryExt = player.Avatar.GetExtension<InventoryExt>();
            if (!inventoryExt) return;

            inventoryExt.ChangeResource(resource, changeInt, (List<string>)null, false, (Entity)null);
            Debug.Log($"{UnderCheatBase.ModGuid}: Attempted to change `{resource.Name}` by {changeInt} from {inventoryExt.GetResource(resource) - changeInt} to {inventoryExt.GetResource(resource)}");
        }
        public static void MaxPetLevel()
        {
            PetOwnerExt petOwnerExt = Game.Instance.Simulation.Avatars[0].GetExtension<PetOwnerExt>();
            if (!petOwnerExt) return;
            
            foreach (PetOwnerExt.PetSlot petSlot in petOwnerExt.PetSlots)
            {
                if (!petSlot.pet) continue;
                
                InventoryExt inventoryExt = petSlot.pet.GetExtension<InventoryExt>();
                if (!inventoryExt) continue;
                if (inventoryExt.GetMaxResource(GameData.Instance.XPResource) == inventoryExt.GetResource(GameData.Instance.XPResource)) continue;
                
                inventoryExt.ChangeResource(GameData.Instance.XPResource, inventoryExt.GetMaxResource(GameData.Instance.XPResource) - inventoryExt.GetResource(GameData.Instance.XPResource), (List<string>)null, false, (Entity)null);
                Debug.Log($"{UnderCheatBase.ModGuid}: Set pet's level to max");
            }
        }
    }
}
