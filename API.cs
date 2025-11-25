using System;
using System.Linq;
using System.Text.RegularExpressions;
using Thor;
using UnderCheat;
using UnityEngine;
using BepInEx;

namespace Undercheat
{
    internal class API
    {
        public static GameData Data => GameData.Instance;
        private static Game Game => Game.Instance;
        private static Simulation Simulation => Game.Simulation;

        static readonly int MinPage = 1;
        static readonly int MaxPage = 4;
        public static int CurrentPage = MinPage;
        public static int DiscoverTabItemIndex = 0;
        
        public static bool IsProton = BepInEx.Paths.ConfigPath.Contains("Z:\\");

        public static int next_page()
        {
            int page = CurrentPage;
            page++;
            if (MaxPage < page || MinPage > page)
            {
                page = MinPage;
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

        public static int wrap_index(int index, int maxIndex)
        {
            return ((index % maxIndex) + maxIndex) % maxIndex;
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

        public static void PrintRelicList(bool discoveredOnly)
        {
            foreach (var item in Data.RelicCollection)
            {
                if (!(item is ItemData itemData)) { break; }
                
                if (discoveredOnly)
                {
                    if (!itemData.IsDiscovered) { break; }
                    
                    Debug.Log($"{UnderCheatBase.ModGuid}: Relic: {itemData.name}");
                }
                else
                {
                    Debug.Log($"{UnderCheatBase.ModGuid}: Relic: {itemData.name}");
                }
            }
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

                var prefab = Data.GetItemTemplate(data);
                if (prefab == null)
                {
                    Debug.LogWarning($"{UnderCheatBase.ModGuid}: Could not find item template for: " + data.name);
                    return null;
                }

                using (new ItemExt.ItemDataScope(data))
                {
                    var entity = Simulation.SpawnEntity(prefab, position, Quaternion.identity, -1, null);
                    Debug.Log($"{UnderCheatBase.ModGuid}: Created new relic with name : `{data.name}`");
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
                Debug.LogError($"{UnderCheatBase.ModGuid}: Error occurred while spawning relic: " + e);
                return null;
            }
        }
        public static ItemData GetRelic(string name)
        {
            var dataObject = Data.RelicCollection.FirstOrDefault(i => i.name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (dataObject == null)
            {
                Debug.LogWarning($"{UnderCheatBase.ModGuid}: Item with name '{name}' not found.");
                return null;
            }

            if (dataObject is ItemData itemData) { return itemData; }

            Debug.LogWarning($"{UnderCheatBase.ModGuid}: DataObject '{name}' is not an ItemData.");
            return null;
        }
    }
}
