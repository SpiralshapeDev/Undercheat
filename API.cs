using BepInEx.Logging;
using HarmonyLib;
using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Thor;
using UnderCheat;
using UnityEngine;

namespace Undercheat
{
    internal class API
    {
        public static GameData Data => GameData.Instance;
        internal static ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(UnderCheatBase.modGUID);
        public static Game Game => Game.Instance;
        public static Simulation Simulation => Game.Simulation;

        public static int min_page = 1;
        public static int max_page = 3;
        public static int current_page = min_page;
        public static int discover_tab_item_index = 0;

        public static int next_page()
        {
            int page = current_page;
            page++;
            if (max_page < page || min_page > page)
            {
                page = min_page;
            }
            return page;
        }

        public static string CapitalizedSpace(string text)
        {
            bool firstMatch = true;

            string result = Regex.Replace(text, @"[A-Z]", match =>
            {
                if (firstMatch)
                {
                    firstMatch = false;
                    return $"{match.Value}";
                }
                else
                {
                    return $" {match.Value}";
                }
            });
            return result;
        }

        public static int wrap_index(int index, int max_index)
        {
            return ((index % max_index) + max_index) % max_index;
        }

        public static ItemData GetItemDataIndex(int index)
        {
            var relics = Data.RelicCollection;
            index = wrap_index(index, relics.Count);

            var item = relics[index];
            if (item is ItemData itemData)
            {
                return itemData;
            }
            return null;
        }

        public static void PrintRelicList()
        {
            foreach (var item in Data.RelicCollection)
            {
                if (item is ItemData itemData)
                {
                    mls.LogInfo($"Relic: {itemData.name}");
                }
            }
        }


        public static Entity SpawnRelic(ItemData data, Vector3 position)
        {
            try
            {
                if (data == null)
                {
                    mls.LogWarning("Item's Data cannot be null!");
                    return null;
                }

                var prefab = Data?.GetItemTemplate(data);
                if (prefab == null)
                {
                    mls.LogWarning("Could not find item template for: " + data?.name);
                    return null;
                }

                using (new ItemExt.ItemDataScope(data))
                {
                    var entity = Simulation.SpawnEntity(prefab, position, Quaternion.identity, -1, null);
                    mls.LogInfo($"Created new relic with name : `{data.name}`");
                    var mover = entity.GetExtension<MoverExt>();
                    if (mover == null)
                        return entity;

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
                mls.LogError("Error occurred while spawning relic: " + e);
                return null;
            }
        }
        public static ItemData GetRelic(string name)
        {
            var dataObject = Data.RelicCollection.FirstOrDefault(i => i.name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (dataObject == null)
            {
                mls.LogWarning($"Item with name '{name}' not found.");
                return null;
            }

            if (dataObject is ItemData itemData)
                return itemData;

            mls.LogWarning($"DataObject '{name}' is not an ItemData.");
            return null;
        }
    }
}
