using System.Text.RegularExpressions;
using HarmonyLib;
using Thor;
using TMPro;
using UnityEngine;

namespace UnderCheat.Helpers
{
    internal static class HudManager
    {
        // Variables
        private static SimulationPlayer player => Game.Instance.Simulation.PrimaryPlayer;
        private static GameObject panelGameObject;
        private static TextMeshProUGUI textPanel;
        public static bool Hidden;
        private const float LerpSpeed = 4f;
        private static int lastKeyAmount = UnderCheatBase.KeyAmountAdd.Value;
        private static int lastBombAmount = UnderCheatBase.BombAmountAdd.Value;
        private static int lastGoldAmount = UnderCheatBase.GoldAmountAdd.Value;
        private static int lastThoriumAmount = UnderCheatBase.ThoriumAmountAdd.Value;
        private static int lastNetherAmount = UnderCheatBase.NetherAmountAdd.Value;
        private static bool hintsHidden => UnderCheatBase.HideConfigHints.Value;

        private static readonly Vector3 RestPosition = new Vector3(-938.0f, 272.0f, 0.0f);
        private static readonly Vector3 RestPositionOffset = new Vector3(-1250,0,0);

        private static string DecorateText(string text, bool condition, string prefix, string postfix) => condition ? $"{prefix}{text}{postfix}" : text;

        private static void UpdateText()
        {
            if (!panelGameObject) { return; }
            if (!textPanel) { return; }
            if (!player.Avatar) return;

            textPanel.text = "";
            if (Hidden) return;

            textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"toggle_ui")}: Toggle UI<br>";
            textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"next_page")}: Next page ({PageManager.nextPage})<br>";

            lastKeyAmount = UnderCheatBase.KeyAmountAdd.Value;
            lastBombAmount = UnderCheatBase.BombAmountAdd.Value;
            lastGoldAmount = UnderCheatBase.GoldAmountAdd.Value;
            lastThoriumAmount = UnderCheatBase.ThoriumAmountAdd.Value;
            lastNetherAmount = UnderCheatBase.NetherAmountAdd.Value;
            switch (PageManager.currentPage)
            {
                case 1:
                {
                    bool PetMaxed(SimulationPlayer petOwner = null)
                    {
                        petOwner = petOwner ?? player;
                        PetOwnerExt petExt = petOwner.Avatar.GetExtension<PetOwnerExt>();
                        PetOwnerExt.PetSlot petSlot = petExt.PetSlots[0];
                        if (!petSlot.pet) return false;

                        InventoryExt extension1 = petSlot.pet.GetExtension<InventoryExt>();
                        if (!extension1) return false;
                        if (extension1.GetResource(GameData.Instance.XPResource) != extension1.GetMaxResource(GameData.Instance.XPResource)) return false;
                        return true;
                    }

                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"toggle_damage_reducer")}: {DecorateText($"Toggle Damage Reducer ({UnderCheatBase.DamageReduceHackPercentage.Value}%)",CheatManager.playerImmunity,"<color=yellow>","</color>")}<br>";
                    float damageBoostAmount = UnderCheatBase.DamageBoostAmount.Value;
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"toggle_attack_damage_booster")}: {DecorateText($"Toggle Attack Damage Booster ({DecorateText($"{damageBoostAmount}",damageBoostAmount < 0,"+","")} DMG)",player.Avatar.HasModifier("UnderCheat.meleeDamageBoost"),"<color=yellow>","</color>")}<br>";
                    textPanel.text += hintsHidden ? "" : "<br><color=#c3c3c3>> Damage reducer amount can be edited in mod's config <<br>> Hints can also be disabled <</color><br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"toggle_closed_doors")}: Toggle closed doors<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"unlock_all_items")}: Unlock All Items<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"max_pet_level")}: {DecorateText("Max Pet Level", PetMaxed(player), "<color=red>","</color>")}<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"refresh_config")}: Refresh Config<br>";
                    textPanel.text += hintsHidden ? "" : "<br><color=#c3c3c3>> Due to sandboxing, config can't be opened when running proton <</color><br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"open_config_file")}: {DecorateText("Open Config File", FileManager.isProton, "<color=red><s>","</s></color>")}<br>";
                    break;
                }

                case 2:
                    // Resource Texts
                    string keyText = (lastKeyAmount < 0 ? "Removes " : "Adds ") + Mathf.Abs(lastKeyAmount) + (Mathf.Abs(lastKeyAmount) == 1 ? " key" : " keys");
                    string bombText = (lastBombAmount < 0 ? "Removes " : "Adds ") + Mathf.Abs(lastBombAmount) + (Mathf.Abs(lastBombAmount) == 1 ? " bomb" : " bombs");
                    string goldText = (lastGoldAmount < 0? "Removes " : "Adds ") + Mathf.Abs(lastGoldAmount) + " gold";
                    string thoriumText = (lastThoriumAmount < 0 ? "Removes " : "Adds ") + Mathf.Abs(lastThoriumAmount) + " thorium";
                    string netherText = (lastNetherAmount < 0 ? "Removes " : "Adds ") + Mathf.Abs(lastNetherAmount) + " nether";

                    // Show max resource quantity
                    InventoryExt inventoryExt = player.Avatar.GetExtension<InventoryExt>();
                    keyText += $" ({inventoryExt.GetResource(GameData.Instance.KeyResource)}/{inventoryExt.GetMaxResource(GameData.Instance.KeyResource)})";
                    bombText += $" ({inventoryExt.GetResource(GameData.Instance.BombResource)}/{inventoryExt.GetMaxResource(GameData.Instance.BombResource)})";
                    goldText += $" ({inventoryExt.GetResource(GameData.Instance.GoldResource)}/{inventoryExt.GetMaxResource(GameData.Instance.GoldResource)})";
                    thoriumText += $" ({inventoryExt.GetResource(GameData.Instance.ThoriumResource)}/{inventoryExt.GetMaxResource(GameData.Instance.ThoriumResource)})";
                    netherText += $" ({inventoryExt.GetResource(GameData.Instance.NetherResource)}/{inventoryExt.GetMaxResource(GameData.Instance.NetherResource)})";

                    // Change text color if maxed
                    bool IsMaxed(ResourceData resource, int currentAmount)
                    {
                        int futureQuantity = (inventoryExt.GetResource(resource) + 1) * (currentAmount > 0 ? 1 : -1);
                        return futureQuantity < inventoryExt.GetMinResource(resource) || futureQuantity > inventoryExt.GetMaxResource(resource);
                    }
                    keyText = DecorateText(keyText,IsMaxed(GameData.Instance.KeyResource,lastKeyAmount),"<color=red>","</color>");
                    bombText = DecorateText(bombText,IsMaxed(GameData.Instance.BombResource,lastBombAmount),"<color=red>","</color>");
                    goldText = DecorateText(goldText,IsMaxed(GameData.Instance.GoldResource,lastGoldAmount),"<color=red>","</color>");
                    thoriumText = DecorateText(thoriumText,IsMaxed(GameData.Instance.ThoriumResource,lastThoriumAmount),"<color=red>","</color>");
                    netherText = DecorateText(netherText,IsMaxed(GameData.Instance.NetherResource,lastNetherAmount),"<color=red>","</color>");

                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"mod_keys")}: {keyText}<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"mod_bombs")}: {bombText}<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"mod_gold")}: {goldText}<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"mod_thorium")}: {thoriumText}<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"mod_nether")}: {netherText}<br>";
                    textPanel.text += hintsHidden ? "" : "<br><color=#c3c3c3>> Amounts can be edited in mod's config <<br>> Hints can also be disabled <</color>";
                    break;

                case 3:
                    string CapitalizeSpace(string text) => Regex.Replace(text, @"[A-Z]", match => match.Index == 0 ? match.Value : $" {match.Value}");

                    ItemData currentItem = CheatManager.GetItemDataIndex(PageManager.discoverPageItemIndex);
                    if (!currentItem) break;
                    ItemData previousItem = CheatManager.GetItemDataIndex(PageManager.discoverPageItemIndex - 1);
                    if (!previousItem) break;
                    ItemData nextItem = CheatManager.GetItemDataIndex(PageManager.discoverPageItemIndex + 1);
                    if (!nextItem) break;

                    textPanel.text += $"<color=grey>{CapitalizeSpace(previousItem.name)}</color> <- {CapitalizeSpace(currentItem.name)} -> <color=grey>{CapitalizeSpace(nextItem.name)}</color><br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"item_previous")}: Previous item<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"item_next")}: Next item<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"discover_relic")}: {DecorateText($"Discover {CapitalizeSpace(currentItem.name)}", currentItem.IsDiscovered, "<color=red>","</color>")}<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"spawn_relic")}: Spawn {CapitalizeSpace(currentItem.name)} on player<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"spawn_random_relic")}: Spawn a random relic on player<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"spawn_all_relics")}: Spawn all relics on player<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"spawn_all_discovered_relics")}: Spawn all unlocked relics on player<br>";
                    break;

                case 4:
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"add_minor_curse")}: Add random Minor curse<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"remove_minor_curse")}: Remove random Minor curse<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"add_major_curse")}: Add random Major curse<br>";
                    textPanel.text += $"{KeybindLib.KeybindManager.GetKey(UnderCheatBase.ModGuid,"remove_major_curse")}: Remove random Major curse<br>";
                    break;
            }
        }

        [HarmonyPatch(typeof(Simulation))]
        [HarmonyPatch("Process")]
        [HarmonyPostfix]
        private static void Update()
        {
            if (!player.Avatar) return; // GUI shouldn't have appeared until player avatar spawn.
            if (panelGameObject == null || textPanel == null) return;
            RectTransform panelGameObjectRect = panelGameObject.GetComponent<RectTransform>();

            if (!Mathf.Approximately(Mathf.Round(panelGameObjectRect.anchoredPosition3D.x), Mathf.Round(RestPosition.x)))
            {
                panelGameObjectRect.anchoredPosition3D = Vector3.Lerp(
                        panelGameObjectRect.anchoredPosition3D,
                        RestPosition,
                        Time.deltaTime * LerpSpeed
                    );
            }
            if (Hidden)
            {
                textPanel.text = "";
                return;
            }

            InventoryExt inventoryExt = player.Avatar.GetExtension<InventoryExt>();
            if (inventoryExt == null) return;

            bool keyMismatched = inventoryExt.GetResource(GameData.Instance.KeyResource) != lastKeyAmount;
            bool bombMismatched = inventoryExt.GetResource(GameData.Instance.BombResource) != lastBombAmount;
            bool goldMismatched = inventoryExt.GetResource(GameData.Instance.GoldResource) != lastGoldAmount;
            bool thoriumMismatched = inventoryExt.GetResource(GameData.Instance.ThoriumResource) != lastThoriumAmount;
            bool netherMismatched = inventoryExt.GetResource(GameData.Instance.NetherResource) != lastNetherAmount;
            bool amountMismatched = keyMismatched || bombMismatched || goldMismatched || thoriumMismatched || netherMismatched;

            if (amountMismatched) { UpdateText(); }
        }

        private static void OnSpawnsAvatar(PlayerEvent playerEvent)
        {
            // Create GUI
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            panelGameObject = new GameObject("UndercheatPanel", typeof(RectTransform), typeof(CanvasGroup))
            {
                name = $"{UnderCheatBase.ModGuid}.TextPanel"
            };
            panelGameObject.transform.SetParent(canvas.transform);

            RectTransform panelGameObjectRect = panelGameObject.GetComponent<RectTransform>();
            panelGameObjectRect.sizeDelta = new Vector2(200, 100);
            panelGameObjectRect.anchoredPosition3D = RestPosition + RestPositionOffset;
            panelGameObjectRect.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            // Create Text
            GameObject textGameObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGameObject.transform.SetParent(panelGameObject.transform, false);

            textPanel = textGameObject.GetComponent<TextMeshProUGUI>();
            textPanel.text = "";
            textPanel.fontSize = 16;
            textPanel.color = Color.white;
            textPanel.alignment = (TextAlignmentOptions)TextAnchor.UpperLeft;

            // Show GUI
            Debug.Log($"{UnderCheatBase.ModGuid}: Showing GUI");
            PageManager.Reset();
            UpdateText();
        }

        private static void OnDestroysAvatar(PlayerEvent playerEvent)
        {
            // Delete GUI
            Object.Destroy(panelGameObject);
            panelGameObject = null;
            PageManager.currentPage = 1;
            Debug.Log($"{UnderCheatBase.ModGuid}: Hiding GUI");
        }

        [HarmonyPatch(typeof(HUD))]
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        private static void Awake()
        {
            Debug.Log($"{UnderCheatBase.ModGuid}: Creating UI Events");
            player.RegisterEvent(PlayerEvent.EventType.SpawnsAvatar, OnSpawnsAvatar);
            player.RegisterEvent(PlayerEvent.EventType.DestroysAvatar, OnDestroysAvatar);
            Debug.Log($"{UnderCheatBase.ModGuid}: Done Creating UI Events");
        }
    }
}
